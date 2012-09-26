using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace Calendo
{
    public interface IStorage
    {
        List<Entry> Entries
        {
            get;
            set;
        }
        string FilePath
        {
            get;
        }
        bool Save();
        bool Load();
    }

    /// <summary>
    /// Used for wrapping list-based data
    /// </summary>
    public class DataWrapper
    {
        private List<Entry> _entries;
        /// <summary>
        /// Gets or sets the list of entries
        /// </summary>
        public List<Entry> Entries
        {
            get { return _entries; }
            set { _entries = value; }
        }
    }

    /// <summary>
    /// Stores and retrieve entries
    /// </summary>
    class DataStorage : IStorage
    {
        private const string DEFAULT_FILE_PATH = "data.txt";
        private DataWrapper data;
        private string _DataFilePath;
        private XmlSerializer _Serializer;

        /// <summary>
        /// Creates a new DataStorage object
        /// </summary>
        public DataStorage()
        {
            _DataFilePath = DEFAULT_FILE_PATH;
            init();
        }

        /// <summary>
        /// Creates a new DataStorage object
        /// </summary>
        /// <param name="FilePath">Path to database file</param>
        public DataStorage(string FilePath)
        {
            _DataFilePath = FilePath;
            init();
        }

        private void init()
        {
            //_Entries = new List<Entry>();
            data = new DataWrapper();
            data.Entries = new List<Entry>();
                            _Serializer = new XmlSerializer(data.GetType());
        }

        /// <summary>
        /// Gets or sets the list of entries
        /// </summary>
        public List<Entry> Entries
        {
            get { return data.Entries; }
            set { data.Entries = value; }
        }

        /// <summary>
        /// Gets the file path used
        /// </summary>
        public string FilePath
        {
            get { return _DataFilePath; }
        }

        /// <summary>
        /// Save to database
        /// </summary>
        /// <returns>Returns true on success</returns>
        public bool Save()
        {
            try
            {
                // Override old file completely
                Stream s = new FileStream(_DataFilePath, FileMode.Create);
                _Serializer.Serialize(s, data);
                s.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Load from database
        /// </summary>
        /// <returns>Returns true on success</returns>
        public bool Load()
        {
            try
            {
                // Open or create if not exist
                Stream s = new FileStream(_DataFilePath, FileMode.OpenOrCreate);
                if (s.Length != 0)
                {
                    data = (DataWrapper)_Serializer.Deserialize(s);
                }
                s.Close();
                return true;
            }
            catch
            {
                data.Entries = new List<Entry>();
                return false;
            }
        }
    }
}
