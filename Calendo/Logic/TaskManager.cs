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
        private const int FLAG_DESCRIPTION = 1;
        private const int FLAG_STARTTIME = 2;
        private const int FLAG_ENDTIME = 4;
        private const string ERROR_ENTRYNOTFOUND = "Entry not found";
        private const string ERROR_INVALIDDATETIME = "Specified Date or Time is invalid";
        private const string STORAGE_PATH = "archive.txt";
        private static TaskManager CurrentInstance = new TaskManager();
        private StateStorage<List<Entry>> storage;
        private List<Delegate> subscribers = new List<Delegate>();

        /// <summary>
        /// Update Handler for TaskManager subscriber
        /// </summary>
        public delegate void UpdateHandler();

        /// <summary>
        /// Creates a new instance of TaskManager
        /// </summary>
        private TaskManager()
        {
            this.storage = new StateStorage<List<Entry>>(STORAGE_PATH);
            this.storage.Load();
            this.UpdateSubscribers();
        }

        /// <summary>
        /// Gets the current instance of TaskManager
        /// </summary>
        public static TaskManager Instance
        {
            get
            {
                return CurrentInstance;
            }
        }
        
        /// <summary>
        /// Adds a handler to the list of subscribers
        /// </summary>
        /// <param name="updateHandler">Update Handler</param>
        public void AddSubscriber(Delegate updateHandler)
        {
            this.subscribers.Add(updateHandler);
        }

        /// <summary>
        /// Invoke subscriber update methods
        /// </summary>
        private void UpdateSubscribers()
        {
            foreach (Delegate handler in subscribers)
            {
                handler.DynamicInvoke();
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
            this.Add(description, new TaskTime(), new TaskTime());
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
            startDate = SanitizeString(startDate);
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
            this.Add(entry);
        }

        /// <summary>
        /// Add an entry to task list
        /// </summary>
        /// <param name="entry"></param>
        private void Add(Entry entry)
        {
            this.storage.Entries.Add(entry);
            this.Save();
        }

        /// <summary>
        /// Gets the task type
        /// </summary>
        /// <param name="startTime">Start Time</param>
        /// <param name="endTime">End Time</param>
        /// <returns></returns>
        private EntryType GetTaskType(TaskTime startTime, TaskTime endTime)
        {
            bool isStartFormatNone = HasNoTimeFormat(startTime);
            bool isEndFormatNone = HasNoTimeFormat(endTime);

            if (!isStartFormatNone && !isEndFormatNone)
            {
                if (startTime.Time > endTime.Time)
                {
                    // End is before start, mark as invalid
                    startTime.Format = TimeFormat.NONE;
                    endTime.Format = TimeFormat.NONE;
                    DebugTool.Alert("End date cannot be before start date.");
                    return EntryType.FLOATING;
                }
            }
            // Start time none, but end time set, mark as invalid
            if (isStartFormatNone)
            {
                if (!HasNoTimeFormat(endTime))
                {
                    // Mark end time as not valid
                    endTime.Format = TimeFormat.NONE;
                }
                return EntryType.FLOATING;
            }

            // Has Start time, but no end time
            if (!isStartFormatNone && isEndFormatNone)
            {
                return EntryType.DEADLINE;
            }

            // Both Start and End time are used
            return EntryType.TIMED;
        }

        /// <summary>
        /// Determine if provided TaskTime has a time format
        /// </summary>
        /// <param name="taskTime">TaskTime object</param>
        /// <returns>True if no time format</returns>
        private bool HasNoTimeFormat(TaskTime taskTime)
        {
            return (taskTime == null) || (taskTime.Format == TimeFormat.NONE);
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
            if (!startDateTime.HasError && HasText(startDate, startTime))
            {
                // Start Date changed
                flag |= FLAG_STARTTIME;
            }
            if (!endDateTime.HasError && HasText(endDate, endTime))
            {
                // End Date changed
                flag |= FLAG_ENDTIME;
            }
            this.Change(id, flag, description, startDateTime, endDateTime);
        }

        /// <summary>
        /// Checks if there is a non-empty string amongst list of provided strings
        /// </summary>
        /// <param name="strings">Strings to check</param>
        /// <returns>Returns true if there is at least one non-empty string</returns>
        private bool HasText(params string[] strings)
        {
            foreach (string value in strings)
            {
                if (value != null && value.Trim() != "")
                {
                    return true;
                }
            }
            return false;
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
                if (this.FlagContains(flag, FLAG_DESCRIPTION))
                {
                    entry.Description = description;
                }
                if (this.FlagContains(flag, FLAG_STARTTIME))
                {
                    entry.StartTime = startTime.Time;
                    entry.StartTimeFormat = startTime.Format;
                }
                if (this.FlagContains(flag, FLAG_ENDTIME))
                {
                    entry.EndTime = endTime.Time;
                    entry.EndTimeFormat = endTime.Format;
                }
                TaskTime startTaskTime = new TaskTime(entry.StartTime, entry.StartTimeFormat);
                TaskTime endTaskTime = new TaskTime(entry.EndTime, entry.EndTimeFormat);
                entry.Type = this.GetTaskType(startTaskTime, endTaskTime);
                this.Save();
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
                this.storage.Entries.Remove(entry);
                this.Save();
            }
            else
            {
                DebugTool.Alert(ERROR_ENTRYNOTFOUND);
            }
        }

        /// <summary>
        /// Get a task by ID
        /// </summary>
        /// <param name="id">Task ID, 1-based</param>
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
            this.storage.Undo();
            this.UpdateSubscribers();
        }

        /// <summary>
        /// Redo an operation
        /// </summary>
        public void Redo()
        {
            this.storage.Redo();
            this.UpdateSubscribers();
        }

        /// <summary>
        /// Export from Google Calendar
        /// </summary>
        public void Export()
        {
            // Authorization must occur on same thread as main application
            GoogleCalendar.GoogleCalendar.Authorize();
            this.RunThread(new ThreadStart(ThreadedExport));
        }

        /// <summary>
        /// Import from Google Calendar
        /// </summary>
        public void Import()
        {
            // Authorization must occur on same thread as main application
            GoogleCalendar.GoogleCalendar.Authorize();
            this.RunThread(new ThreadStart(ThreadedImport));
        }

        /// <summary>
        /// Wrapper method for multithreading export
        /// </summary>
        private void ThreadedExport()
        {
            GoogleCalendar.GoogleCalendar gcal = new GoogleCalendar.GoogleCalendar();
            gcal.Export();
        }

        /// <summary>
        /// Wrapper method for multithreading import
        /// </summary>
        private void ThreadedImport()
        {
            GoogleCalendar.GoogleCalendar gcal = new GoogleCalendar.GoogleCalendar();
            gcal.Import();
        }

        /// <summary>
        /// Performs the operation in a separate thread
        /// </summary>
        /// <param name="method"></param>
        private void RunThread(ThreadStart method)
        {
            Thread threadInstance = new Thread(method);
            threadInstance.Start();
        }

        /// <summary>
        /// Force a save
        /// </summary>
        public void Save()
        {
            this.storage.Save();
            this.UpdateSubscribers();
        }

        /// <summary>
        /// Force a load
        /// </summary>
        public void Load()
        {
            // Loading does not notify subscribers (otherwise it triggers infinite loop if they load on update)
            this.storage.Load();
        }
        
        /// <summary>
        /// Converts a string to a processable type
        /// </summary>
        /// <param name="str">String to be converted</param>
        /// <returns>If input string was null, return an empty string. Otherwise return the original string.</returns>
        private string SanitizeString(string str)
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
