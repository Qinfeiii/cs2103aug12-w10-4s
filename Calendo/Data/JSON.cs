using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace Calendo.Data
{
    /// <summary>
    /// JavaScript Object Notation (JSON) Parser
    /// </summary>
    /// <typeparam name="T">Type to convert to and from JSON</typeparam>
    public class JSON<T>
    {
        /// <summary>
        /// Serialize an object to JSON
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string Serialize(T obj) {
            var jss = new JavaScriptSerializer();
            var json = jss.Serialize(obj);
            return json.ToString();
        }

        /// <summary>
        /// Deserialize an object from JSON
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public T Deserialize(string json)
        {
            var jss = new JavaScriptSerializer();
            var dict = jss.Deserialize<T>(json);
            return dict;
        }

        /// <summary>
        /// Convert DateTime to Google date format
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public string DateToJSON(DateTime date)
        {
            // NOTE: JSON do not have a standardized format. Google API uses RFC 3339.
            DateTime utcDate = date.ToUniversalTime();
            // Format: [Date]T[Time].[Time zone], Time zone assumed to be UTC for simplicity
            return utcDate.ToString("yyyy-MM-dd") + "T" + utcDate.ToString("HH:mm:ss") + ".000Z";
        }

        /// <summary>
        /// Convert Google date format to UTC DateTime
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public DateTime JSONToDate(string json)
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
    }
}
