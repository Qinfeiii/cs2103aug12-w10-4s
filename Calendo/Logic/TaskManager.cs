using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
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

        private TaskManager()
        {
            storage = new StateStorage<List<Entry>>(STORAGE_PATH);
            storage.Load();
            UpdateSubscribers();
        }

        private static TaskManager currentInstance = new TaskManager();
        public static TaskManager Instance
        {
            get
            {
                return currentInstance;
            }
        }

        private List<Delegate> subscriberList = new List<Delegate>();

        /// <summary>
        /// List of subscribers to invoke update methods
        /// </summary>
        public List<Delegate> Subscribers
        {
            get
            {
                return subscriberList;
            }
            set
            {
                subscriberList = value;
            }
        }

        /// <summary>
        /// Call each subscriber update methods
        /// </summary>
        private void UpdateSubscribers()
        {
            foreach (Delegate d in Subscribers)
            {
                d.DynamicInvoke();
            }
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
            this.Add(description, date, time, "", "");
        }

        /// <summary>
        /// Add a task
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
        /// Add a task
        /// </summary>
        /// <param name="description">Task Description</param>
        /// <param name="startDate">Start Date</param>
        /// <param name="startTime">Start Time</param>
        /// <param name="endDate">End Date</param>
        /// <param name="endTime">End Time</param>
        private void Add(string description, TaskTime startTime, TaskTime endTime)
        {
            Entry entry = new Entry();
            entry.Type = GetTaskType(startTime, endTime);
            entry.Description = description;
            entry.StartTime = startTime.Time;
            entry.StartTimeFormat = startTime.Format;
            entry.EndTime = endTime.Time;
            entry.EndTimeFormat = endTime.Format;
            Add(entry);
        }

        /// <summary>
        /// Add an entry to task list
        /// </summary>
        /// <param name="entry"></param>
        private void Add(Entry entry)
        {
            storage.Entries.Add(entry);
            storage.Save();
            UpdateSubscribers();
        }

        /// <summary>
        /// Gets the task type
        /// </summary>
        /// <param name="startTime">Start Time</param>
        /// <param name="endTime">End Time</param>
        /// <returns></returns>
        private EntryType GetTaskType(TaskTime startTime, TaskTime endTime)
        {
            if (startTime != null && endTime != null && startTime.Format != TimeFormat.NONE && endTime.Format != TimeFormat.NONE)
            {
                if (startTime.Time > endTime.Time)
                {
                    // End is before start, mark both as invalid
                    startTime.Format = TimeFormat.NONE;
                    endTime.Format = TimeFormat.NONE;
                    DebugTool.Alert("End date cannot be before start date.");
                    return EntryType.FLOATING;
                }
            }
            // Start and end times are valid
            if (startTime == null || startTime.Format == TimeFormat.NONE)
            {
                if (endTime != null)
                {
                    // Mark end time as not valid
                    endTime.Format = TimeFormat.NONE;
                }
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
            this.Change(id, description, "", "", "", "");
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
                if (FlagContains(flag, FLAG_DESCRIPTION))
                {
                    entry.Description = description;
                }
                if (FlagContains(flag, FLAG_STARTTIME))
                {
                    entry.StartTime = startTime.Time;
                    entry.StartTimeFormat = startTime.Format;
                }
                if (FlagContains(flag, FLAG_ENDTIME))
                {
                    entry.EndTime = endTime.Time;
                    entry.EndTimeFormat = endTime.Format;
                }
                entry.Type = GetTaskType(new TaskTime(entry.StartTime, entry.StartTimeFormat), new TaskTime(entry.EndTime, entry.EndTimeFormat));
                storage.Save();
                UpdateSubscribers();
            }
            else
            {
                DebugTool.Alert(ERROR_ENTRYNOTFOUND);
            }
        }

        /// <summary>
        /// Checks if a flag contains the attribute
        /// </summary>
        /// <param name="flag">Binary Flag</param>
        /// <param name="attribute">Attribute</param>
        /// <returns>Returns true if flag contains the attribute</returns>
        private bool FlagContains(int flag, int attribute)
        {
            return (flag & attribute) == attribute;
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
                UpdateSubscribers();
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
            UpdateSubscribers();
        }

        /// <summary>
        /// Redo an operation
        /// </summary>
        public void Redo()
        {
            storage.Redo();
            UpdateSubscribers();
        }

        /// <summary>
        /// Export from Google Calendar
        /// </summary>
        public void Export()
        {
            // Authorization must occur on same thread as main application
            GoogleCalendar.GoogleCalendar.Authorize();

            // Multithread so UI will not be frozen by slow web requests
            Thread threadInstance = new Thread(new ThreadStart(GCalExport));
            threadInstance.Start();
        }

        /// <summary>
        /// Wrapper method for multithreading export
        /// </summary>
        private void GCalExport()
        {
            GoogleCalendar.GoogleCalendar gcal = new GoogleCalendar.GoogleCalendar();
            gcal.Sync();
        }

        /// <summary>
        /// Import from Google Calendar
        /// </summary>
        public void Import()
        {
            // Authorization must occur on same thread as main application
            GoogleCalendar.GoogleCalendar.Authorize();

            // Multithread so UI will not be frozen by slow web requests
            Thread threadInstance = new Thread(new ThreadStart(GCalImport));
            threadInstance.Start();
        }

        /// <summary>
        /// Wrapper method for multithreading export
        /// </summary>
        private void GCalImport()
        {
            GoogleCalendar.GoogleCalendar gcal = new GoogleCalendar.GoogleCalendar();
            gcal.Import();
        }

        /// <summary>
        /// Force a save
        /// </summary>
        public void Save()
        {
            storage.Save();
            UpdateSubscribers();
        }

        /// <summary>
        /// Force a load
        /// </summary>
        public void Load()
        {
            // Note: Loading does not require updating subscribers
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
            TimeConverter timeConvert = new TimeConverter();
            return timeConvert.Convert(date, time);
        }

    }
}
