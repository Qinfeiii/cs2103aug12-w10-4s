using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace Calendo
{
    class JSON<T>
    {
        public string Serialize(T obj) {
            var jss = new JavaScriptSerializer();
            var json = jss.Serialize(obj);
            return json.ToString();
        }
        public T Deserialize(string json)
        {
            var jss = new JavaScriptSerializer();
            var dict = jss.Deserialize<T>(json);
            return dict;
        }
    }
}
