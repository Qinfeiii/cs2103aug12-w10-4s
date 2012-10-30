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
        private static Stack<T> RedoStack = new Stack<T>();

        /// <summary>
        /// Create a State object to represent object states
        /// </summary>
        public State()
        {
            this.Value = new T();
            this.States = new List<T>();
        }

        /// <summary>
        /// Current State
        /// </summary>
        public T Value
        {
            get;
            set;
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
            if (this.States.Count == 0)
            {
                this.States.Add(new T());
            }
            this.States.Add(PerformClone(Value));
            RedoStack.Clear();
        }

        /// <summary>
        /// Revert to state before last state
        /// </summary>
        /// <returns>Return true if state is changed</returns>
        public bool Undo()
        {
            if (this.States.Count > 1)
            {
                // First state does not count
                RedoStack.Push(this.States[States.Count - 1]);
                this.States.RemoveAt(this.States.Count - 1);
                this.Value = PerformClone(this.States[States.Count - 1]);
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
                return (this.States.Count > 1);
            }
        }

        /// <summary>
        /// Revert to state in redo stack
        /// </summary>
        /// <returns>Return true if state is changed</returns>
        public bool Redo()
        {
            if (RedoStack.Count > 0)
            {
                this.Value = PerformClone(RedoStack.Pop());
                this.States.Add(Value);
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
                return (RedoStack.Count > 0);
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
            this.dataStorage = new Storage<State<T>>();
            this.Initialize();
        }

        /// <summary>
        /// Creates a Storage object
        /// </summary>
        /// <param name="filePath">File path to data file</param>
        public StateStorage(string filePath)
        {
            this.dataStorage = new Storage<State<T>>(filePath);
            this.Initialize();
        }

        /// <summary>
        /// Shared Constructor
        /// </summary>
        private void Initialize()
        {
            this.currentState = new State<T>();
        }

        /// <summary>
        /// Data stored in storage
        /// </summary>
        public override T Entries
        {
            get { return this.dataStorage.Entries.Value; }
            set { this.dataStorage.Entries.Value = value; }
        }

        /// <summary>
        /// Saves the data
        /// </summary>
        /// <returns>Returns true if file has been changed</returns>
        public override bool Save()
        {
            this.dataStorage.Entries.AddState();
            this.currentState = this.dataStorage.Entries;
            return this.dataStorage.Save();
        }

        /// <summary>
        /// Loads the data
        /// </summary>
        /// <returns>Returns true if the file has been read</returns>
        public override bool Load()
        {
            bool loadResult = this.dataStorage.Load();
            this.currentState = this.dataStorage.Entries;
            return loadResult;
        }

        /// <summary>
        /// Gets a boolean value determining if there are undo states
        /// </summary>
        public bool HasUndo
        {
            get
            {
                return (this.currentState.HasUndo);
            }
        }

        /// <summary>
        /// Revert to a previous state
        /// </summary>
        /// <returns>Returns true if a state is reverted, false if no action taken</returns>
        public bool Undo()
        {
            bool undoResult = currentState.Undo();
            this.dataStorage.Save();
            return undoResult;
        }

        /// <summary>
        /// Gets a boolean value determining if there are redo states
        /// </summary>
        public bool HasRedo
        {
            get
            {
                return (this.currentState.HasRedo);
            }
        }

        /// <summary>
        /// Undo changes done by undo command
        /// </summary>
        /// <returns>Returns true if a state is reverted, false if no action taken</returns>
        public bool Redo()
        {
            bool redoResult = this.currentState.Redo();
            this.dataStorage.Save();
            return redoResult;
        }

        /// <summary>
        /// Reset number of undos to zero
        /// </summary>
        public void Clear()
        {
            this.dataStorage.Entries.States.Clear();
            this.dataStorage.Save();
            this.dataStorage.Load();
        }
    }
}
