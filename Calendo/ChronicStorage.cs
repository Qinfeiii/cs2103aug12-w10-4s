using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;


namespace Calendo
{
    class ChronicStorage : IStorage
    {
        private const string DEFAULT_FILE_PATH = "archive.txt";
        private string _DataFilePath;
        private List<DataWrapper> states;
        private Stack<DataWrapper> redoStates;
        private DataWrapper data;
        private XmlSerializer _Serializer;

        /// <summary>
        /// Creates a new DataStorage object
        /// </summary>
        public ChronicStorage()
        {
            _DataFilePath = DEFAULT_FILE_PATH;
            Init();
        }

        /// <summary>
        /// Creates a new DataStorage object
        /// </summary>
        /// <param name="FilePath">Path to database file</param>
        public ChronicStorage(string FilePath)
        {
            _DataFilePath = FilePath;
            Init();
        }

        /// <summary>
        /// Initializes variables shared by constructors
        /// </summary>
        private void Init()
        {
            //_Entries = new List<Entry>();
            states = new List<DataWrapper>();
            redoStates = new Stack<DataWrapper>();
            data = null;
            _Serializer = new XmlSerializer(states.GetType());
        }

        /// <summary>
        /// Gets or sets the list of entries
        /// </summary>
        public List<Entry> Entries
        {
            get
            {
                if (states.Count > 0)
                {
                    return data.Entries;
                }
                else
                {
                    if (data == null)
                    {
                        data = new DataWrapper();
                        data.Entries = new List<Entry>();
                    }
                    return data.Entries;
                }
            }
            set
            {
                data.Entries = value;
            }
        }

        /// <summary>
        /// Gets the file path used
        /// </summary>
        public string FilePath
        {
            get { return _DataFilePath; }
        }

        /// <summary>
        /// Revert to a previous state
        /// </summary>
        /// <returns>Returns true if a state is reverted, false if no action taken</returns>
        public bool Undo()
        {
            if (states.Count > 0)
            {
                redoStates.Push(states[states.Count - 1]);
                states.RemoveAt(states.Count - 1);
                return SaveWithoutAdd();
            }
            return false;
        }

        /// <summary>
        /// Gets the current undo state
        /// </summary>
        public bool HasUndo
        {
            get
            {
                // Exclude initial state
                if (states.Count > 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Undo changes done by undo command
        /// </summary>
        /// <returns>Returns true if a state is reverted, false if no action taken</returns>
        public bool Redo()
        {
            if (redoStates.Count > 0)
            {
                states.Add(redoStates.Pop());
                return SaveWithoutAdd();
            }
            return false;
        }

        /// <summary>
        /// Gets the current undo state
        /// </summary>
        public bool HasRedo
        {
            get
            {
                if (redoStates.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Removes all state records
        /// </summary>
        public void Clear()
        {
            states.Clear();
            Stream s = new FileStream(_DataFilePath, FileMode.Create);
            _Serializer.Serialize(s, states);
            s.Close();
        }

        /// <summary>
        /// Saves without adding a state record
        /// </summary>
        /// <returns>Returns true on success</returns>
        private bool SaveWithoutAdd()
        {
            try
            {
                data = states[states.Count - 1].Clone();
                // Override old file completely

                Stream s = new FileStream(_DataFilePath, FileMode.Create);
                _Serializer.Serialize(s, states);
                s.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Save the database
        /// </summary>
        /// <returns>Returns true on success</returns>
        public bool Save()
        {
            try
            {
                if (states.Count == 0)
                {
                    // Add the initial state
                    DataWrapper dw = new DataWrapper();
                    dw.Entries = new List<Entry>();
                    states.Add(dw);
                }
                // Add the current state
                states.Add(data);
                return SaveWithoutAdd();
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
                    states = (List<DataWrapper>)_Serializer.Deserialize(s);
                }
                s.Close();
                if (states.Count > 0)
                {
                    data = states[states.Count - 1].Clone();
                }
                else
                {
                    data = null;
                }
                return true;
            }
            catch
            {
                states = new List<DataWrapper>();
                data = null;
                return false;
            }
        }
    }
}
