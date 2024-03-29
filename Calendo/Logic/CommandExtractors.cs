﻿//@author A0091571E
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calendo.Logic
{
    public class CommandExtractors
    {
        #region constants
        // This is the list of "actual" command types identified by the program
        public const string COMMAND_TYPE_ADD = "add";
        public const string COMMAND_TYPE_REMOVE = "remove";
        public const string COMMAND_TYPE_CHANGE = "change";
        public const string COMMAND_TYPE_UNDO = "undo";
        public const string COMMAND_TYPE_REDO = "redo";
        public const string COMMAND_TYPE_EXPORT = "export";
        public const string COMMAND_TYPE_IMPORT = "import";

        // This is the list of user-inputs the program can handle and process as a "proper" command
        private readonly string[] INPUT_COMMANDS_ADD = { "/add", "/a", "/+" };
        private readonly string[] INPUT_COMMANDS_REMOVE = { "/remove", "/delete", "/rm", "/del", "/-" };
        private readonly string[] INPUT_COMMANDS_CHANGE = { "/change", "/update", "/modify", "/!" };
        private readonly string[] INPUT_COMMANDS_UNDO = { "/undo" };
        private readonly string[] INPUT_COMMANDS_REDO = { "/redo" };
        private readonly string[] INPUT_COMMANDS_SYNC = { "/export" }; // [temporary substitute for demo]
        private readonly string[] INPUT_COMMANDS_IMPORT = { "/import" };
        private readonly string INPUT_COMMAND_EMPTY = "/";

        // If only one date-time is given, it is defined as the start, not the end
        private readonly string[] INPUT_HANDLES_START_DATE = { "/date", "/startdate" };
        private readonly string[] INPUT_HANDLES_START_TIME = { "/time", "/starttime" };
        private readonly string[] INPUT_HANDLES_END_DATE = { "/enddate" };
        private readonly string[] INPUT_HANDLES_END_TIME = { "/endtime" };
        #endregion

        private List<string> VALID_INPUT_COMMANDS;
        // This maps the recognised user-input commands to their "proper" command type
        public Dictionary<string, string[]> INPUT_COMMANDS_BY_COMMAND_TYPE { get; private set; }

        private List<string> inputStringWords;

        public void Extract(string userInput,
            ref string commandType,
            ref string commandStartDate,
            ref string commandStartTime,
            ref string commandEndDate,
            ref string commandEndTime,
            ref string commandText)
        {
            inputStringWords = userInput.Trim().Split().ToList();
            ExtractAndRemoveCommandType(ref commandType);
            ExtractAndRemoveCommandStartDate(ref commandStartDate);
            ExtractAndRemoveCommandEndDate(ref commandEndDate);
            ExtractAndRemoveCommandStartTime(ref commandStartTime);
            ExtractAndRemoveCommandEndTime(ref commandEndTime);
            ExtractCommandText(ref commandText);
        }

        public CommandExtractors()
        {
            INPUT_COMMANDS_BY_COMMAND_TYPE = new Dictionary<string, string[]>();
            INPUT_COMMANDS_BY_COMMAND_TYPE.Add(COMMAND_TYPE_ADD, INPUT_COMMANDS_ADD);
            INPUT_COMMANDS_BY_COMMAND_TYPE.Add(COMMAND_TYPE_REMOVE, INPUT_COMMANDS_REMOVE);
            INPUT_COMMANDS_BY_COMMAND_TYPE.Add(COMMAND_TYPE_CHANGE, INPUT_COMMANDS_CHANGE);
            INPUT_COMMANDS_BY_COMMAND_TYPE.Add(COMMAND_TYPE_UNDO, INPUT_COMMANDS_UNDO);
            INPUT_COMMANDS_BY_COMMAND_TYPE.Add(COMMAND_TYPE_REDO, INPUT_COMMANDS_REDO);
            INPUT_COMMANDS_BY_COMMAND_TYPE.Add(COMMAND_TYPE_EXPORT, INPUT_COMMANDS_SYNC);
            INPUT_COMMANDS_BY_COMMAND_TYPE.Add(COMMAND_TYPE_IMPORT, INPUT_COMMANDS_IMPORT);

            VALID_INPUT_COMMANDS = new List<string>();
            foreach (string[] commands in INPUT_COMMANDS_BY_COMMAND_TYPE.Values)
            {
                VALID_INPUT_COMMANDS.AddRange(commands);
            }
            /*
            VALID_INPUT_COMMANDS.AddRange(INPUT_COMMANDS_ADD);
            VALID_INPUT_COMMANDS.AddRange(INPUT_COMMANDS_REMOVE);
            VALID_INPUT_COMMANDS.AddRange(INPUT_COMMANDS_CHANGE);
            VALID_INPUT_COMMANDS.AddRange(INPUT_COMMANDS_UNDO);
            VALID_INPUT_COMMANDS.AddRange(INPUT_COMMANDS_REDO);
            VALID_INPUT_COMMANDS.AddRange(INPUT_COMMANDS_SYNC);
            VALID_INPUT_COMMANDS.AddRange(INPUT_COMMANDS_IMPORT);
             * */
        }

        private void ExtractAndRemoveCommandType(ref string commandType)
        {
            // By default, the program interprets a query as an empty command
            if (IsNoCommand())
            {
                commandType = COMMAND_TYPE_ADD;
                return;
            }

            // If nothing has been entered, return
            if (IsEmptyList(inputStringWords))
                return;

            //This is the command type ENTERED by the user
            string commandTypeInput = inputStringWords.First();

            // If command is empty, return
            if (IsEmptyCommand(commandTypeInput))
                return;

            //Extract actual command type from input
            //For example, if user input was "/remove", "/delete", "/rm" or "/del",
            //The command type to be processed is "remove"
            if (IsValidCommand(commandTypeInput))
                GetCommandType(commandTypeInput, ref commandType);

            inputStringWords.RemoveAt(0);
        }

        private void ExtractAndRemoveCommandStartDate(ref string commandStartDate)
        {
            commandStartDate = RemoveAndReturnCommandDate(INPUT_HANDLES_START_DATE);
        }

        private void ExtractAndRemoveCommandEndDate(ref string commandEndDate)
        {
            commandEndDate = RemoveAndReturnCommandDate(INPUT_HANDLES_END_DATE);
        }

        private void ExtractAndRemoveCommandStartTime(ref string commandStartTime)
        {
            commandStartTime = RemoveAndReturnCommandTime(INPUT_HANDLES_START_TIME);
        }

        private void ExtractAndRemoveCommandEndTime(ref string commandEndTime)
        {
            commandEndTime = RemoveAndReturnCommandTime(INPUT_HANDLES_END_TIME);
        }

        private void ExtractCommandText(ref string commandText)
        {
            string separator = " ";
            commandText = String.Join(separator, inputStringWords);
        }

        #region helpers
        private string RemoveAndReturnCommandDate(string[] dateInputHandles)
        {
            string dateWord = null;

            // Process the presence of the given date input handle
            int dateIndex = inputStringWords.FindIndex(x => dateInputHandles.Contains(x.ToLower()));
            if (IsInvalidIndex(dateIndex))
                return null;

            // Perform extraction and removal
            if (inputStringWords.Count > dateIndex + 1) //If the next string exists
            {
                // Get date word(s) from input
                dateWord = inputStringWords[dateIndex + 1];
                // Remove date word(s)
                inputStringWords.RemoveAt(dateIndex + 1);
            }
            //Remove handle
            inputStringWords.RemoveAt(dateIndex);

            return dateWord;
        }

        // Expecting time as: HH:MM ["AM"/"PM"]
        private string RemoveAndReturnCommandTime(string[] timeInputHandles)
        {
            string timeValue = null;
            int timeIndex = inputStringWords.FindIndex(x => timeInputHandles.Contains(x.ToLower()));

            if (IsInvalidIndex(timeIndex))
                return null;

            if (inputStringWords.Count > timeIndex + 1)
            {
                timeValue = inputStringWords[timeIndex + 1];
                // Handle optional AM/PM
                bool hasAMPM = false;
                if (inputStringWords.Count > timeIndex + 2)
                {
                    string timeAMPM = inputStringWords[timeIndex + 2].ToUpper();
                    // Only add AM/PM if it really is AM/PM
                    if (timeAMPM == "PM" || timeAMPM == "AM")
                    {
                        timeValue = timeValue + " " + timeAMPM;
                        hasAMPM = true;
                    }
                }
                // Remove time value
                inputStringWords.RemoveAt(timeIndex + 1);
                if (hasAMPM)
                {
                    // Remove AM/PM
                    inputStringWords.RemoveAt(timeIndex + 1);
                }

            }

            //Remove handle
            inputStringWords.RemoveAt(timeIndex);

            return timeValue;
        }

        private void GetCommandType(string commandTypeInput, ref string commandType)
        {
            KeyValuePair<string, string[]> commandTypePair = INPUT_COMMANDS_BY_COMMAND_TYPE.Single(x => x.Value.Contains(commandTypeInput.ToLower()));
            commandType = commandTypePair.Key;
        }
        private Boolean IsNoCommand()
        {
            return !inputStringWords.First().StartsWith("/");
        }

        private bool IsEmptyList(List<string> inputList)
        {
            return inputList.Count == 0;
        }
        private bool IsEmptyCommand(string commandTypeInput)
        {
            return commandTypeInput == INPUT_COMMAND_EMPTY;
        }
        private bool IsValidCommand(string commandTypeInput)
        {
            return VALID_INPUT_COMMANDS.Contains(commandTypeInput.ToLower());
        }

        private static bool IsInvalidIndex(int dateIndex)
        {
            return dateIndex < 0;
        }
        #endregion
    }
}
