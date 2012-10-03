using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <param name="Description">Task Description</param>
        public void Add(string Description)
        {
            Entry entry = new Entry();
            entry.Type = EntryType.FLOATING;
            entry.Description = Description;
            Add(entry);
        }
        public void Add(string Description, string Date, string Time)
        {
            DateTime StartTime = this.ConvertTime(Date, Time);
            TimeFormat StartTimeFormat = this.GetFormat(Date, Time);
            this.Add(Description, StartTime, StartTimeFormat);
        }
        public void Add(string Description, DateTime StartTime, TimeFormat StartTimeFormat)
        {
            Entry entry = new Entry();
            entry.Description = Description;
            entry.StartTime = StartTime;
            entry.StartTimeFormat = StartTimeFormat;
            entry.Type = EntryType.DEADLINE;
            Add(entry);
        }
        public void Add(string Description, string StartDate, string StartTime, string EndDate, string EndTime)
        {
            DateTime StartDateTime = this.ConvertTime(StartDate, StartTime);
            TimeFormat StartTimeFormat = this.GetFormat(StartDate, StartTime);
            DateTime EndDateTime = this.ConvertTime(EndDate, EndTime);
            TimeFormat EndTimeFormat = this.GetFormat(EndDate, EndTime);
            this.Add(Description, StartDateTime, StartTimeFormat, EndDateTime, EndTimeFormat);
        }
        public void Add(string Description, DateTime StartTime, TimeFormat StartTimeFormat, DateTime EndTime, TimeFormat EndTimeFormat)
        {
            Entry entry = new Entry();
            entry.Description = Description;
            entry.StartTime = StartTime;
            entry.StartTimeFormat = StartTimeFormat;
            entry.EndTime = EndTime;
            entry.EndTimeFormat = EndTimeFormat;
            entry.Type = EntryType.TIMED;
            Add(entry);
        }
        private void Add(Entry entry)
        {
            storage.Entries.Add(entry);
            storage.Save();
        }

        public void Change(int id, string Description)
        {
            Entry entry = this.Get(id);
            if (entry != null)
            {
                entry.Description = Description;
                storage.Save();
            }
        }

        public void Remove(int Id)
        {
            Entry entry = this.Get(Id);
            if (entry != null)
            {
                storage.Entries.Remove(entry);
                storage.Save();
            }
        }

        public Entry Get(int Id)
        {
            for (int i = 0; i < storage.Entries.Count; i++)
            {
                if (storage.Entries[i].ID == Id)
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
            StringBuilder CommandParameter = new StringBuilder();
            List<Command> commands = new List<Command>();
            bool FirstCommand = true;
            string CommandType = "";
            for (int i = 0; i < commandfrag.Length; i++)
            {
                if (commandfrag[i].Length > 0 && commandfrag[i][0] == '/')
                {
                    if (!FirstCommand)
                    {
                        commands.Add(new Command(CommandType, CommandParameter.ToString()));
                        CommandParameter.Clear();
                    }
                    CommandType = commandfrag[i].Substring(1);
                    FirstCommand = false;
                }
                else
                {
                    CommandParameter.Append(commandfrag[i] + " ");
                }

            }
            // Add last command
            commands.Add(new Command(CommandType, CommandParameter.ToString()));
            CommandParameter.Clear();
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

        private void ProcessCommand(Command command, string Date, string Time)
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
                    this.Remove(this.ConvertInt(command.Parameter));
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

        public TimeFormat GetFormat(string Date, string Time)
        {
            TimeFormat NewTimeFormat = TimeFormat.NONE;
            if (Date != "")
            {
                NewTimeFormat = TimeFormat.DATE;
            }
            if (Time != "")
            {
                NewTimeFormat = TimeFormat.TIME;
            }
            if (Date != "" && Time != "")
            {
                NewTimeFormat = TimeFormat.DATETIME;
            }
            return NewTimeFormat;
        }

        public DateTime ConvertTime(string date, string time)
        {
            // Date: Day/Month[/Year]
            // Time (24HR): Hour:Minute
            string[] datefrag = date.Split(new char[] { '/' }, 2);
            string[] timefrag = date.Split(new char[] { ':' }, 2);
            int year = DateTime.Today.Year;
            int day = 0;
            int month = 0;
            int hour = 0;
            int minute = 0;
            int second = 0;
            
            if (datefrag.Length > 0)
            {
                day = this.ConvertInt(datefrag[0]);
            }
            if (datefrag.Length > 1)
            {
                month = this.ConvertInt(datefrag[1]);
                int thismonth = DateTime.Today.Month;
                if (month < thismonth)
                {
                    // The month is actually before, assume referring to next year
                    year++;
                }
            }
            if (datefrag.Length > 2)
            {
                year = this.ConvertInt(datefrag[1]);
            }
            if (timefrag.Length > 0)
            {
                hour = this.ConvertInt(timefrag[0]);
            }
            if (timefrag.Length > 1)
            {
                minute = this.ConvertInt(timefrag[0]);
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
