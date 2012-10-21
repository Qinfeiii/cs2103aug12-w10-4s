using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Calendo.Data;

namespace Calendo.Converters
{
    class EntryToDateTimeVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var current = value as Entry;
            if (current != null)
            {
                var parameterString = parameter as string;
                if (parameterString == "StackPanel")
                {
                    if (current.StartTimeFormat != TimeFormat.NONE || current.EndTimeFormat != TimeFormat.NONE)
                    {
                        return Visibility.Visible;
                    }
                }
                else if (parameterString == "RangeText")
                {
                    if (current.EndTimeFormat != TimeFormat.NONE)
                    {
                        return Visibility.Visible;
                    }
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
