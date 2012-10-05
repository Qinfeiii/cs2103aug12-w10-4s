using System;
using System.Collections.Generic;
using System.Text;

namespace Calendo
{
    class TaskManager
    {
        private ChronicStorage storage;
        public TaskManager()
        {
            storage = new ChronicStorage();
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
        /// Add a Task
        /// </summary>
        /// <param name="description">Task Description</param>
        public void Add(string description)
        {
            Entry entry = new Entry();
            entry.Type = EntryType.FLOATING;
            entry.Description = description;
            Add(entry);
        }
        public void Add(string description, string date, string time)
        {
            DateTime startTime = this.ConvertTime(date, time);
            TimeFormat startTimeFormat = this.GetFormat(date, time);
            this.Add(description, startTime, startTimeFormat);
        }
        public void Add(string description, DateTime startTime, TimeFormat startTimeFormat)
        {
            Entry entry = new Entry();
            entry.Description = description;
            entry.StartTime = startTime;
            entry.StartTimeFormat = startTimeFormat;
            entry.Type = EntryType.DEADLINE;
            Add(entry);
        }
        public void Add(string description, string startDate, string startTime, string endDate, string endTime)
        {
            DateTime startDateTime = this.ConvertTime(startDate, startTime);
            TimeFormat startTimeFormat = this.GetFormat(startDate, startTime);
            DateTime endDateTime = this.ConvertTime(endDate, endTime);
            TimeFormat endTimeFormat = this.GetFormat(endDate, endTime);
            this.Add(description, startDateTime, startTimeFormat, endDateTime, endTimeFormat);
        }
        public void Add(string description, DateTime startTime, TimeFormat startTimeFormat, DateTime endTime, TimeFormat endTimeFormat)
        {
            Entry entry = new Entry();
            entry.Description = description;
            entry.StartTime = startTime;
            entry.StartTimeFormat = startTimeFormat;
            entry.EndTime = endTime;
            entry.EndTimeFormat = endTimeFormat;
            entry.Type = EntryType.TIMED;
            Add(entry);
        }
        private void Add(Entry entry)
        {
            storage.Entries.Add(entry);
            storage.Save();
        }

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


        public void Remove(int id)
        {
            Entry entry = this.Get(id);
            if (entry != null)
            {
                storage.Entries.Remove(entry);
                storage.Save();
            }
        }

        public Entry Get(int id)
        {
            for (int i = 0; i < storage.Entries.Count; i++)
            {
                if (storage.Entries[i].ID == id)
                {
                    return storage.Entries[i];
                }
            }
            return null;
        }

        public void Undo()
        {
            storage.Undo();
        }

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

        // For testing purposes
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

        public TimeFormat GetFormat(string date, string time)
        {
            TimeFormat newTimeFormat = TimeFormat.NONE;
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

        public DateTime ConvertTime(string date, string time)
        {
            // Date: Day/Month[/Year]
            // Time (24HR): Hour:Minute
            string[] dateFrag = date.Split(new char[] { '/' }, 2);
            string[] timeFrag = date.Split(new char[] { ':' }, 2);
            int year = DateTime.Today.Year;
            int day = 0;
            int month = 0;
            int hour = 0;
            int minute = 0;
            int second = 0;

            if (dateFrag.Length > 0)
            {
                day = this.ConvertInt(dateFrag[0]);
            }
            if (dateFrag.Length > 1)
            {
                month = this.ConvertInt(dateFrag[1]);
                int thismonth = DateTime.Today.Month;
                if (month < thismonth)
                {
                    // The month is actually before, assume referring to next year
                    year++;
                }
            }
            if (dateFrag.Length > 2)
            {
                year = this.ConvertInt(dateFrag[1]);
            }
            if (timeFrag.Length > 0)
            {
                hour = this.ConvertInt(timeFrag[0]);
            }
            if (timeFrag.Length > 1)
            {
                minute = this.ConvertInt(timeFrag[0]);
            }

            DateTime dt = new DateTime(year, day, month, hour, minute, second);
            return dt;
        }

        private int ConvertInt(string str)
        {
            try
            {
                return int.Parse(str);
            }
            catch
            {
                return -1;
            }
        }


    }
}
