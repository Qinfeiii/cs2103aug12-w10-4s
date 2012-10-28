using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Calendo.Data;

namespace Calendo.Converters
{
    public class EntryToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Entry currentEntry = value as Entry;
            if (currentEntry != null)
            {
                if (UiTaskHelper.IsTaskOngoing(currentEntry))
                {
                    return Brushes.Orange;
                }
                else if (UiTaskHelper.IsTaskOverdue(currentEntry))
                {
                    return Brushes.Red;
                }
            }

            BrushConverter converter = new BrushConverter();
            return converter.ConvertFrom("#FF464646") as Brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}