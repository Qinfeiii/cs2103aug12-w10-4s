//@author A0080860H
using System;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace Calendo.Converters
{
    public class StringArrayToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string[] array = value as string[];
            StringBuilder outputBuilder = new StringBuilder();
            if (array != null && array.Length > 1)
            {
                for (int i = 1; i < array.Length; i++)
                {
                    string current = array[i];
                    outputBuilder.Append(current + ", ");
                }
                outputBuilder.Remove(outputBuilder.Length - 2, 2);
                return outputBuilder.ToString();
            }
            return "none";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}