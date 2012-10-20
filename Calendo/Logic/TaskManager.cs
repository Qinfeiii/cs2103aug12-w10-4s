using System;
using System.Collections.Generic;
using System.Text;
using Calendo.Data;
using Calendo.Diagnostics;

namespace Calendo.Logic
{

    public class TaskManager
    {
        private StateStorage<List<Entry>> storage;
        private const int FLAG_DESCRIPTION = 1;
        private const int FLAG_STARTTIME = 2;
        private const int FLAG_ENDTIME = 4;
        private const string ERROR_ENTRYNOTFOUND = "Entry not found";
        private const string ERROR_INVALIDDATETIME = "Specified Date or Time is invalid";
        private const string STORAGE_PATH = "archive.txt";

        public TaskManager()
        {
            storage = new StateStorage<List<Entry>>(STORAGE_PATH);
            storage.Load();
        }

        /// <summary>
        /// Get or set the entries
        /// </summary>
        public List<Entry> Entries
        {
            get { return storage.Entries; }
        }

        /// <summary>
        /// Add a floating task
        /// </summary>
        /// <param name="description">Task Description</param>
        public void Add(string description)
        {
            Add(description, new TaskTime(), new TaskTime());
        }

        /// <summary>
        /// Add a deadline task
        /// </summary>
        /// <param name="description">Task Description</param>
        /// <param name="date">Start Date</param>
        /// <param name="time">Start Time</param>
        public void Add(string description, string date, string time)
        {

            TaskTime endTime = new TaskTime();
            date = DefaultString(date);
            if (date.Contains("-"))
            {
                // Date is of format [Start Date]-[End Date]
                string[] dateFrag = date.Split(new char[] { '-' }, 2);
                if (dateFrag.Length > 1)
                {
                    date = dateFrag[0];
                    string endDate = dateFrag[1];
                    endTime = this.ConvertTime(endDate, "");
                }
            }
            TaskTime startTime = this.ConvertTime(date, time);
            this.Add(description, startTime, endTime);
        }

        /// <summary>
        /// Add a timed task
        /// </summary>
        /// <param name="description">Task Description</param>
        /// <param name="startDate">Start Date</param>
        /// <param name="startTime">Start Time</param>
        /// <param name="endDate">End Date</param>
        /// <param name="endTime">End Time</param>
        public void Add(string description, string startDate, string startTime, string endDate, string endTime)
        {
            startDate = DefaultString(startDate);
            if (startDate.Contains("-") && endDate == "")
            {
                // Date is of format [Start Date]-[End Date]
                string[] dateFrag = startDate.Split(new char[] { '-' }, 2);
                if (dateFrag.Length > 1)
                {
                    startDate = dateFrag[0];
                    endDate = dateFrag[1];
                }
            }
            TaskTime startDateTime = this.ConvertTime(startDate, startTime);
            TaskTime endDateTime = this.ConvertTime(endDate, endTime);
            this.Add(description, startDateTime, endDateTime);
        }

        /// <summary>
        /// Add a timed task
        /// </summary>
        /// <param name="description">Task Description</param>
        /// <param name="startDate">Start Date</param>
        /// <param name="startTime">Start Time</param>
        /// <param name="endDate">End Date</param>
        /// <param name="endTime">End Time</param>
        private void Add(string description, TaskTime startTime, TaskTime endTime)
        {
            Entry entry = new Entry();
            entry.Description = description;
            entry.StartTime = startTime.Time;
            entry.StartTimeFormat = startTime.Format;
            entry.EndTime = endTime.Time;
            entry.EndTimeFormat = endTime.Format;
            entry.Type = GetTaskType(startTime, endTime);
            Add(entry);
        }

        /// <summary>
        /// Add a task
        /// </summary>
        /// <param name="entry"></param>
        private void Add(Entry entry)
        {
            storage.Entries.Add(entry);
            storage.Save();
        }

        /// <summary>
        /// Gets the task type
        /// </summary>
        /// <param name="startTime">Start Time</param>
        /// <param name="endTime">End Time</param>
        /// <returns></returns>
        private EntryType GetTaskType(TaskTime startTime, TaskTime endTime)
        {
            if (startTime == null || startTime.Format == TimeFormat.NONE)
            {
                // No start or end time
                return EntryType.FLOATING;
            }
            if (startTime.Format != TimeFormat.NONE && (endTime == null || endTime.Format == TimeFormat.NONE))
            {
                // Only start time is used
                return EntryType.DEADLINE;
            }
            // Both Start and End time are used
            return EntryType.TIMED;
        }

        /// <summary>
        /// Modify a task
        /// </summary>
        /// <param name="id">Task ID</param>
        /// <param name="description">Description</param>
        public void Change(int id, string description)
        {
            Entry entry = this.Get(id);
            if (entry != null)
            {
                entry.Description = description;
                storage.Save();
            }
        }

        /// <summary>
        /// Modify a task
        /// </summary>
        /// <param name="description">Task Description</param>
        /// <param name="startDate">Start Date</param>
        /// <param name="startTime">Start Time</param>
        /// <param name="endDate">End Date</param>
        /// <param name="endTime">End Time</param>
        public void Change(int id, string description, string startDate, string startTime, string endDate, string endTime)
        {
            TaskTime startDateTime = this.ConvertTime(startDate, startTime);
            TaskTime endDateTime = this.ConvertTime(endDate, endTime);
            int flag = 0; // Flag is a bitwise switch determining which field to change
            if (description != "")
            {
                // Description changed
                flag |= FLAG_DESCRIPTION;
            }
            if (!startDateTime.HasError && (startDate + startTime) != "")
            {
                // Start Date changed
                flag |= FLAG_STARTTIME;
            }
            if (!endDateTime.HasError && (endDate + endTime) != "")
            {
                // End Date changed
                flag |= FLAG_ENDTIME;
            }
            this.Change(id, flag, description, startDateTime, endDateTime);
        }

        /// <summary>
        /// Modify a task
        /// </summary>
        /// <param name="id">Task Number</param>
        /// <param name="flag">Flag</param>
        /// <param name="description">Task Description</param>
        /// <param name="startDate">Start Date</param>
        /// <param name="startTime">Start Time</param>
        /// <param name="endDate">End Date</param>
        /// <param name="endTime">End Time</param>
        private void Change(int id, int flag, string description, TaskTime startTime, TaskTime endTime)
        {
            Entry entry = this.Get(id);
            if (entry != null)
            {
                if ((flag & FLAG_DESCRIPTION) == FLAG_DESCRIPTION)
                {
                    entry.Description = description;
                }
                if ((flag & FLAG_STARTTIME) == FLAG_STARTTIME)
                {
                    entry.StartTime = startTime.Time;
                    entry.StartTimeFormat = startTime.Format;
                }
                if ((flag & FLAG_ENDTIME) == FLAG_ENDTIME)
                {
                    entry.EndTime = endTime.Time;
                    entry.EndTimeFormat = endTime.Format;
                }
                entry.Type = GetTaskType(new TaskTime(entry.StartTime, entry.StartTimeFormat), new TaskTime(entry.EndTime, entry.EndTimeFormat));
                storage.Save();
            }
            else
            {
                DebugTool.Alert(ERROR_ENTRYNOTFOUND);
            }
        }

        /// <summary>
        /// Takes in the index of an item to remove from the Entries list, removes that item.
        /// </summary>
        /// <param name="index">The 0-indexed index of the item to be removed.</param>
        public void RemoveByIndex(int index)
        {
            /*
            if (index >= 0 && index < Entries.Count)
            {
                storage.Entries.RemoveAt(index);
                storage.Save();
            }
             * */
            Remove(index + 1);
        }

        /// <summary>
        /// Remove a task by ID
        /// </summary>
        /// <param name="id">Task ID</param>
        public void Remove(int id)
        {
            Entry entry = this.Get(id);
            if (entry != null)
            {
                storage.Entries.Remove(entry);
                storage.Save();
            }
            else
            {
                DebugTool.Alert(ERROR_ENTRYNOTFOUND);
            }
        }

        /// <summary>
        /// Get a task by ID
        /// </summary>
        /// <param name="id">Task ID</param>
        /// <returns>Returns Entry object matching the ID, null if not found</returns>
        public Entry Get(int id)
        {
            if (id >= 1 && id <= storage.Entries.Count)
            {
                return storage.Entries[id - 1];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Undo an operation
        /// </summary>
        public void Undo()
        {
            storage.Undo();
        }

        /// <summary>
        /// Redo an operation
        /// </summary>
        public void Redo()
        {
            storage.Redo();
        }

        public void Sync()
        {
            GoogleCalendar.GoogleCalendar.Import();
        }

        public void Import()
        {
            // STUB
        }

        /// <summary>
        /// Gets the TimeFormat associated with the date and time
        /// </summary>
        /// <param name="hasDate">Format has a date</param>
        /// <param name="hasTime">Format has a time</param>
        /// <returns>Returns TimeFormat value</returns>
        public TimeFormat GetFormat(bool hasDate, bool hasTime)
        {
            TimeFormat newTimeFormat = TimeFormat.NONE;
            if (hasDate)
            {
                newTimeFormat = TimeFormat.DATE;
            }
            if (hasTime)
            {
                newTimeFormat = TimeFormat.TIME;
            }
            if (hasDate && hasTime)
            {
                newTimeFormat = TimeFormat.DATETIME;
            }
            return newTimeFormat;
        }

        /// <summary>
        /// Force a save
        /// </summary>
        public void Save()
        {
            storage.Save();
        }

        /// <summary>
        /// Force a load
        /// </summary>
        public void Load()
        {
            storage.Load();
        }
        
        /// <summary>
        /// Converts a string to a processable type
        /// </summary>
        /// <param name="str">String to be converted</param>
        /// <returns>If input string was null, return an empty string. Otherwise return the original string.</returns>
        private string DefaultString(string str)
        {
            if (str == null)
            {
                return "";
            }
            else
            {
                return str.Trim();
            }
        }

        /// <summary>
        /// Converts a string date and time to DateTime object
        /// </summary>
        /// <param name="date">Date in Day/Month/Year</param>
        /// <param name="time">Time in Hour/Minutes (24 hour)</param>
        /// <returns>Returns TaskTime object</returns>
        private TaskTime ConvertTime(string date, string time)
        {
            TimeConverter tc = new TimeConverter();
            return tc.Convert(date, time);
        }

    }
}
