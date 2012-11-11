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
        private const ModifyFlag CHANGE_DESCRIPTION = ModifyFlag.Description;
        private const ModifyFlag CHANGE_START = ModifyFlag.StartDate | ModifyFlag.StartTime | ModifyFlag.EraseStartDate | ModifyFlag.EraseStartTime;
        private const ModifyFlag CHANGE_END = ModifyFlag.EndDate | ModifyFlag.EndTime | ModifyFlag.EraseEndDate | ModifyFlag.EraseEndTime;
        private static TaskManager CurrentInstance = new TaskManager();
        private static bool AllowContinueOnError = false;
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
        /// Call subscriber update methods
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
            MakeTimedShorthand(ref startDate, ref endDate);
            TaskTime startDateTime = this.ConvertTime(startDate, startTime);
            TaskTime endDateTime = this.ConvertTime(endDate, endTime);
            this.Add(description, startDateTime, endDateTime);
            this.RecordLog("Add", description, startDate, startTime, endDate, endTime);
        }

        /// <summary>
        /// Prepares conversion to a timed task using shorthand hyphen keyword
        /// </summary>
        /// <param name="startDate">Start Date</param>
        /// <param name="endDate">End Date</param>
        private void MakeTimedShorthand(ref string startDate, ref string endDate)
        {
            if (startDate.Contains("-") && !HasText(endDate))
            {
                string[] dateFragment = startDate.Split(new char[] { '-' }, 2);
                if (dateFragment.Length > 1)
                {
                    startDate = dateFragment[0];
                    endDate = dateFragment[1];
                }
            }
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
            UpdateTimes(entry, startTime, endTime);
            if (HasError(startTime, endTime))
            {
                return;
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
        }

        /// <summary>
        /// Gets the entry type
        /// </summary>
        /// <param name="startTime">Start Time</param>
        /// <param name="endTime">End Time</param>
        /// <returns>Entry Type</returns>
        private EntryType GetTaskType(TaskTime startTime, TaskTime endTime)
        {
            bool hasStartTime = HasTimeFormat(startTime);
            bool hasEndTime = HasTimeFormat(endTime);

            if (!hasStartTime)
            {
                if (hasEndTime)
                {
                    MarkEndTimeInvalid(endTime);
                }
                return EntryType.Floating;
            }
            else
            {
                if (!hasEndTime)
                {
                    return EntryType.Deadline;
                }
                else if (startTime.Time > endTime.Time)
                {
                    // End is before start
                    MarkEndTimeInvalid(endTime);
                    DebugTool.Alert(ERROR_END_BEFORE_START);
                    return EntryType.Deadline;
                }
                else
                {
                    return EntryType.Timed;
                }
            }
        }

        private void MarkEndTimeInvalid(TaskTime endTime)
        {
            endTime.Format = TimeFormat.None;
            endTime.HasError = true;
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
            ModifyFlag modifyFlag = 0; // Flag determines which field to change

            // Description changed
            modifyFlag = modifyFlag.Add(ModifyFlag.Description, HasText(description));

            if (!startDateTime.HasError)
            {
                // Start Date or Time changed
                modifyFlag = modifyFlag.Add(ModifyFlag.StartDate, HasText(startDate));
                modifyFlag = modifyFlag.Add(ModifyFlag.StartTime, HasText(startTime));
            }

            if (!endDateTime.HasError)
            {
                // End Date or Time changed
                modifyFlag = modifyFlag.Add(ModifyFlag.EndDate, HasText(endDate));
                modifyFlag = modifyFlag.Add(ModifyFlag.EndTime, HasText(endTime));
            }

            // Flag parameters for removal if requested
            modifyFlag = modifyFlag.Add(ModifyFlag.EraseStartDate, startDate == KEYWORD_REMOVE);
            modifyFlag = modifyFlag.Add(ModifyFlag.EraseStartTime, startTime == KEYWORD_REMOVE);
            modifyFlag = modifyFlag.Add(ModifyFlag.EraseEndDate, endDate == KEYWORD_REMOVE);
            modifyFlag = modifyFlag.Add(ModifyFlag.EraseEndTime, endTime == KEYWORD_REMOVE);

            this.Change(id, modifyFlag, description, startDateTime, endDateTime);
            this.RecordLog("Change", id.ToString(), description, startDate, startTime, endDate, endTime);
        }

        /// <summary>
        /// Modify a task
        /// </summary>
        /// <param name="id">Task Number</param>
        /// <param name="modifyFlag">Flag</param>
        /// <param name="description">Task Description</param>
        /// <param name="startDate">Start Date</param>
        /// <param name="startTime">Start Time</param>
        /// <param name="endDate">End Date</param>
        /// <param name="endTime">End Time</param>
        private void Change(int id, ModifyFlag modifyFlag, string description, TaskTime startTime, TaskTime endTime)
        {
            Entry entry = this.Get(id);
            if (entry != null)
            {
                string entryDescription = entry.Description;
                if (modifyFlag.Contains(ModifyFlag.Description))
                {
                    entryDescription = description;
                }

                TaskTime entryStart = new TaskTime(entry.StartTime, entry.StartTimeFormat);
                TaskTime entryEnd = new TaskTime(entry.EndTime, entry.EndTimeFormat);

                if (modifyFlag.Contains(CHANGE_START))
                {
                    ModifyFlag startFlag = modifyFlag.Unset(CHANGE_END);
                    entryStart = TimeConverter.MergeTime(startTime, entryStart, startFlag);
                }

                if (modifyFlag.Contains(CHANGE_END))
                {
                    ModifyFlag endFlag = modifyFlag.Unset(CHANGE_START);
                    entryEnd = TimeConverter.MergeTime(endTime, entryEnd, endFlag);
                }

                entry.Type = this.GetTaskType(entryStart, entryEnd);

                if (HasError(startTime, endTime, entryStart, entryEnd))
                {
                    return;
                }

                // Make changes if checks pass
                entry.Description = entryDescription;
                UpdateTimes(entry, entryStart, entryEnd);

                this.Save();
            }
            else
            {
                DebugTool.Alert(ERROR_ENTRY_NOT_FOUND);
            }
        }

        /// <summary>
        /// Updates entry times
        /// </summary>
        /// <param name="entry">Entry to update</param>
        /// <param name="startTime">Start Time</param>
        /// <param name="endTime">End Time</param>
        private void UpdateTimes(Entry entry, TaskTime startTime, TaskTime endTime)
        {
            entry.StartTime = startTime.Time;
            entry.StartTimeFormat = startTime.Format;
            entry.EndTime = endTime.Time;
            entry.EndTimeFormat = endTime.Format;
        }

        /// <summary>
        /// Checks if there is a non-empty string amongst list of provided strings
        /// </summary>
        /// <param name="strings">Strings to check</param>
        /// <returns>True if there is at least one non-empty string</returns>
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
        /// Checks if the task times have errors
        /// </summary>
        /// <param name="taskTimes">Task Times</param>
        /// <returns>True if at least one has errors</returns>
        private bool HasError(params TaskTime[] taskTimes)
        {
            if (AllowContinueOnError)
            {
                return false;
            }

            foreach (TaskTime taskTime in taskTimes)
            {
                Debug.Assert(taskTime != null);
                if (taskTime.HasError)
                {
                    return true;
                }
            }
            return false;
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
            this.RecordLog("Remove", id.ToString());
        }

        /// <summary>
        /// Get a task by ID
        /// </summary>
        /// <param name="id">Task ID, 1-based</param>
        /// <returns>Entry object matching the ID, null if not found</returns>
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
            this.RecordLog("Undo");
            this.UpdateSubscribers();
        }

        /// <summary>
        /// Redo an operation
        /// </summary>
        public void Redo()
        {
            this.storage.Redo();
            this.RecordLog("Redo");
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
        /// <returns>TaskTime object</returns>
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
        private void RecordLog(params string[] parameters)
        {
            string parameterDescription = String.Join(", ", parameters);
            string parameterFormat = "[{0}]";
            parameterDescription = String.Format(parameterFormat, parameterDescription);
            DebugTool.WriteLog(parameterDescription, MessageType.Info);
        }
    }
}
