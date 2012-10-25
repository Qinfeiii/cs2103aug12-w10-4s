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
                if (IsTaskOngoing(currentEntry))
                {
                    return Brushes.Orange;
                }
                else if (IsTaskOverdue(currentEntry))
                {
                    return Brushes.Red;
                }
            }

            BrushConverter converter = new BrushConverter();
            return converter.ConvertFrom("#FF464646") as Brush;
        }

        public bool IsTaskOverdue(Entry currentEntry)
        {
            bool isOverdue = false;

            if (currentEntry.Type == EntryType.TIMED)
            {
                isOverdue = currentEntry.EndTime.CompareTo(DateTime.Now) < 0;
            }
            else if (currentEntry.Type == EntryType.DEADLINE)
            {
                isOverdue = currentEntry.StartTime.CompareTo(DateTime.Now) < 0;
            }

            // Floating tasks can't be overdue.
            return isOverdue;
        }

        public bool IsTaskOngoing(Entry currentEntry)
        {
            bool isOngoing = false;

            TimeSpan nowAndTaskStartDifference = currentEntry.StartTime.Subtract(DateTime.Now);
            bool isTaskStarting = 0 < nowAndTaskStartDifference.TotalHours && nowAndTaskStartDifference.TotalHours < 24;

            if (currentEntry.Type == EntryType.TIMED)
            {
                bool isNowBetweenStartAndEnd = currentEntry.StartTime.CompareTo(DateTime.Now) < 0 &&
                                               DateTime.Now.CompareTo(currentEntry.EndTime) < 0;
                isOngoing = isTaskStarting || isNowBetweenStartAndEnd;
            }
            else if (currentEntry.Type == EntryType.DEADLINE)
            {
                isOngoing = isTaskStarting;
            }

            // Floating tasks can't be ongoing.
            return isOngoing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}