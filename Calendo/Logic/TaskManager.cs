//@author A0080933E
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
using Calendo.Data;
using Calendo.Diagnostics;
using Calendo.GoogleCalendar;

namespace Calendo.Logic
{
    public class TaskManager
    {
        private const string ERROR_ENTRY_NOT_FOUND = "Entry not found";
        private const string ERROR_INVALID_DATETIME = "Specified Date or Time is invalid";
        private const string ERROR_END_BEFORE_START = "End date cannot be before start date";
        private const string KEYWORD_REMOVE = "-";
        private const string STORAGE_PATH = "archive.txt";
        private const bool ALLOW_CONTINUE_ON_ERROR = false;
        private const ModifyFlag CHANGE_DESCRIPTION = ModifyFlag.Description;
        private const ModifyFlag CHANGE_START = ModifyFlag.StartDate | ModifyFlag.StartTime | ModifyFlag.EraseStartDate | ModifyFlag.EraseStartTime;
        private const ModifyFlag CHANGE_END = ModifyFlag.EndDate | ModifyFlag.EndTime | ModifyFlag.EraseEndDate | ModifyFlag.EraseEndTime;
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
        /// Gets the list of entries
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
        public void Add(string description, string startDate = "", string startTime = "", string endDate = "", string endTime = "")
        {
            description = SanitizeString(description);
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
            if (!ALLOW_CONTINUE_ON_ERROR)
            {
                if (startTime.HasError || endTime.HasError)
                {
                    // Do not allow entry to be added
                    return;
                }
            }
            this.Add(entry);
        }

        /// <summary>
        /// Add a task
        /// </summary>
        /// <param name="entry">Entry to be added</param>
        private void Add(Entry entry)
        {
            this.storage.Entries.Add(entry);
            this.Save();
            this.RecordLog(entry, "Add");
        }

        /// <summary>
        /// Gets the task type
        /// </summary>
        /// <param name="startTime">Start Time</param>
        /// <param name="endTime">End Time</param>
        /// <returns></returns>
        private EntryType GetTaskType(TaskTime startTime, TaskTime endTime)
        {
            bool hasStartTime = HasTimeFormat(startTime);
            bool hasEndTime = HasTimeFormat(endTime);

            if (!hasStartTime)
            {
                if (hasEndTime)
                {
                    // End time not to be used
                    endTime.Format = TimeFormat.None;
                    endTime.HasError = true;
                }
                // No start time, means it is a floating task
                return EntryType.Floating;
            }
            else
            {
                if (!hasEndTime)
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
                        endTime.HasError = true;
                        DebugTool.Alert(ERROR_END_BEFORE_START);
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
        private bool HasTimeFormat(TaskTime taskTime)
        {
            Debug.Assert(taskTime != null);
            return (taskTime.Format != TimeFormat.None);
        }

        /// <summary>
        /// Modify a task
        /// </summary>
        /// <param name="description">Task Description</param>
        /// <param name="startDate">Start Date</param>
        /// <param name="startTime">Start Time</param>
        /// <param name="endDate">End Date</param>
        /// <param name="endTime">End Time</param>
        public void Change(int id, string description = "", string startDate = "", string startTime = "", string endDate = "", string endTime = "")
        {
            TaskTime startDateTime = this.ConvertTime(startDate, startTime);
            TaskTime endDateTime = this.ConvertTime(endDate, endTime);
            ModifyFlag flag = 0; // Flag determines which field to change

            // Description changed
            flag = flag.Add(ModifyFlag.Description, HasText(description));

            if (!startDateTime.HasError)
            {
                // Start Date or Time changed
                flag = flag.Add(ModifyFlag.StartDate, HasText(startDate));
                flag = flag.Add(ModifyFlag.StartTime, HasText(startTime));
            }

            if (!endDateTime.HasError)
            {
                // End Date or Time changed
                flag = flag.Add(ModifyFlag.EndDate, HasText(endDate));
                flag = flag.Add(ModifyFlag.EndTime, HasText(endTime));
            }

            // Flag parameters for removal if requested
            flag = flag.Add(ModifyFlag.EraseStartDate, startDate == KEYWORD_REMOVE);
            flag = flag.Add(ModifyFlag.EraseStartTime, startTime == KEYWORD_REMOVE);
            flag = flag.Add(ModifyFlag.EraseEndDate, endDate == KEYWORD_REMOVE);
            flag = flag.Add(ModifyFlag.EraseEndTime, endTime == KEYWORD_REMOVE);

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
                string entryDescription = entry.Description;
                if (flag.Contains(ModifyFlag.Description))
                {
                    entryDescription = description;
                }

                TaskTime entryStart = new TaskTime(entry.StartTime, entry.StartTimeFormat);
                TaskTime entryEnd = new TaskTime(entry.EndTime, entry.EndTimeFormat);

                if (flag.Contains(CHANGE_START))
                {
                    ModifyFlag startFlag = flag.Unset(CHANGE_END);
                    entryStart = TimeConverter.MergeTime(startTime, entryStart, startFlag);
                }

                if (flag.Contains(CHANGE_END))
                {
                    ModifyFlag endFlag = flag.Unset(CHANGE_START);
                    entryEnd = TimeConverter.MergeTime(endTime, entryEnd, endFlag);
                }

                entry.Type = this.GetTaskType(entryStart, entryEnd);

                if (!ALLOW_CONTINUE_ON_ERROR)
                {
                    if (entryStart.HasError || entryEnd.HasError || startTime.HasError || endTime.HasError)
                    {
                        // Do not allow entry to be changed
                        return;
                    }
                }

                // Make changes if checks pass
                entry.Description = entryDescription;
                entry.StartTime = entryStart.Time;
                entry.StartTimeFormat = entryStart.Format;
                entry.EndTime = entryEnd.Time;
                entry.EndTimeFormat = entryEnd.Format;

                this.Save();
            }
            else
            {
                DebugTool.Alert(ERROR_ENTRY_NOT_FOUND);
            }
            this.RecordLog(entry, "Change", id.ToString(), flag.ToString());
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
                DebugTool.Alert(ERROR_ENTRY_NOT_FOUND);
            }
            this.RecordLog(entry, "Remove", id.ToString());
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
            this.RecordLog(null, "Undo");
            this.UpdateSubscribers();
        }

        /// <summary>
        /// Redo an operation
        /// </summary>
        public void Redo()
        {
            this.storage.Redo();
            this.RecordLog(null, "Redo");
            this.UpdateSubscribers();
        }

        /// <summary>
        /// Export from Google Calendar
        /// </summary>
        public void Export()
        {
            ThreadedGoogleCalendar.Export();
        }

        /// <summary>
        /// Import from Google Calendar
        /// </summary>
        public void Import()
        {
            ThreadedGoogleCalendar.Import();
        }

        /// <summary>
        /// Save the list of entries
        /// </summary>
        public void Save()
        {
            this.storage.Save();
            this.UpdateSubscribers();
        }

        /// <summary>
        /// Load the entries
        /// </summary>
        public void Load()
        {
            // Subscribers are not notified, it is up to subscriber to update display
            this.storage.Load();
        }

        /// <summary>
        /// Converts a string to a processable type
        /// </summary>
        /// <param name="input">String to be converted</param>
        /// <returns>If input string was null, return an empty string. Otherwise return the original string.</returns>
        private string SanitizeString(string input)
        {
            return TimeConverter.SanitizeString(input);
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

        /// <summary>
        /// Records the operation to the log
        /// </summary>
        /// <param name="entry">Entry to be recorded</param>
        /// <param name="parameters">Parameters involved in operation</param>
        private void RecordLog(Entry entry, params string[] parameters)
        {
            string entryDescription = "[Empty entry]";
            string entryFormat = "[ID: {0}, {1}]";
            if (entry != null)
            {
                entryDescription = String.Format(entryFormat, entry.ID, entry.Description);
            }

            string parameterDescription = String.Join(", ", parameters);
            string parameterFormat = " [Parameters: {0}]";
            parameterDescription = String.Format(parameterFormat, parameterDescription);
            DebugTool.WriteLog(entryDescription + parameterDescription);
        }
    }
}
