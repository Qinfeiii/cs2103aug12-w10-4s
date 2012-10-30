//@author Nicholas
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Script.Serialization;

namespace Calendo.GoogleCalendar
{
    /// <summary>
    /// JavaScript Object Notation (JSON) Parser
    /// </summary>
    /// <typeparam name="T">Type to convert to and from JSON</typeparam>
    public class JSON<T>
    {
        private JavaScriptSerializer jsSerializer;

        /// <summary>
        /// Create a new instance of the JSON parser
        /// </summary>
        public JSON()
        {
            jsSerializer = new JavaScriptSerializer();
        }

        /// <summary>
        /// Serialize an object to JSON
        /// </summary>
        /// <param name="obj">Object to convert</param>
        /// <returns>JSON string representation</returns>
        public string Serialize(T obj) {
            string json = jsSerializer.Serialize(obj);
            return json;
        }

        /// <summary>
        /// Deserialize an object from JSON
        /// </summary>
        /// <param name="json">JSON string representation</param>
        /// <returns>Converted object</returns>
        public T Deserialize(string json)
        {
            T jsonObject = jsSerializer.Deserialize<T>(json);
            return jsonObject;
        }

        /// <summary>
        /// Convert DateTime to Google date format
        /// </summary>
        /// <param name="date">DateTime to convert</param>
        /// <returns>JSON representation of DateTime object</returns>
        public string DateToJSON(DateTime date)
        {
            // NOTE: JSON do not have a standardized format. Google API uses RFC 3339.
            DateTime utcDate = date.ToUniversalTime();
            return utcDate.ToString("yyyy-MM-dd") + "T" + utcDate.ToString("HH:mm:ss") + ".000Z";
        }

        /// <summary>
        /// Convert Google date format to UTC DateTime
        /// </summary>
        /// <param name="json">JSON representation of DateTime object</param>
        /// <returns>DateTime representing object, if invalid format returns current time</returns>
        public DateTime JSONToDate(string json)
        {
            try
            {
                string[] datetimeParts = json.Split(new char[] { 'T', '.' }, 3);
                string[] dateParts = datetimeParts[0].Split(new char[] { '-' });
                string[] timeParts = datetimeParts[1].Split(new char[] { ':' });
                int year = int.Parse(dateParts[0]);
                int month = int.Parse(dateParts[1]);
                int day = int.Parse(dateParts[2]);
                int hour = int.Parse(timeParts[0]);
                int minute = int.Parse(timeParts[1]);
                int second = int.Parse(timeParts[2]);
                DateTime utcDate = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);
                return utcDate;
            }
            catch
            {
                // Invalid format, return smallest date time
                return new DateTime(0);
            }
        }
    }
}
