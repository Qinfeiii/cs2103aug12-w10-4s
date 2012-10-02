using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calendo.CommandProcessing
{
    //TODO: Refactor by abstraction
    class ProcessString
    {
        private const string COMMAND_TYPE_SEARCH = "search";
        private const string COMMAND_TYPE_ADD = "add";
        private const string COMMAND_TYPE_REMOVE = "remove";
        private const string COMMAND_TYPE_CHANGE = "change";
        private const string COMMAND_TYPE_LIST = "list";
        private const string COMMAND_TYPE_UNDO = "undo";

        //TODO: Ideally, NOCOMMAND should be search in autosuggest mode,
        //      and when the user presses enter, treat it as ADD
        private const string[] INPUT_COMMANDS_SEARCH = { "search", "find" };
        private const string[] INPUT_COMMANDS_ADD = { "add", "a" };
        private const string[] INPUT_COMMANDS_REMOVE = { "remove", "delete", "rm", "del" };
        private const string[] INPUT_COMMANDS_CHANGE = { "change", "update", "modify" };
        private const string[] INPUT_COMMANDS_LIST = { "list", "ls", "show" };
        private const string[] INPUT_COMMANDS_UNDO = { "undo" };

        private const string[] INPUT_HANDLES_DATE = { "/date" };
        private const string[] INPUT_HANDLES_TIME = { "/time" };

        private Dictionary<string, string[]> commandList;

        string inputString;
        List<string> inputStringWords;
        string commandType;
        string commandDate;
        string commandTime;
        string commandText;

        public void Send()
        {
        }

        // Execution pattern: construct, then call Send
        public ProcessString(string inputString)
        {
            this.inputString = inputString;

            commandList = new Dictionary<string, string[]>();
            commandList.Add(COMMAND_TYPE_SEARCH, INPUT_COMMANDS_SEARCH);
            commandList.Add(COMMAND_TYPE_ADD, INPUT_COMMANDS_ADD);
            commandList.Add(COMMAND_TYPE_REMOVE, INPUT_COMMANDS_REMOVE);
            commandList.Add(COMMAND_TYPE_CHANGE, INPUT_COMMANDS_CHANGE);
            commandList.Add(COMMAND_TYPE_LIST, INPUT_COMMANDS_LIST);
            commandList.Add(COMMAND_TYPE_UNDO, INPUT_COMMANDS_UNDO);

            getCommandParts();
        }

        private void getCommandParts()
        {
            createInputStringArray();
            extractAndRemoveCommandType();
            extractAndRemoveCommandDate();
            extractAndRemoveCommandTime();
            extractCommandText();
        }

        private void createInputStringArray()
        {
            inputStringWords = inputString.Trim().Split().ToList();
        }

        private void extractAndRemoveCommandType()
        {
            if (isNoCommand())
            {
                commandType = COMMAND_TYPE_SEARCH;
                return;
            }

            //TODO: Abstract
            //This is the command type ENTERED by the user
            string commandTypeInput = inputStringWords.First().Substring(1);

            //TODO: Abstract
            KeyValuePair<string, string[]> commandTypePair = commandList.Single(x => x.Value.Contains(commandTypeInput.ToLower()));
            commandType = commandTypePair.Key;

            inputStringWords.RemoveAt(0);
        }

        private void extractAndRemoveCommandDate()
        {
            int dateIndex = inputStringWords.FindIndex(x => INPUT_HANDLES_DATE.Contains(x));

            if (dateIndex >= 0)
            {
                if (inputStringWords.Count > dateIndex + 1) //If the next string exists
                {
                    // Get date word(s) from input
                    string dateWord = inputStringWords[dateIndex + 1];

                    //TODO: Process date (alternative style: keep taking words until next handle)

                    commandDate = dateWord;

                    // Remove date word(s)
                    inputStringWords.RemoveAt(dateIndex + 1);
                }

                //Remove handle
                inputStringWords.RemoveAt(dateIndex);
            }
        }

        // Expecting time as: HH:MM ["AM"/"PM"]
        private void extractAndRemoveCommandTime()
        {
            int timeIndex = inputStringWords.FindIndex(x => INPUT_HANDLES_TIME.Contains(x));

            if (timeIndex >= 0)
            {
                if (inputStringWords.Count > timeIndex + 2)//If next two words exist
                {
                    string timeValue = inputStringWords[timeIndex + 1];
                    string timeAMPM = inputStringWords[timeIndex + 2];

                    //TODO: Process time (alternatie style: keep taking words until next handle)

                    commandTime = timeValue + " " + timeAMPM;

                    // Remove time word(s)
                    // ORDER MATTERS HERE!!!
                    inputStringWords.RemoveAt(timeIndex + 2);
                    inputStringWords.RemoveAt(timeIndex + 1);
                }

                //Remove handle
                inputStringWords.RemoveAt(timeIndex);
            }
        }

        private void extractCommandText()
        {
            string separator = " ";
            commandText = inputStringWords.Aggregate((first, rest) => first + separator + rest);
        }

        private Boolean isNoCommand()
        {
            return !inputStringWords.First().StartsWith("/");
        }

        private void callCommand()
        {
        }
    }
}
