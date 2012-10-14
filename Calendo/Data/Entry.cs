using System;
using System.Collections.Generic;
using System.Text;

namespace Calendo.Data
{
    /// <summary>
    /// Entry Type
    /// </summary>
    public enum EntryType {
        FLOATING,
        DEADLINE,
        TIMED
    }

    /// <summary>
    /// Time Format
    /// </summary>
    public enum TimeFormat
    {
        DATETIME,
        DATE,
        TIME,
        NONE
    }

    /// <summary>
    /// An entry represents a task in Calendo
    /// </summary>
    [Serializable]
    public class Entry
    {
        private static int IDCounter = 0;

        public Entry() {
            ID = IDCounter++;
            Created = DateTime.Now;
            Description = "";
            StartTime = DateTime.Today;
            StartTimeFormat = TimeFormat.NONE;
            EndTime = DateTime.Today;
            EndTimeFormat = TimeFormat.NONE;
            Type = EntryType.FLOATING;
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
        /// Make a copy of the Entry object
        /// </summary>
        /// <returns>Returns a copy of the object</returns>
        public Entry clone()
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
            return EntryClone;
        }
    }
}
