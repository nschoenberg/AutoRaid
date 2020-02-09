using System.Text;
using AutoRaid.ViewModels;
using SharpAdbClient;
using Xunit;
using Xunit.Abstractions;

namespace AutoRaid.Tests.ViewModels
{
    public class MainWindowViewModelTests
    {
        private readonly OutputReceiver _receiver;
        private readonly ITestOutputHelper _testOutputHelper;

        private class OutputReceiver : IShellOutputReceiver
        {

            private readonly StringBuilder _stringBuilder = new StringBuilder();
            private readonly ITestOutputHelper _outputHelper;

            public bool ParsesErrors => true;

            public OutputReceiver(ITestOutputHelper outputHelper)
            {
                _outputHelper = outputHelper;
            }

            public void AddOutput(string line)
            {
                _stringBuilder.AppendLine(line);
            }

            public void Flush()
            {
                _outputHelper.WriteLine(_stringBuilder.ToString());
                _stringBuilder.Clear();
            }
        }

        public MainWindowViewModelTests(Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _receiver = new OutputReceiver(testOutputHelper);
        }

        [Fact]
        public void GetDevices_And_Run_Shell_Command()
        {
            MainWindowViewModel.SendReplay();
        }
    }
}
