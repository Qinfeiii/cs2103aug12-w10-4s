using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace Calendo
{
    /// <summary>
    /// Javascript Object Notation Parser
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
    }
}
