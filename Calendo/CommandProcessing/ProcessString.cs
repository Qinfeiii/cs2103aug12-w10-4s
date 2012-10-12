using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calendo.CommandProcessing
{
    //TODO: Refactor by abstraction
    //TODO: LOTS of re-factoring required
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
        private string[] INPUT_COMMANDS_SEARCH = { "search", "find" };
        private string[] INPUT_COMMANDS_ADD = { "add", "a" };
        private string[] INPUT_COMMANDS_REMOVE = { "remove", "delete", "rm", "del" };
        private string[] INPUT_COMMANDS_CHANGE = { "change", "update", "modify" };
        private string[] INPUT_COMMANDS_LIST = { "list", "ls", "show" };
        private string[] INPUT_COMMANDS_UNDO = { "undo" };

        private string[] INPUT_HANDLES_DATE = { "/date" };
        private string[] INPUT_HANDLES_TIME = { "/time" };

        private Dictionary<string, string[]> DICTIONARY_COMMAND_TYPE;

        string inputString;
        List<string> inputStringWords;
        string commandType;
        string commandDate;
        string commandTime;
        string commandText;

        TaskManager taskManager;

        #region Temp for v0.1
        public TaskManager TaskManager { get { return taskManager; } }
        #endregion

        private void HandleCommand()
        {
            // TaskManager.ExecuteCommand(commandType, commandDate, commandTime, commandText);
            switch (commandType)
            {
                case COMMAND_TYPE_SEARCH:
                    ExecuteSearch();
                    break;
                case COMMAND_TYPE_ADD:
                    ExecuteAdd();
                    break;
                case COMMAND_TYPE_REMOVE:
                    ExecuteRemove();
                    break;
                case COMMAND_TYPE_CHANGE:
                    ExecuteChange();
                    break;
                case COMMAND_TYPE_LIST:
                    ExecuteList();
                    break;
                case COMMAND_TYPE_UNDO:
                    ExecuteUndo();
                    break;
                default:
                    break;
            }
        }

        private void ExecuteSearch()
        {
        }

        private void ExecuteRemove()
        {
            int index = Convert.ToInt32(commandText) - 1;
            taskManager.RemoveByIndex(index);
        }

        private void ExecuteChange()
        {
            string[] commandTextPieces = commandText.Split();
            int taskNumberToChange = Convert.ToInt32(commandTextPieces.First());
            List<string> listOfCommandTextPieces = commandTextPieces.ToList();
            listOfCommandTextPieces.RemoveAt(0);
            string newTaskName = listOfCommandTextPieces.Aggregate((x, y) => x + y);
            taskManager.Change(taskNumberToChange, newTaskName);
        }

        private void ExecuteList()
        {
        }

        private void ExecuteUndo()
        {
            taskManager.Undo();
        }

        private void ExecuteAdd()
        {
            if (commandDate == null)
            {
                taskManager.Add(commandText);
            }
            else
            {
                taskManager.Add(commandText, commandDate, commandTime);
            }
        }

        // Execution pattern: construct, then call Send
        public ProcessString(string inputString)
        {
            this.inputString = inputString;

            DICTIONARY_COMMAND_TYPE = new Dictionary<string, string[]>();
            DICTIONARY_COMMAND_TYPE.Add(COMMAND_TYPE_SEARCH, INPUT_COMMANDS_SEARCH);
            DICTIONARY_COMMAND_TYPE.Add(COMMAND_TYPE_ADD, INPUT_COMMANDS_ADD);
            DICTIONARY_COMMAND_TYPE.Add(COMMAND_TYPE_REMOVE, INPUT_COMMANDS_REMOVE);
            DICTIONARY_COMMAND_TYPE.Add(COMMAND_TYPE_CHANGE, INPUT_COMMANDS_CHANGE);
            DICTIONARY_COMMAND_TYPE.Add(COMMAND_TYPE_LIST, INPUT_COMMANDS_LIST);
            DICTIONARY_COMMAND_TYPE.Add(COMMAND_TYPE_UNDO, INPUT_COMMANDS_UNDO);

            InitialiseCommandParts();
            GetCommandParts();

            taskManager = new TaskManager();
        }

        #region Temp for v0.1
        public ProcessString()
        {
            DICTIONARY_COMMAND_TYPE = new Dictionary<string, string[]>();
            DICTIONARY_COMMAND_TYPE.Add(COMMAND_TYPE_SEARCH, INPUT_COMMANDS_SEARCH);
            DICTIONARY_COMMAND_TYPE.Add(COMMAND_TYPE_ADD, INPUT_COMMANDS_ADD);
            DICTIONARY_COMMAND_TYPE.Add(COMMAND_TYPE_REMOVE, INPUT_COMMANDS_REMOVE);
            DICTIONARY_COMMAND_TYPE.Add(COMMAND_TYPE_CHANGE, INPUT_COMMANDS_CHANGE);
            DICTIONARY_COMMAND_TYPE.Add(COMMAND_TYPE_LIST, INPUT_COMMANDS_LIST);
            DICTIONARY_COMMAND_TYPE.Add(COMMAND_TYPE_UNDO, INPUT_COMMANDS_UNDO);

            taskManager = new TaskManager();
        }

        public void ExecuteCommand(string userInput)
        {
            inputString = userInput;
            InitialiseCommandParts();
            GetCommandParts();
            HandleCommand();
        }
        #endregion

        private void InitialiseCommandParts()
        {
            commandDate = null;
            commandType = null;
            commandTime = null;
            commandText = null;
        }

        private void GetCommandParts()
        {
            CreateInputStringArray();
            ExtractAndRemoveCommandType();
            ExtractAndRemoveCommandDate();
            ExtractAndRemoveCommandTime();
            ExtractCommandText();
        }

        private void CreateInputStringArray()
        {
            inputStringWords = inputString.Trim().Split().ToList();
        }

        private void ExtractAndRemoveCommandType()
        {
            if (IsNoCommand())
            {
                commandType = COMMAND_TYPE_SEARCH;
                return;
            }

            //TODO: Abstract
            //This is the command type ENTERED by the user
            string commandTypeInput = inputStringWords.First().Substring(1);

            //TODO: Abstract
            KeyValuePair<string, string[]> commandTypePair = DICTIONARY_COMMAND_TYPE.Single(x => x.Value.Contains(commandTypeInput.ToLower()));
            commandType = commandTypePair.Key;

            inputStringWords.RemoveAt(0);
        }

        private void ExtractAndRemoveCommandDate()
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
        private void ExtractAndRemoveCommandTime()
        {
            int timeIndex = inputStringWords.FindIndex(x => INPUT_HANDLES_TIME.Contains(x));

            if (timeIndex >= 0)
            {
                if (inputStringWords.Count > timeIndex + 1)//If next word exists
                    //Change to +2 if AM/PM is to be expected
                {
                    string timeValue = inputStringWords[timeIndex + 1];
                    //string timeAMPM = inputStringWords[timeIndex + 2];

                    //TODO: Process time (alternatie style: keep taking words until next handle)

                    //commandTime = timeValue + " " + timeAMPM;
                    commandTime = timeValue;

                    // Remove time word(s)
                    // ORDER MATTERS HERE!!!
                    //inputStringWords.RemoveAt(timeIndex + 2);
                    inputStringWords.RemoveAt(timeIndex + 1);
                }

                //Remove handle
                inputStringWords.RemoveAt(timeIndex);
            }
        }

        private void ExtractCommandText()
        {
            string separator = " ";
            commandText = inputStringWords.Aggregate((first, rest) => first + separator + rest);
        }

        private Boolean IsNoCommand()
        {
            return !inputStringWords.First().StartsWith("/");
        }
    }
}
