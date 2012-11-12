//@author A0080933E
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
    /// <summary>
    /// Represents a state
    /// </summary>
    /// <typeparam name="T">Serializable generic type</typeparam>
    [Serializable]
    [XmlRoot("Data")]
    public class State<T> where T : new()
    {
        private const string ERROR_UNSERIALIZABLE = "Object is not serializable";
        private const int INVALID_INDEX = -1;
        private static Stack<T> RedoStack = new Stack<T>();
        private int undoOffset = INVALID_INDEX;
        private bool hasUndo = true;
        private List<T> stateList;

        /// <summary>
        /// Create a State object to represent object states
        /// </summary>
        public State()
        {
            this.Value = new T();
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
            get
            {
                // Recreate state list if it was null
                if (stateList == null)
                {
                    this.States = new List<T>();
                }
                // There must be at least one item in state list
                if (stateList.Count == 0)
                {
                    stateList.Add(new T());
                }
                return stateList;
            }
            set
            {
                stateList = value;
            }
        }

        /// <summary>
        /// Adds a state
        /// </summary>
        public void AddState(bool resetRedo = true)
        {
            this.States.Add(PerformClone(Value));
            if (resetRedo)
            {
                RedoStack.Clear();
            }
            undoOffset = INVALID_INDEX;
            hasUndo = true;
        }

        /// <summary>
        /// Revert to state before last state
        /// </summary>
        /// <returns>True if state is changed</returns>
        public bool Undo()
        {
            if (this.States.Count > 1 && HasUndo)
            {
                // Update current undo offset
                if (undoOffset == INVALID_INDEX)
                {
                    undoOffset = this.States.Count - 1;
                }
                undoOffset--;

                if (undoOffset >= 0)
                {
                    // Push current state
                    RedoStack.Push(this.States[this.States.Count - 1]);
                    T undoState = PerformClone(this.States[undoOffset]);
                    this.States.Add(undoState);
                    this.Value = PerformClone(undoState);
                    if (undoOffset == 0)
                    {
                        // Cannot undo anymore
                        hasUndo = false;
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets a boolean value determining if there are undo states
        /// </summary>
        public bool HasUndo
        {
            get
            {
                return (this.States.Count > 1) && hasUndo;
            }
        }

        /// <summary>
        /// Revert to state in redo stack
        /// </summary>
        /// <returns>True if state is changed</returns>
        public bool Redo()
        {
            if (RedoStack.Count > 0)
            {
                this.Value = PerformClone(RedoStack.Pop());
                this.AddState(false);
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
        /// <param name="obj">Object to clone</param>
        /// <returns>Deep copy of the object</returns>
        private T PerformClone(T obj)
        {
            Debug.Assert(typeof(T).IsSerializable, ERROR_UNSERIALIZABLE);
            MemoryStream memoryStream = new MemoryStream();
            StreamingContext streamingContext = new StreamingContext(StreamingContextStates.Clone);
            BinaryFormatter formatter = new BinaryFormatter(null, streamingContext);
            formatter.Serialize(memoryStream, obj);
            memoryStream.Seek(0, SeekOrigin.Begin);
            T clone = (T)formatter.Deserialize(memoryStream);
            return clone;
        }
    }
}
