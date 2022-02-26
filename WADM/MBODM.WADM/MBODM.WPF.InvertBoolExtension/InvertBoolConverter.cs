using System;
using System.Globalization;
using System.Windows.Data;

namespace MBODM.WPF
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public sealed class InvertBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (value is bool == false)
            {
                throw new ArgumentException("Parameter is not a bool.", "value");
            }

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
}
