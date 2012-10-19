using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Diagnostics;
using System.IO;
using Calendo.Diagnostics;

namespace Calendo.Data
{
    [Serializable]
    [XmlRoot("Data")]
    public class Data<T> where T : new()
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
    }

    /// <summary>
    /// Saves objects into files.
    /// </summary>
    /// <typeparam name="T">Type stored by storage</typeparam>
    public class Storage<T> where T : new()
    {
        private const string DEFAULT_FILE_PATH = "data.txt";
        private const string ERROR_UNWRITABLE = "Unable to write file";
        private const string ERROR_INCOMPATIBLE = "Data file is unreadable";

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
            Stream fileStream = null;
            try
            {
                fileStream = new FileStream(dataFilePath, FileMode.Create);
                XmlSerializerNamespaces xmlNamespace = new XmlSerializerNamespaces();
                xmlNamespace.Add("", ""); // omit XML namespaces
                serializer.Serialize(fileStream, dataWrapper, xmlNamespace);
                fileStream.Close();
                return true;
            }
            catch
            {
                DebugTool.Alert(ERROR_UNWRITABLE);
                return false;
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }

        /// <summary>
        /// Loads the data
        /// </summary>
        /// <returns>Returns true if the file has been read</returns>
        public bool Load()
        {
            Stream fileStream = null;
            try
            {
                fileStream = new FileStream(dataFilePath, FileMode.OpenOrCreate);
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
                DebugTool.Alert(ERROR_INCOMPATIBLE);
                dataWrapper = new Data<T>();
                return false;
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }
    }
}
