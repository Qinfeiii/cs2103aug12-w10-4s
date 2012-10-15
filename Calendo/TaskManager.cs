using System;
using System.Collections.Generic;
using System.Text;
using Calendo.Data;
using Calendo.DebugTool;

namespace Calendo
{
    class TaskTime
    {
        public TaskTime()
        {
            this.Format = TimeFormat.NONE;
            this.Time = DateTime.Today;
            this.IsDefault = true;
        }
        public TaskTime(DateTime Time, TimeFormat Format)
        {
            this.Time = Time;
            this.Format = Format;
            this.IsDefault = false;
        }
        public TimeFormat Format
        {
            get;
            set;
        }
        public DateTime Time
        {
            get;
            set;
        }
        public bool IsDefault
        {
            get;
            set;
        }
    }
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
            if (!startDateTime.IsDefault && (startDate + startTime) != "")
            {
                // Start Date changed
                flag |= FLAG_STARTTIME;
            }
            if (!endDateTime.IsDefault && (endDate + endTime) != "")
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
                Debug.Alert(ERROR_ENTRYNOTFOUND);
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
                Debug.Alert(ERROR_ENTRYNOTFOUND);
            }
        }

        /// <summary>
        /// Get a task by ID
        /// </summary>
        /// <param name="id">Task ID</param>
        /// <returns>Returns Entry object matching the ID, null if not found</returns>
        public Entry Get(int id)
        {
            /*
            for (int i = 0; i < storage.Entries.Count; i++)
            {
                if (storage.Entries[i].ID == id)
                {
                    return storage.Entries[i];
                }
            }
            */
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
            // STUB
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
        /// Is it a leap year?
        /// </summary>
        /// <param name="year">Year</param>
        /// <returns>Returns true if it is a leap year, false otherwise</returns>
        private bool isLeapYear(int year) {
            bool isLeap = false;
            if (year % 4 == 0)
            {
                isLeap = true;
            }
            if (year % 100 == 0)
            {
                isLeap = false;
            }
            if (year % 400 == 0)
            {
                isLeap = true;
            }
            return isLeap;
        }

        /// <summary>
        /// Get the maximum number of days in the specified month
        /// </summary>
        /// <param name="month">Month</param>
        /// <param name="year">Year</param>
        /// <returns></returns>
        private int MaxDays(int month, int year)
        {
            if (month >= 1 && month <= 12 && year >= 0)
            {
                // Max days of each month
                int[] maxDays = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
                if (isLeapYear(year))
                {
                    // February has an extra day
                    maxDays[1] = 29;
                }
                return maxDays[month - 1];
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Converts a string date and time to DateTime object
        /// </summary>
        /// <param name="date">Date in Day/Month/Year</param>
        /// <param name="time">Time in Hour/Minutes (24 hour)</param>
        /// <returns>Returns DateTime object</returns>
        private TaskTime ConvertTime(string date, string time)
        {
            date = DefaultString(date);
            time = DefaultString(time);

            bool isValidDate = true;
            bool isValidTime = true;
            bool hasError = false;


            DateTime defaultDateTime = DateTime.Today;

            // Defaults
            int year = DateTime.Today.Year;
            int day = DateTime.Today.Day;
            int month = DateTime.Today.Month;
            int hour = 0;
            int minute = 0;
            int second = 0;

            // Date: Day/Month[/Year]
            string[] dateFrag = date.Split(new char[] { '/', '.' }, 3);
            if (date == "")
            {
                // No date supplied (not an error)
                isValidDate = false;
            }
            // Month
            if (isValidDate && dateFrag.Length > 1 && dateFrag[1] != "")
            {
                int convertedMonth = this.ConvertInt(dateFrag[1]);
                if (convertedMonth >= 1 && convertedMonth <= 12)
                {
                    month = convertedMonth;
                    if (month < defaultDateTime.Month)
                    {
                        // The month is actually before this month
                        // Assume referring to next year
                        year++;
                    }
                }
                else
                {
                    // Invalid month
                    hasError = true;
                    isValidDate = false;
                }
            }
            // Year (must be in future)
            if (isValidDate && dateFrag.Length > 2 && dateFrag[2] != "")
            {
                int convertedYear = this.ConvertInt(dateFrag[2]);
                if (convertedYear >= defaultDateTime.Year)
                {
                    year = convertedYear;
                }
                else
                {
                    // Invalid year
                    hasError = true;
                    isValidDate = false;
                }
            }
            // Day
            if (isValidDate && dateFrag.Length > 0 && dateFrag[0] != "")
            {
                int convertedDay = this.ConvertInt(dateFrag[0]);
                if (convertedDay >= 1 && convertedDay <= MaxDays(month, year))
                {
                    day = convertedDay;
                }
                else
                {
                    // Invalid day
                    hasError = true;
                    isValidDate = false;
                }
            }

            // Time (24HR): Hour:Minute
            string timeMeta = "";
            if (time.Length > 2)
            {
                timeMeta = time.Substring(time.Length - 2); // Get last 2 letters
                timeMeta = timeMeta.ToUpper();
            }
            
            // Only used by 12 hour format
            // If both are false, 24 hour format is used
            bool isAM = false;
            bool isPM = false;

            // Handle PM
            if (timeMeta == "PM")
            {
                isPM = true;
            }
            if (timeMeta == "AM")
            {
                isAM = true;
            }
            if (isAM || isPM)
            {
                // Get the remainder
                time = time.Substring(0, time.Length - 2);
                time = time.Trim();
            }

            string[] timeFrag = time.Split(new char[] { ':', '.' }, 2);

            if (time == "")
            {
                // No time supplied (not an error)
                isValidTime = false;
            }

            // Process the time field
            // Hour
            if (timeFrag.Length > 0 && timeFrag[0] != "")
            {
                int convertedHour = this.ConvertInt(timeFrag[0]);
                int originalHour = convertedHour;
                if (convertedHour == 12 && isAM)
                {
                    convertedHour = 0;
                }
                if (isPM)
                {
                    if (convertedHour != 12)
                    {
                        convertedHour += 12;
                    }
                }
                // Reject if the original provided hour is invalid, even if resulting hour is correct
                if (originalHour >= 0 && originalHour < 24 && convertedHour >= 0 && convertedHour < 24)
                {
                    hour = convertedHour;

                    if (day == DateTime.Today.Day && month == DateTime.Today.Month && year == DateTime.Today.Year && hour < DateTime.Now.Hour)
                    {
                        // It is on the next day
                        day++;
                        // Last day of the month, roll over to next month
                        if (day >= MaxDays(month, year))
                        {
                            day = 1;
                            month++;
                        }
                        // Last day of the year, roll over to next year
                        if (month > 12)
                        {
                            month = 1;
                            year++;
                        }
                        // Mark as valid date
                        isValidDate = true;
                    }
                }
                else
                {
                    hasError = true;
                    isValidTime = false;
                }

            }

            // Minute
            if (timeFrag.Length > 1 && timeFrag[1] != "")
            {
                int convertedMinute = this.ConvertInt(timeFrag[1]);
                if (convertedMinute >= 0 && convertedMinute <= 59)
                {
                    minute = convertedMinute;
                }
                else
                {
                    hasError = true;
                    isValidTime = false;
                }
            }

            if (hasError)
            {
                Debug.Alert(ERROR_INVALIDDATETIME);
            }

            TaskTime tt = new TaskTime();
            DateTime dt = DateTime.Today;
            if (isValidDate || isValidTime)
            {
                dt = new DateTime(year, month, day, hour, minute, second);
            }
            tt.Format = GetFormat(isValidDate, isValidTime);
            tt.Time = dt;
            tt.IsDefault = hasError;
            return tt;
        }

        /// <summary>
        /// Converts a string to an integer
        /// </summary>
        /// <param name="str">Integer in string format</param>
        /// <returns>Return the converted numeric value. or -1 if conversion failed</returns>
        private int ConvertInt(string str)
        {
            try
            {
                return int.Parse(str);
            }
            catch
            {
                // -1 used for detecting errors
                return -1;
            }
        }


    }
}
