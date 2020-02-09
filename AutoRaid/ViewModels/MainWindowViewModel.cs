using AutoRaid.Adb;
using Prism.Commands;
using Prism.Mvvm;
using SharpAdbClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using JetBrains.Annotations;
using Color = System.Drawing.Color;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace AutoRaid.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "AutoRaid: VDT";
        private ICommand _playAgainCommand;
        private Random _random;
        private readonly IAdbService _adbService;
        private ICommand _takeScreenShotCommand;

        public MainWindowViewModel(IAdbService adbService)
        {
            _adbService = adbService;

        }

        private ImageSource _screenShot;
        public ImageSource Screenshot
        {
            get { return _screenShot; }
            set { SetProperty(ref _screenShot, value); }
        }

        private ImageSource _cropped;
        private ImageSource _scaled;

        public ImageSource Cropped
        {
            get { return _cropped; }
            set { SetProperty(ref _cropped, value); }
        }

        public ImageSource Scaled
        {
            get { return _scaled; }
            set { SetProperty(ref _scaled, value); }
        }

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }


        public ICommand PlayAgainCommand
        {
            get { return _playAgainCommand ??= new DelegateCommand(ExecutePlayAgainCommand); }
        }

        public ICommand TakeScreenshotCommand
        {
            get { return _takeScreenShotCommand ??= new DelegateCommand(ExecuteTakeScreenshotCommand); }
        }

        private void ExecuteTakeScreenshotCommand()
        {
            using (var ms = new MemoryStream(_adbService.GetScreenshot()))
            {
                ms.Position = 0;
                var image = new BitmapImage();
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = ms;
                image.EndInit();

                var croppedImage = new CroppedBitmap(image, new Int32Rect(1500, 20, 80, 80));

                Screenshot = image;
                Cropped = croppedImage;
                
                var thumbnail = CreateResizedImage(croppedImage, 16, 16, 0);

                using (var bitmap = ConvertToBitmap(thumbnail))
                {
                    var blackWhiteResult = CreateBlackAndWhite(bitmap);
                    Scaled = ConvertToImageSource(blackWhiteResult.bitmap);
                    var sb = new StringBuilder();
                    var hash = blackWhiteResult.hash;
                    for (var i = 0; i < hash.Length; i++)
                    {
                        sb.Append(hash[i]);
                        if (i > 0 && i % 16 == 0)
                        {
                            sb.AppendLine();
                        }
                    }

                    Debug.Write(sb.ToString());
                }
                
            }
            
        }



        private static BitmapFrame CreateResizedImage(ImageSource source, int width, int height, int margin)
        {
            var rect = new Rect(margin, margin, width - margin * 2, height - margin * 2);

            var group = new DrawingGroup();
            RenderOptions.SetBitmapScalingMode(group, BitmapScalingMode.HighQuality);
            group.Children.Add(new ImageDrawing(source, rect));

            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
                drawingContext.DrawDrawing(group);

            var resizedImage = new RenderTargetBitmap(
                width, height,         // Resized dimensions
                96, 96,                // Default DPI values
                PixelFormats.Default); // Default pixel format
            resizedImage.Render(drawingVisual);

            return BitmapFrame.Create(resizedImage);
        }

        public static Bitmap ConvertToBitmap([NotNull] BitmapSource bitmapSource)
        {
            var width = bitmapSource.PixelWidth;
            var height = bitmapSource.PixelHeight;
            var stride = width * ((bitmapSource.Format.BitsPerPixel + 7) / 8);
            var memoryBlockPointer = Marshal.AllocHGlobal(height * stride);
            bitmapSource.CopyPixels(new Int32Rect(0, 0, width, height), memoryBlockPointer, height * stride, stride);
            var bitmap = new Bitmap(width, height, stride, PixelFormat.Format32bppPArgb, memoryBlockPointer);
            
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var pixel = bitmap.GetPixel(x, y);
                    var color = pixel.GetBrightness() <= 0.5f ? Color.White : Color.Black;
                    bitmap.SetPixel(x, y, color);
                }
            }


            return bitmap;
        }

        private (Bitmap bitmap, int[] hash) CreateBlackAndWhite(Bitmap bitmap)
        {
            var width = bitmap.Width;
            var height = bitmap.Height;

            var result = bitmap.Clone(new Rectangle(0, 0, width, height), bitmap.PixelFormat);
            var hash = new int[width * height];
            var hashIndex = 0;
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var pixel = bitmap.GetPixel(x, y);
                    var hashNum = pixel.GetBrightness() <= 0.5f ? 0 : 1;
                    hash[hashIndex++] = hashNum;
                    var color = pixel.GetBrightness() <= 0.5f ? Color.White : Color.Black;
                    result.SetPixel(x, y, color);
                }

            }

            return (result, hash);
        }

        public BitmapImage ConvertToImageSource([NotNull] Bitmap src)
        {
            var ms = new MemoryStream();
            src.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            var image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }

        private void ExecutePlayAgainCommand()
        {
            _random = new Random();
            _ = StartSendReplayAsync();
        }

        private async Task StartSendReplayAsync()
        {
            var delayTime = _random.Next(5000, 10000);

            await Task.Run(SendReplay).ConfigureAwait(false);
            await Task.Delay(delayTime).ConfigureAwait(false);
            await StartSendReplayAsync().ConfigureAwait(false);
        }

        public static void SendReplay()
        {

            var client = SharpAdbClient.AdbClient.Instance;
            var device = client.GetDevices()[0];

            var receiver = new DebugReceiver();
            var commands = new List<string>()
            {
                "sendevent /dev/input/event8 3 53 16336",
                "sendevent /dev/input/event8 3 54 29875",
                "sendevent /dev/input/event8 3 48 5",
                "sendevent /dev/input/event8 3 58 50",
                "sendevent /dev/input/event8 0 2 0",
                "sendevent /dev/input/event8 0 0 0",

                "sendevent /dev/input/event8 3 53 16336",
                "sendevent /dev/input/event8 3 54 29875",
                "sendevent /dev/input/event8 3 48 5",
                "sendevent /dev/input/event8 3 58 50",
                "sendevent /dev/input/event8 0 2 0",
                "sendevent /dev/input/event8 0 0 0",

                "sendevent /dev/input/event8 0 2 0",
                "sendevent /dev/input/event8 0 0 0",
            };

            foreach (var cmd in commands)
            {
                client.ExecuteRemoteCommand(cmd, device, receiver);
            }
        }

    }
}
