using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;

namespace Calendo.Data
{
    [Serializable]
    [XmlRoot("Data")]
    public class State<T> where T : new()
    {
        private T baseValue;
        private static Stack<T> redoStack = new Stack<T>();

        /// <summary>
        /// Create a State object to represent object states
        /// </summary>
        public State()
        {
            baseValue = new T();
            States = new List<T>();
        }

        /// <summary>
        /// Current State
        /// </summary>
        public T Value
        {
            get { return baseValue; }
            set { baseValue = value; }
        }

        /// <summary>
        /// List of States
        /// </summary>
        [XmlArrayItem("State")]
        public List<T> States
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
                States.Add(new T());
            }
            States.Add(PerformClone(baseValue));
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
                // First state does not count
                redoStack.Push(States[States.Count - 1]);
                States.RemoveAt(States.Count - 1);
                baseValue = PerformClone(States[States.Count - 1]);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a boolean value determining if there are undo states
        /// </summary>
        public bool HasUndo
        {
            get
            {
                // First state does not count
                return (States.Count > 1);
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
                baseValue = PerformClone(redoStack.Pop());
                States.Add(baseValue);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a boolean value determining if there are redo states
        /// </summary>
        public bool HasRedo
        {
            get
            {
                return (redoStack.Count > 0);
            }
        }

        /// <summary>
        /// Clones the object
        /// </summary>
        /// <param name="obj">Provided object</param>
        /// <returns>Deep copy of the object</returns>
        private T PerformClone(T obj)
        {
            Debug.Assert(typeof(T).IsSerializable, "Object is not serializable!");
            MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.Clone));
            formatter.Serialize(memoryStream, obj);
            memoryStream.Seek(0, SeekOrigin.Begin);
            T clone = (T)formatter.Deserialize(memoryStream);
            return clone;
        }
    }

    public class StateStorage<T> : Storage<T> where T : new()
    {
        private Storage<State<T>> dataStorage;
        private State<T> currentState;

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
        }

        /// <summary>
        /// Data stored in storage
        /// </summary>
        public override T Entries
        {
            get { return dataStorage.Entries.Value; }
            set { dataStorage.Entries.Value = value; }
        }

        /// <summary>
        /// Saves the data
        /// </summary>
        /// <returns>Returns true if file has been changed</returns>
        public override bool Save()
        {
            dataStorage.Entries.AddState();
            currentState = dataStorage.Entries;
            return dataStorage.Save();
        }

        /// <summary>
        /// Loads the data
        /// </summary>
        /// <returns>Returns true if the file has been read</returns>
        public override bool Load()
        {
            bool loadResult = dataStorage.Load();
            currentState = dataStorage.Entries;
            return loadResult;
        }

        /// <summary>
        /// Gets a boolean value determining if there are undo states
        /// </summary>
        public bool HasUndo
        {
            get
            {
                return (currentState.HasUndo);
            }
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
        /// Gets a boolean value determining if there are redo states
        /// </summary>
        public bool HasRedo
        {
            get
            {
                return (currentState.HasRedo);
            }
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
