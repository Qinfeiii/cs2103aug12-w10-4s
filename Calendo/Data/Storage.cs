using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.IO;
using Calendo.DebugTool;

namespace Calendo.Data
{
    [Serializable]
    [XmlRoot("Data")]
    public class Data<T> : ICloneable where T : new()
    {
        public Data() {
            Value = new T();
        }

        [XmlElement("Entry")]
        public T Value
        {
            get;
            set;
        }

        /// <summary>
        /// Returns a copy of this object
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.Clone));
            formatter.Serialize(memoryStream, this);
            memoryStream.Seek(0, SeekOrigin.Begin);
            object clone = formatter.Deserialize(memoryStream);
            return clone;
        }
    }

    /// <summary>
    /// Saves objects into files.
    /// </summary>
    /// <typeparam name="T">Type stored by storage</typeparam>
    public class Storage<T> where T : new()
    {
        private const string DEFAULT_FILE_PATH = "data.txt";
        private const string ERROR_UNWRITABLE = "Unable to write file";
        private const string ERROR_INCOMPATIBLE = "Data file might be corrupted or incompatible";

        private string dataFilePath;
        private Data<T> dataWrapper;
        private XmlSerializer serializer;

        /// <summary>
        /// Creates a Storage object
        /// </summary>
        public Storage()
        {
            dataFilePath = DEFAULT_FILE_PATH;
            Initialize();
        }

        /// <summary>
        /// Creates a Storage object
        /// </summary>
        /// <param name="filePath">File path to data file</param>
        public Storage(string filePath)
        {
            dataFilePath = filePath;
            Initialize();
        }

        /// <summary>
        /// Shared Constructor
        /// </summary>
        private void Initialize()
        {
            dataWrapper = new Data<T>();
            serializer = new XmlSerializer(dataWrapper.GetType());
        }

        /// <summary>
        /// Data stored in storage
        /// </summary>
        public T Entries
        {
            get { return dataWrapper.Value; }
            set { dataWrapper.Value = value; }
        }

        /// <summary>
        /// Saves the data
        /// </summary>
        /// <returns>Returns true if file has been changed</returns>
        public bool Save()
        {
            try
            {
                Stream fileStream = new FileStream(dataFilePath, FileMode.Create);
                XmlSerializerNamespaces xmlNamespace = new XmlSerializerNamespaces();
                xmlNamespace.Add("", ""); // omit XML namespaces
                serializer.Serialize(fileStream, dataWrapper, xmlNamespace);
                fileStream.Close();
                return true;
            }
            catch
            {
                Debug.Alert(ERROR_UNWRITABLE);
                return false;
            }
        }

        /// <summary>
        /// Loads the data
        /// </summary>
        /// <returns>Returns true if the file has been read</returns>
        public bool Load()
        {
            try
            {
                Stream fileStream = new FileStream(dataFilePath, FileMode.OpenOrCreate);
                if (fileStream.Length != 0)
                {
                    dataWrapper = (Data<T>)serializer.Deserialize(fileStream);
                }
                fileStream.Close();
                return true;
            }
            catch
            {
                // Invalid file, recreate empty state
                Debug.Alert(ERROR_INCOMPATIBLE);
                dataWrapper = new Data<T>();
                return false;
            }
        }
    }
}
