//@author Nicholas
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
        private const string ERROR_UNSERIALIZABLE = "Object is not serializable";
        private static Stack<T> RedoStack = new Stack<T>();

        /// <summary>
        /// Create a State object to represent object states
        /// </summary>
        public State()
        {
            this.Value = new T();
            //this.States = new List<T>();
        }

        /// <summary>
        /// Current State
        /// </summary>
        public T Value
        {
            get;
            set;
        }

        private List<T> stateList;

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
                    stateList = new List<T>();
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
        public void AddState()
        {
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
            Debug.Assert(typeof(T).IsSerializable, ERROR_UNSERIALIZABLE);
            MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.Clone));
            formatter.Serialize(memoryStream, obj);
            memoryStream.Seek(0, SeekOrigin.Begin);
            T clone = (T)formatter.Deserialize(memoryStream);
            return clone;
        }
    }
}
