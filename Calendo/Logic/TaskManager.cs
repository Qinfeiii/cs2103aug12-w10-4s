//@author A0080933E
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
using Calendo.Data;
using Calendo.Diagnostics;

namespace Calendo.Logic
{
    [Flags]
    public enum ModifyFlag
    {
        Description = 1,
        StartDate = 2,
        StartTime = 4,
        EndDate = 8,
        EndTime = 16,
        EraseStartDate = 32,
        EraseStartTime = 64,
        EraseEndDate = 128,
        EraseEndTime = 256
    }

    public class TaskManager
    {
        private const string ERROR_ENTRYNOTFOUND = "Entry not found";
        private const string ERROR_INVALIDDATETIME = "Specified Date or Time is invalid";
        private const string KEYWORD_REMOVE = "-";
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
            if (startDate.Contains("-") && !HasText(endDate))
            {
                string[] dateFragment = startDate.Split(new char[] { '-' }, 2);
                if (dateFragment.Length > 1)
                {
                    startDate = dateFragment[0];
                    endDate = dateFragment[1];
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

            if (isStartFormatNone)
            {
                if (!isEndFormatNone)
                {
                    // End time not to be used
                    endTime.Format = TimeFormat.None;
                }
                // No start time, means it is a floating task
                return EntryType.Floating;
            }
            else
            {
                if (isEndFormatNone)
                {
                    // Has only start time
                    return EntryType.Deadline;
                }
                else
                {
                    if (startTime.Time > endTime.Time)
                    {
                        // End is before start, mark end as invalid
                        endTime.Format = TimeFormat.None;
                        DebugTool.Alert("End date cannot be before start date.");
                        return EntryType.Deadline;
                    }
                    else
                    {
                        // Both Start and End time are used
                        return EntryType.Timed;
                    }
                }
            }
        }

        /// <summary>
        /// Determine if provided TaskTime has a time format
        /// </summary>
        /// <param name="taskTime">TaskTime object</param>
        /// <returns>True if no time format</returns>
        private bool HasNoTimeFormat(TaskTime taskTime)
        {
            return (taskTime == null) || (taskTime.Format == TimeFormat.None);
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
            ModifyFlag flag = 0; // Flag is a bitwise switch determining which field to change

            // Description changed
            flag = AddFlag(flag, ModifyFlag.Description, HasText(description));

            if (!startDateTime.HasError)
            {
                // Start Date changed
                flag = AddFlag(flag, ModifyFlag.StartDate, HasText(startDate));
                // Start Time changed
                flag = AddFlag(flag, ModifyFlag.StartTime, HasText(startTime));
            }

            if (!endDateTime.HasError)
            {
                // End Date changed
                flag = AddFlag(flag, ModifyFlag.EndDate, !endDateTime.HasError && HasText(endDate));
                // End Time changed
                flag = AddFlag(flag, ModifyFlag.EndTime, !endDateTime.HasError && HasText(endTime));
            }

            // Flag entries for removal if requested
            flag = AddFlag(flag, ModifyFlag.StartDate | ModifyFlag.EraseStartDate, startDate == KEYWORD_REMOVE);
            flag = AddFlag(flag, ModifyFlag.StartTime | ModifyFlag.EraseStartTime, startTime == KEYWORD_REMOVE);
            flag = AddFlag(flag, ModifyFlag.EndDate | ModifyFlag.EraseEndDate, endDate == KEYWORD_REMOVE);
            flag = AddFlag(flag, ModifyFlag.EndTime | ModifyFlag.EraseEndTime, endTime == KEYWORD_REMOVE);

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
        private void Change(int id, ModifyFlag flag, string description, TaskTime startTime, TaskTime endTime)
        {
            Entry entry = this.Get(id);
            if (entry != null)
            {
                // If the relevant flag is set, update that value, otherwise ignore the value

                if (ContainsFlag(flag, ModifyFlag.Description))
                {
                    entry.Description = description;
                }
                if (ContainsFlag(flag, ModifyFlag.StartTime | ModifyFlag.StartDate))
                {
                    // Create a new flag to avoid conflicts with other time values
                    ModifyFlag startFlag = UnsetFlag(flag, ModifyFlag.EndDate | ModifyFlag.EndTime);

                    TaskTime entryTime = new TaskTime(entry.StartTime, entry.StartTimeFormat);
                    TaskTime mergedTime = MergeTime(startTime, entryTime, startFlag);
                    entry.StartTime = mergedTime.Time;
                    entry.StartTimeFormat = mergedTime.Format;
                }
                if (ContainsFlag(flag, ModifyFlag.EndTime | ModifyFlag.EndDate))
                {
                    // Create a new flag to avoid conflicts with other time values
                    ModifyFlag endFlag = UnsetFlag(flag, ModifyFlag.StartDate | ModifyFlag.StartTime);
                    TaskTime entryTime = new TaskTime(entry.EndTime, entry.EndTimeFormat);
                    TaskTime mergedTime = MergeTime(endTime, entryTime, endFlag);
                    entry.EndTime = mergedTime.Time;
                    entry.EndTimeFormat = mergedTime.Format;
                }
                TaskTime startTaskTime = new TaskTime(entry.StartTime, entry.StartTimeFormat);
                TaskTime endTaskTime = new TaskTime(entry.EndTime, entry.EndTimeFormat);
                entry.Type = this.GetTaskType(startTaskTime, endTaskTime);

                // Re-update in case there are invalid times caught by checking task type
                entry.StartTime = startTaskTime.Time;
                entry.StartTimeFormat = startTaskTime.Format;
                entry.EndTime = endTaskTime.Time;
                entry.EndTimeFormat = endTaskTime.Format;
                this.Save();
            }
            else
            {
                DebugTool.Alert(ERROR_ENTRYNOTFOUND);
            }
        }

        /// <summary>
        /// Merge changes in times
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        private TaskTime MergeTime(TaskTime source, TaskTime destination, ModifyFlag flag)
        {
            int day = destination.Time.Day;
            int month = destination.Time.Month;
            int year = destination.Time.Year;
            int minute = destination.Time.Minute;
            int hour = destination.Time.Hour;
            TimeFormat format = destination.Format;

            // Only override required fields
            if (ContainsFlag(flag, ModifyFlag.StartDate | ModifyFlag.EndDate))
            {
                day = source.Time.Day;
                month = source.Time.Month;
                year = source.Time.Year;
                // Add the date flag
                format |= TimeFormat.Date;
            }
            if (ContainsFlag(flag, ModifyFlag.StartTime | ModifyFlag.EndTime))
            {
                minute = source.Time.Minute;
                hour = source.Time.Hour;
                // Add the time flag
                format |= TimeFormat.Time;
            }
            if (ContainsFlag(flag, ModifyFlag.EraseStartDate | ModifyFlag.EraseEndDate))
            {
                // Unset the date flag
                format |= TimeFormat.Date;
                format ^= TimeFormat.Date;
            }
            if (ContainsFlag(flag, ModifyFlag.EraseStartTime | ModifyFlag.EraseEndTime))
            {
                // Unset the time flag
                format |= TimeFormat.Time;
                format ^= TimeFormat.Time;
            }
            DateTime newTime = new DateTime(year, month, day, hour, minute, 0);
            return new TaskTime(newTime, format);
        }

        /// <summary>
        /// Checks if a flag contains the attribute
        /// </summary>
        /// <param name="flag">Binary Flag</param>
        /// <param name="attribute">Attribute</param>
        /// <returns>Returns true if flag has at least one of the attributes</returns>
        private bool ContainsFlag(ModifyFlag flag, ModifyFlag attribute)
        {
            return (flag & attribute) != 0;
        }

        /// <summary>
        /// Adds attributes to the flag if the condition is true
        /// </summary>
        /// <param name="flag">Binary Flag</param>
        /// <param name="attribute">Attribute</param>
        /// <param name="condition">Condition to check with, default is true</param>
        /// <returns>Returns the merged flag</returns>
        private ModifyFlag AddFlag(ModifyFlag flag, ModifyFlag attribute, bool condition = true)
        {
            if (condition)
            {
                return flag | attribute;
            }
            else
            {
                return flag;
            }
        }

        /// <summary>
        /// Removes the attributes from the flag
        /// </summary>
        /// <param name="flag">Binary flag</param>
        /// <param name="attribute">Attributes to be removed</param>
        /// <returns>Flag with attributes removed</returns>
        private ModifyFlag UnsetFlag(ModifyFlag flag, ModifyFlag attribute)
        {
            ModifyFlag presetFlag = AddFlag(flag, attribute);
            ModifyFlag unsetFlag = presetFlag ^ attribute;
            return unsetFlag;
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
        /// <param name="input">String to be converted</param>
        /// <returns>If input string was null, return an empty string. Otherwise return the original string.</returns>
        private string SanitizeString(string input)
        {
            string convertedString = "";
            if (input != null)
            {
                convertedString = input;
            }

            Debug.Assert(convertedString != null);
            return convertedString;
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
