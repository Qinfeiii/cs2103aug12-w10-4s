using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace Calendo.Data
{
    [XmlRoot("Data")]
    public class State<T> where T : new()
    {
        private Data<T> baseValue;
        private Stack<Data<T>> redoStack;

        /// <summary>
        /// Create a State object to represent object states
        /// </summary>
        public State()
        {
            baseValue = new Data<T>();
            redoStack = new Stack<Data<T>>();
            States = new List<Data<T>>();
        }

        /// <summary>
        /// Current State
        /// </summary>
        public T Value
        {
            get { return baseValue.Value; }
            set { baseValue.Value = value; }
        }

        /// <summary>
        /// List of States
        /// </summary>
        [XmlArrayItem("State")]
        public List<Data<T>> States
        {
            get;
            set;
        }

        /// <summary>
        /// Adds a state
        /// </summary>
        public void AddState()
        {
            if (States.Count == 0)
            {
                States.Add(new Data<T>());
            }
            States.Add((Data<T>)baseValue.Clone());
            redoStack.Clear();
        }

        /// <summary>
        /// Revert to state before last state
        /// </summary>
        /// <returns>Return true if state is changed</returns>
        public bool Undo()
        {
            if (States.Count > 1)
            {
                redoStack.Push(States[States.Count - 1]);
                States.RemoveAt(States.Count - 1);
                baseValue = (Data<T>)States[States.Count - 1].Clone();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Revert to state in redo stack
        /// </summary>
        /// <returns>Return true if state is changed</returns>
        public bool Redo()
        {
            if (redoStack.Count > 0)
            {
                baseValue = (Data<T>)redoStack.Pop().Clone();
                States.Add(baseValue);
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class StateStorage<T> where T : new()
    {
        private Storage<State<T>> dataStorage;
        private State<T> currentState;
        private XmlSerializer serializer;

        /// <summary>
        /// Creates a Storage object
        /// </summary>
        public StateStorage()
        {
            dataStorage = new Storage<State<T>>();
        }
        
        /// <summary>
        /// Creates a Storage object
        /// </summary>
        /// <param name="filePath">File path to data file</param>
        public StateStorage(string filePath)
        {
            dataStorage = new Storage<State<T>>(filePath);
            Initialize();
        }

        /// <summary>
        /// Shared Constructor
        /// </summary>
        private void Initialize()
        {
            currentState = new State<T>();
            serializer = new XmlSerializer(currentState.GetType());
        }

        /// <summary>
        /// Data stored in storage
        /// </summary>
        public T Entries
        {
            get { return dataStorage.Entries.Value; }
            set { dataStorage.Entries.Value = value; }
        }

        /// <summary>
        /// Saves the data
        /// </summary>
        /// <returns>Returns true if file has been changed</returns>
        public bool Save()
        {
            dataStorage.Entries.AddState();
            return dataStorage.Save();
        }

        /// <summary>
        /// Loads the data
        /// </summary>
        /// <returns>Returns true if the file has been read</returns>
        public bool Load()
        {
            bool loadResult = dataStorage.Load();
            currentState = dataStorage.Entries;
            return loadResult;
        }

        /// <summary>
        /// Revert to a previous state
        /// </summary>
        /// <returns>Returns true if a state is reverted, false if no action taken</returns>
        public bool Undo()
        {
            bool undoResult = currentState.Undo();
            dataStorage.Save();
            return undoResult;
        }

        /// <summary>
        /// Undo changes done by undo command
        /// </summary>
        /// <returns>Returns true if a state is reverted, false if no action taken</returns>
        public bool Redo()
        {
            bool redoResult = currentState.Redo();
            dataStorage.Save();
            return redoResult;
        }

        /// <summary>
        /// Remove all state information
        /// </summary>
        public void Clear()
        {
            dataStorage.Entries.States.Clear();
            dataStorage.Save();
            dataStorage.Load();
        }
    }
}
