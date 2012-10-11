using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Calendo.Data;

namespace Calendo.Converters
{
    class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Entry)
            {
                Entry current = (Entry)value;

                DateTime returnDate = (string)parameter == "StartDate" ? current.StartTime : current.EndTime;

                switch (current.StartTimeFormat)
                {
                    case TimeFormat.NONE:
                        break;
                    case TimeFormat.TIME:
                        return returnDate.ToString("t");
                    case TimeFormat.DATETIME:
                        return returnDate.ToString("g");
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
