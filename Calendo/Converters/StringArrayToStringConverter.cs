using System;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace Calendo.Converters
{
    class StringArrayToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string[] array = value as string[];
            StringBuilder outputBuilder = new StringBuilder();
            if(array != null)
            {
                foreach(string current in array)
                {
                    outputBuilder.Append(current + ", ");
                }
                outputBuilder.Remove(outputBuilder.Length - 2, 2);
                return outputBuilder.ToString();
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}