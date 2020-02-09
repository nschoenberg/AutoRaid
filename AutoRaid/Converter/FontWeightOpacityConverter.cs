using System;
using System.Globalization;
using System.Windows.Data;

namespace AutoRaid.Converter
{
    public class FontWeightOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int number && number == 1)
            {
                return 0.75;
            }

            return 1.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
