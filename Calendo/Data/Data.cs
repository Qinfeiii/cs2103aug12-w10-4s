//@author A0080933E
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Calendo.Data
{
    /// <summary>
    /// Wrapper class for object used in serialization
    /// </summary>
    /// <typeparam name="T">Generic type to wrap</typeparam>
    [Serializable]
    [XmlRoot("Data")]
    public class Data<T> where T : new()
    {
        /// <summary>
        /// Creates a new Data object
        /// </summary>
        public Data()
        {
            Value = new T();
        }

        /// <summary>
        /// Gets or sets the value represented by the object
        /// </summary>
        [XmlElement("Entry")]
        public virtual T Value
        {
            get;
            set;
        }
    }
}
