//@author A0080933E
using System;
using System.Collections.Generic;
using System.Text;

namespace Calendo.Logic
{
    /// <summary>
    /// Entry Type
    /// </summary>
    public enum EntryType {
        Floating,
        Deadline,
        Timed,
        Complete
    }

    /// <summary>
    /// Time Format
    /// </summary>
    public enum TimeFormat
    {
        DateTime = 3,
        Date = 1,
        Time = 2,
        None = 0
    }

    /// <summary>
    /// An entry represents a task in Calendo
    /// </summary>
    [Serializable]
    public class Entry : ICloneable
    {
        private static int IDCounter = 0;

        /// <summary>
        /// Creates a new entry object
        /// </summary>
        public Entry() {
            ID = IDCounter++;
            Created = DateTime.Now;
            Description = "";
            StartTime = DateTime.Today;
            StartTimeFormat = TimeFormat.None;
            EndTime = DateTime.Today;
            EndTimeFormat = TimeFormat.None;
            Type = EntryType.Floating;
        }

        /// <summary>
        /// Identification number
        /// </summary>
        public int ID
        {
            get;
            set;
        }

        /// <summary>
        /// Creation time
        /// </summary>
        public DateTime Created
        {
            get;
            set;
        }

        /// <summary>
        /// Description
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Start Time
        /// </summary>
        public DateTime StartTime
        {
            get;
            set;
        }

        /// <summary>
        /// Format of the Start Time
        /// </summary>
        public TimeFormat StartTimeFormat
        {
            get;
            set;
        }

        /// <summary>
        /// End Time
        /// </summary>
        public DateTime EndTime
        {
            get;
            set;
        }

        /// <summary>
        /// Format of the End Time
        /// </summary>
        public TimeFormat EndTimeFormat
        {
            get;
            set;
        }

        /// <summary>
        /// Entry Type
        /// </summary>
        public EntryType Type
        {
            get;
            set;
        }

        /// <summary>
        /// Meta information about the entry
        /// </summary>
        public string Meta
        {
            get;
            set;
        }

        /// <summary>
        /// Make a copy of the Entry object
        /// </summary>
        /// <returns>Returns a copy of the object</returns>
        public object Clone()
        {
            Entry EntryClone = new Entry();
            EntryClone.ID = ID;
            EntryClone.Created = Created;
            EntryClone.Description = Description;
            EntryClone.StartTime = StartTime;
            EntryClone.StartTimeFormat = StartTimeFormat;
            EntryClone.EndTime = EndTime;
            EntryClone.EndTimeFormat = EndTimeFormat;
            EntryClone.Type = Type;
            EntryClone.Meta = Meta;
            return (object) EntryClone;
        }
    }
}
