//@author A0080860H
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Calendo.Logic;

namespace Calendo.Converters
{
    public class EntryToDateTimeVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var current = value as Entry;
            if (current != null)
            {
                var parameterString = parameter as string;
                if (parameterString == "StackPanel")
                {
                    if (current.StartTimeFormat != TimeFormat.None || current.EndTimeFormat != TimeFormat.None)
                    {
                        return Visibility.Visible;
                    }
                }
                else if (parameterString == "RangeText")
                {
                    if (current.EndTimeFormat != TimeFormat.None)
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
