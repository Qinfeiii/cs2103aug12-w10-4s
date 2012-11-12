//@author A0080933E
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Diagnostics;
using System.IO;
using Calendo.Diagnostics;

namespace Calendo.Data
{
    /// <summary>
    /// Saves objects into files.
    /// </summary>
    /// <typeparam name="T">Type stored by storage</typeparam>
    public class Storage<T> where T : new()
    {
        private const string DEFAULT_FILE_PATH = "data.txt";
        private const string DEFAULT_BACKUP_PATH = "backup.txt";
        private const string MESSAGE_BACKUP = "\r\nUsing backup file: " + DEFAULT_BACKUP_PATH;
        private const string ERROR_UNWRITABLE = "Unable to write to file";
        private const string ERROR_INCOMPATIBLE = "Data file is unreadable";
        private const string ERROR_UNSERIALIZABLE = "Object is not serializable";

        private bool useBackup;
        private string dataFilePath;
        private object dataObject;
        private XmlSerializer serializer;

        /// <summary>
        /// Creates a Storage object
        /// </summary>
        public Storage()
        {
            dataFilePath = DEFAULT_FILE_PATH;
            useBackup = false;
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
            dataObject = new Data<T>();
            try
            {
                // Initialize XMLserializer to use the Data<T> type template
                serializer = XmlSerializer.FromTypes(new[] { typeof(Data<T>) })[0];
            }
            catch
            {
                DebugTool.Alert(ERROR_UNSERIALIZABLE);
            }
        }

        /// <summary>
        /// Sets the XML Serializer used
        /// </summary>
        protected XmlSerializer Serializer
        {
            set
            {
                serializer = value;
                UseWrapper = false;
            }
        }

        /// <summary>
        /// Indicates whether to use a wrapper
        /// </summary>
        protected bool UseWrapper = true;

        /// <summary>
        /// Data stored in storage
        /// </summary>
        public virtual T Entries
        {
            get
            {
                if (UseWrapper)
                {
                    Data<T> dataWrapper = dataObject as Data<T>;
                    return dataWrapper.Value;
                }
                else
                {
                    T dataWrapper = (T)dataObject;
                    return dataWrapper;
                }
            }
            set
            {
                if (UseWrapper)
                {
                    Data<T> dataWrapper = dataObject as Data<T>;
                    dataWrapper.Value = value;
                }
                else
                {
                    T dataWrapper = (T)dataObject;
                    dataWrapper = value;
                }
            }
        }

        /// <summary>
        /// Saves the data
        /// </summary>
        /// <returns>True if file has been changed</returns>
        public virtual bool Save()
        {
            string currentFilePath = dataFilePath;
            if (useBackup)
            {
                currentFilePath = DEFAULT_BACKUP_PATH;
            }

            Stream fileStream = null;
            try
            {
                fileStream = new FileStream(currentFilePath, FileMode.Create);
                XmlSerializerNamespaces xmlNamespace = new XmlSerializerNamespaces();
                xmlNamespace.Add("", ""); // omit XML namespaces
                serializer.Serialize(fileStream, dataObject, xmlNamespace);
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
        /// <returns>True if the file has been read</returns>
        public virtual bool Load()
        {
            return Load(false);
        }

        /// <summary>
        /// Loads the data
        /// </summary>
        /// <param name="useBackup">Determines whether to use backup path</param>
        /// <returns>True if the file has been read</returns>
        private bool Load(bool useBackup)
        {
            this.useBackup = useBackup;
            string currentFilePath = dataFilePath;

            if (useBackup)
            {
                currentFilePath = DEFAULT_BACKUP_PATH;
            }

            Stream fileStream = null;
            try
            {
                fileStream = new FileStream(currentFilePath, FileMode.OpenOrCreate);
                if (fileStream.Length != 0)
                {
                    dataObject = (Data<T>)serializer.Deserialize(fileStream);
                }
                fileStream.Close();
                return true;
            }
            catch
            {
                // Invalid file, use backup file
                string errorMessage = ERROR_INCOMPATIBLE;
                dataObject = new Data<T>();

                if (currentFilePath != DEFAULT_BACKUP_PATH)
                {
                    errorMessage += MESSAGE_BACKUP;
                    Load(true);
                    DebugTool.Alert(errorMessage);
                }
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
