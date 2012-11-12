//@author A0080933E
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Diagnostics;

namespace Calendo.Data
{
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
        /// <returns>True if file has been changed</returns>
        public override bool Save()
        {
            this.dataStorage.Entries.AddState();
            this.currentState = this.dataStorage.Entries;
            return this.dataStorage.Save();
        }

        /// <summary>
        /// Loads the data
        /// </summary>
        /// <returns>True if the file has been read</returns>
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
        /// <returns>True if a state is reverted, false if no action taken</returns>
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
            Debug.Assert(this.dataStorage.Entries != null);
        }
    }
}
