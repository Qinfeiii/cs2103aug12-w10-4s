//@author A0080860H
using System;
using System.Globalization;
using System.Windows.Data;
using Calendo.Logic;

namespace Calendo.Converters
{
    public class EntryToDateTimeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var current = value as Entry;
            if (current != null)
            {
                DateTime returnDate;
                TimeFormat returnTimeFormat;
                if((string) parameter == "StartDate")
                {
                    returnDate = current.StartTime;
                    returnTimeFormat = current.StartTimeFormat;
                } else
                {
                    returnDate = current.EndTime;
                    returnTimeFormat = current.EndTimeFormat;
                }
                switch (returnTimeFormat)
                {
                    case TimeFormat.None:
                        break;
                    case TimeFormat.Time:
                        return returnDate.ToString("HH:mm");
                    case TimeFormat.DateTime:
                        if (returnDate.Date == DateTime.Today)
                        {
                            return "Today " + returnDate.ToString("HH:mm");
                        }
                        else if (returnDate.Date == DateTime.Today.AddDays(1))
                        {
                            return "Tomorrow " + returnDate.ToString("HH:mm");
                        }
                        else
                        {
                            return returnDate.ToString("dd/MM/yyyy HH:mm");
                        }
                    case TimeFormat.Date:
                        if (returnDate.Date == DateTime.Today)
                        {
                            return "Today";
                        }
                        else if (returnDate.Date == DateTime.Today.AddDays(1))
                        {
                            return "Tomorrow";
                        }
                        else
                        {
                            return returnDate.ToString("dd/MM/yyyy");
                        }
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
