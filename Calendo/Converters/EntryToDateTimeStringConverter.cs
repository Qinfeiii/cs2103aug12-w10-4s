using System;
using System.Globalization;
using System.Windows.Data;
using Calendo.Data;

namespace Calendo.Converters
{
    class EntryToDateTimeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var current = value as Entry;
            if (current != null)
            {
                DateTime returnDate = (string)parameter == "StartDate" ? current.StartTime : current.EndTime;

                switch (current.StartTimeFormat)
                {
                    case TimeFormat.NONE:
                        break;
                    case TimeFormat.TIME:
                        return returnDate.ToString("HH:MM");
                    case TimeFormat.DATETIME:
                        return returnDate.ToString("dd/MM/yyyy HH:MM");
                    case TimeFormat.DATE:
                        return returnDate.ToString("dd/MM/yyyy");
                }
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
