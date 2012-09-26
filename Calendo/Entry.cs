using System;
using System.Collections.Generic;
using System.Text;

namespace Calendo
{
    public enum EntryType {
        FLOATING,
        DEADLINE,
        TIMED
    }
    public enum TimeFormat
    {
        DATETIME,
        DATE,
        TIME,
        NONE
    }
    public interface IEntry
    {
        int ID
        {
            get;
        }
        string Description
        {
            get;
            set;
        }
        DateTime StartTime
        {
            get;
            set;
        }
        TimeFormat StartTimeFormat
        {
            get;
            set;
        }
        DateTime EndTime
        {
            get;
            set;
        }
        EntryType Type
        {
            get;
        }
        TimeFormat EndTimeFormat
        {
            get;
            set;
        }
    }

    /// <summary>
    /// An entry represents a task in Calendo
    /// </summary>
    public class Entry : IEntry
    {
        private static int IDCounter = 0;
        private int _ID;
        private string _Description;
        private DateTime _StartTime;
        private TimeFormat _StartTimeFormat;
        private DateTime _EndTime;
        private TimeFormat _EndTimeFormat;
        private EntryType _EntryType;
        public Entry() {
            _ID = IDCounter++;
            _Description = "";
            _StartTime = DateTime.Today;
            _StartTimeFormat = TimeFormat.NONE;
            _EndTime = DateTime.Today;
            _EndTimeFormat = TimeFormat.NONE;
            _EntryType = EntryType.FLOATING;
        }

        /// <summary>
        /// Identification number
        /// </summary>
        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        /// <summary>
        /// Description
        /// </summary>
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        /// <summary>
        /// Start Time
        /// </summary>
        public DateTime StartTime
        {
            get { return _StartTime; }
            set { _StartTime = value; }
        }

        /// <summary>
        /// Format of the Start Time
        /// </summary>
        public TimeFormat StartTimeFormat
        {
            get { return _StartTimeFormat; }
            set { _StartTimeFormat = value; }
        }

        /// <summary>
        /// End Time
        /// </summary>
        public DateTime EndTime
        {
            get { return _EndTime; }
            set { _EndTime = value; }
        }

        /// <summary>
        /// Format of the End Time
        /// </summary>
        public TimeFormat EndTimeFormat
        {
            get { return _EndTimeFormat; }
            set { _EndTimeFormat = value; }
        }

        /// <summary>
        /// Entry Type
        /// </summary>
        public EntryType Type
        {
            get { return _EntryType; }
            set { _EntryType = value;  }
        }
    }
}
