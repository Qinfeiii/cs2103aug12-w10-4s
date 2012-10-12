using System;
using System.Collections.Generic;
using System.Text;
using Calendo.Data;

namespace Calendo
{
    class TaskTime
    {
        public TaskTime()
        {
            Format = TimeFormat.NONE;
            Time = DateTime.Today;
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
    }
    class TaskManager
    {
        private StateStorage<List<Entry>> storage;
        public TaskManager()
        {
            storage = new StateStorage<List<Entry>>("archive.txt");
            storage.Load();
        }

        /// <summary>
        /// Get the entries
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
            TaskTime startTime = this.ConvertTime(date, time);
            this.Add(description, startTime, new TaskTime());
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
        public void Add(string description, TaskTime startTime, TaskTime endTime)
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
        public void Change(int id, string description, DateTime startTime, TimeFormat startTimeFormat, DateTime endTime, TimeFormat endTimeFormat)
        {
            Entry entry = this.Get(id);
            if (entry != null)
            {
                entry.Description = description;
                entry.StartTime = startTime;
                entry.StartTimeFormat = startTimeFormat;
                entry.EndTime = endTime;
                entry.EndTimeFormat = endTimeFormat;
                entry.Type = EntryType.TIMED;
                Add(entry);
            }
        }

        /// <summary>
        /// Takes in the index of an item to remove from the Entries list, removes that item.
        /// </summary>
        /// <param name="index">The 0-indexed index of the item to be removed.</param>
        public void RemoveByIndex(int index)
        {
            if (index >= 0 && index < Entries.Count)
            {
                storage.Entries.RemoveAt(index);
                storage.Save();
            }
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
            return null;
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

        //Changed this execution pattern
        public void PerformCommand(string command)
        {
            ProcessCommands(ReadCommand(command));
        }

        public static void ExecuteCommand(string commandType, string commandDate, string commandTime, string commandText)
        {
            //Do something with this stuff
        }

        // Working stub for CP
        public List<Command> ReadCommand(string command)
        {
            string[] commandfrag = command.Split(new char[] { ' ' });
            StringBuilder commandParameter = new StringBuilder();
            List<Command> commands = new List<Command>();
            bool firstCommand = true;
            string commandType = "";
            for (int i = 0; i < commandfrag.Length; i++)
            {
                if (commandfrag[i].Length > 0 && commandfrag[i][0] == '/')
                {
                    if (!firstCommand)
                    {
                        commands.Add(new Command(commandType, commandParameter.ToString()));
                        commandParameter.Clear();
                    }
                    commandType = commandfrag[i].Substring(1);
                    firstCommand = false;
                }
                else
                {
                    commandParameter.Append(commandfrag[i] + " ");
                }

            }
            // Add last command
            commands.Add(new Command(commandType, commandParameter.ToString()));
            commandParameter.Clear();
            return commands;
        }

        // Working stub for CP
        public void ProcessCommands(List<Command> commands)
        {
            string date = "";
            string time = "";
            for (int i = 0; i < commands.Count; i++)
            {
                if (commands[i].Type == "date")
                {
                    date = commands[i].Parameter;
                    continue;
                }
                if (commands[i].Type == "time")
                {
                    time = commands[i].Parameter;
                    continue;
                }
                ProcessCommand(commands[i], date, time);
            }
        }

        // Working stub for CP
        private void ProcessCommand(Command command, string date, string time)
        {
            switch (command.Type)
            {
                case "add":
                    this.Add(command.Parameter);
                    break;
                case "change":
                    // STUB
                    break;
                case "remove":
                    int index = this.ConvertInt(command.Parameter) - 1;
                    this.RemoveByIndex(index);
                    break;
                case "undo":
                    this.Undo();
                    break;
                case "sync":
                    this.Sync();
                    break;
                case "import":
                    this.Import();
                    break;
                case "redo":
                    this.Redo();
                    break;
            }
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
        /// <param name="date">Date</param>
        /// <param name="time">Time</param>
        /// <returns>Returns TimeFormat value</returns>
        public TimeFormat GetFormat(string date, string time)
        {
            TimeFormat newTimeFormat = TimeFormat.NONE;
            date = DefaultString(date);
            time = DefaultString(time);

            if (date != "")
            {
                newTimeFormat = TimeFormat.DATE;
            }
            if (time != "")
            {
                newTimeFormat = TimeFormat.TIME;
            }
            if (date != "" && time != "")
            {
                newTimeFormat = TimeFormat.DATETIME;
            }
            return newTimeFormat;
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
        /// Converts a string to a processable type
        /// </summary>
        /// <param name="str">String to be converted</param>
        /// <returns></returns>
        private string DefaultString(string str)
        {
            if (str == null)
            {
                return "";
            }
            else
            {
                return str;
            }
        }

        /// <summary>
        /// Is it a leap year?
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
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
        public TaskTime ConvertTime(string date, string time)
        {
            date = DefaultString(date);
            time = DefaultString(time);

            bool isValidDate = true;
            bool isValidTime = true;
            bool hasError = false;


            DateTime defaultDateTime = DateTime.Today;

            // Defaults
            int year = DateTime.Today.Year;
            int day = 1;
            int month = 1;
            int hour = 0;
            int minute = 0;
            int second = 0;

            // Date: Day/Month[/Year]
            string[] dateFrag = date.Split(new char[] { '/' }, 2);
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
                    // TODO: Catch most cases, but not all
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
            string[] timeMeta = time.Split(new char[] { ' ' }, 2);
            string[] timeFrag = timeMeta[0].Split(new char[] { ':' }, 2);
            
            // Only used by 12 hour format
            // If both are false, 24 hour format is used
            bool isAM = false;
            bool isPM = false;

            // Handle PM
            if (timeMeta.Length > 1)
            {
                if (timeMeta[1] == "PM")
                {
                    isPM = true;
                }
                if (timeMeta[1] == "AM")
                {
                    isAM = true;
                }
            }

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
                if (convertedHour >= 0 && convertedHour <= 24)
                {
                    hour = convertedHour;
                    if (hour == 12 && isAM)
                    {
                        hour = 0;
                    }
                    if (isPM)
                    {
                        if (hour != 12)
                        {
                            hour += 12;
                        }
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
                Debug.Alert("Specified Date or Time is invalid, and will be ignored");
            }

            TaskTime tt = new TaskTime();
            DateTime dt = new DateTime(year, day, month, hour, minute, second);
            tt.Format = GetFormat(isValidDate, isValidTime);
            tt.Time = dt;
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
