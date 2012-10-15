using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calendo.CommandProcessing
{
    //TODO: Refactor by abstraction
    //TODO: LOTS of re-factoring required
    class CommandProcessor
    {
        #region constants
        private const string COMMAND_TYPE_SEARCH = "search";
        private const string COMMAND_TYPE_ADD = "add";
        private const string COMMAND_TYPE_REMOVE = "remove";
        private const string COMMAND_TYPE_CHANGE = "change";
        private const string COMMAND_TYPE_LIST = "list";
        private const string COMMAND_TYPE_UNDO = "undo";
        private const string COMMAND_TYPE_REDO = "redo";

        //TODO: Ideally, NOCOMMAND should be search in autosuggest mode,
        //      and when the user presses enter, treat it as ADD
        private string[] INPUT_COMMANDS_SEARCH = { "/search", "/find" };
        private string[] INPUT_COMMANDS_ADD = { "/add", "/a" };
        private string[] INPUT_COMMANDS_REMOVE = { "/remove", "/delete", "/rm", "/del" };
        private string[] INPUT_COMMANDS_CHANGE = { "/change", "/update", "/modify" };
        private string[] INPUT_COMMANDS_LIST = { "/list", "/ls", "/show" };
        private string[] INPUT_COMMANDS_UNDO = { "/undo" };
        private string[] INPUT_COMMANDS_REDO = { "/redo" };
        private string INPUT_COMMAND_EMPTY = "/";

        private string[] INPUT_HANDLES_DATE = { "/date" };
        private string[] INPUT_HANDLES_TIME = { "/time" };
        #endregion

        private List<string> validInputCommmandList;
        private Dictionary<string, string[]> DICTIONARY_COMMAND_TYPE;

        string inputString;
        List<string> inputStringWords;
        string commandType;
        string commandDate;
        string commandTime;
        string commandText;

        TaskManager taskManager;

        #region Temp for v0.1
        public List<Calendo.Data.Entry> TaskList { get { return taskManager.Entries; } }
        #endregion

        #region execution
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
                case COMMAND_TYPE_REDO:
                    ExecuteRedo();
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
            if (commandText == null)
            {
                // Command without parameter
                return;
            }
            int index = 0;
            try
            {
                index = Convert.ToInt32(commandText) - 1;
            }
            catch
            {
                // Invalid ID
                return;
            }
            taskManager.RemoveByIndex(index);
        }

        private void ExecuteChange()
        {
            if (commandText == null)
            {
                // Command without parameter
                return;
            }
            string[] commandTextPieces = commandText.Split();
            int taskNumberToChange = 0;
            try
            {
                taskNumberToChange = Convert.ToInt32(commandTextPieces.First());
            }
            catch
            {
                // Invalid id
                return;
            }
            List<string> listOfCommandTextPieces = commandTextPieces.ToList();
            listOfCommandTextPieces.RemoveAt(0);
            string newTaskName = "";
            if (listOfCommandTextPieces.Count > 0)
            {
                newTaskName = listOfCommandTextPieces.Aggregate((x, y) => x + " " + y);
            }
            taskManager.Change(taskNumberToChange, newTaskName, commandDate, commandTime, "", "");
        }

        private void ExecuteList()
        {
        }

        private void ExecuteUndo()
        {
            taskManager.Undo();
        }

        private void ExecuteRedo()
        {
            taskManager.Redo();
        }

        private void ExecuteAdd()
        {
            /*
            if (commandDate == null)
            {
                taskManager.Add(commandText);
            }
            else
            {
                taskManager.Add(commandText, commandDate, commandTime);
            }
             * */
            taskManager.Add(commandText, commandDate, commandTime);
        }
        #endregion execution

        // Execution pattern: construct, then call Send
        public CommandProcessor(string inputString)
        {
            this.inputString = inputString;

            DICTIONARY_COMMAND_TYPE = new Dictionary<string, string[]>();
            DICTIONARY_COMMAND_TYPE.Add(COMMAND_TYPE_SEARCH, INPUT_COMMANDS_SEARCH);
            DICTIONARY_COMMAND_TYPE.Add(COMMAND_TYPE_ADD, INPUT_COMMANDS_ADD);
            DICTIONARY_COMMAND_TYPE.Add(COMMAND_TYPE_REMOVE, INPUT_COMMANDS_REMOVE);
            DICTIONARY_COMMAND_TYPE.Add(COMMAND_TYPE_CHANGE, INPUT_COMMANDS_CHANGE);
            DICTIONARY_COMMAND_TYPE.Add(COMMAND_TYPE_LIST, INPUT_COMMANDS_LIST);
            DICTIONARY_COMMAND_TYPE.Add(COMMAND_TYPE_UNDO, INPUT_COMMANDS_UNDO);
            DICTIONARY_COMMAND_TYPE.Add(COMMAND_TYPE_REDO, INPUT_COMMANDS_REDO);

            InitialiseCommandParts();
            GetCommandParts();

            taskManager = new TaskManager();
        }

        #region Temp for v0.1
        public CommandProcessor()
        {
            DICTIONARY_COMMAND_TYPE = new Dictionary<string, string[]>();
            DICTIONARY_COMMAND_TYPE.Add(COMMAND_TYPE_SEARCH, INPUT_COMMANDS_SEARCH);
            DICTIONARY_COMMAND_TYPE.Add(COMMAND_TYPE_ADD, INPUT_COMMANDS_ADD);
            DICTIONARY_COMMAND_TYPE.Add(COMMAND_TYPE_REMOVE, INPUT_COMMANDS_REMOVE);
            DICTIONARY_COMMAND_TYPE.Add(COMMAND_TYPE_CHANGE, INPUT_COMMANDS_CHANGE);
            DICTIONARY_COMMAND_TYPE.Add(COMMAND_TYPE_LIST, INPUT_COMMANDS_LIST);
            DICTIONARY_COMMAND_TYPE.Add(COMMAND_TYPE_UNDO, INPUT_COMMANDS_UNDO);
            DICTIONARY_COMMAND_TYPE.Add(COMMAND_TYPE_REDO, INPUT_COMMANDS_REDO);

            validInputCommmandList = new List<string>();
            validInputCommmandList.AddRange(INPUT_COMMANDS_SEARCH);
            validInputCommmandList.AddRange(INPUT_COMMANDS_ADD);
            validInputCommmandList.AddRange(INPUT_COMMANDS_REMOVE);
            validInputCommmandList.AddRange(INPUT_COMMANDS_CHANGE);
            validInputCommmandList.AddRange(INPUT_COMMANDS_LIST);
            validInputCommmandList.AddRange(INPUT_COMMANDS_UNDO);
            validInputCommmandList.AddRange(INPUT_COMMANDS_REDO);

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
            // By default, the program interprets a query as a "search" command
            if (IsNoCommand())
            {
                commandType = COMMAND_TYPE_SEARCH;
                return;
            }

            // If nothing has been entered, return
            if (IsEmptyList(inputStringWords))
                return;

            //TODO: Abstract
            //This is the command type ENTERED by the user
            string commandTypeInput = inputStringWords.First();

            // If command is empty, return
            if (IsEmptyCommand(commandTypeInput))
                return;

            //TODO: Abstract
            //Extract actual command type from input
            //For example, if user input was "/remove", "/delete", "/rm" or "/del",
            //The command type to be processed is "remove"
            if (IsValidCommand(commandTypeInput))
                GetCommandType(commandTypeInput);

            inputStringWords.RemoveAt(0);
        }

        private void GetCommandType(string commandTypeInput)
        {

            KeyValuePair<string, string[]> commandTypePair = DICTIONARY_COMMAND_TYPE.Single(x => x.Value.Contains(commandTypeInput.ToLower()));
            commandType = commandTypePair.Key;
        }

        private bool IsValidCommand(string commandTypeInput)
        {
            //return DICTIONARY_COMMAND_TYPE.Values.Any(x => commandTypeInput.ToLower() == x);
            return validInputCommmandList.Contains(commandTypeInput.ToLower());
        }

        private bool IsEmptyCommand(string commandTypeInput)
        {
            return commandTypeInput == INPUT_COMMAND_EMPTY;
        }

        private bool IsEmptyList(List<string> inputList)
        {
            return inputList.Count == 0;
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
                if (inputStringWords.Count > timeIndex + 1)
                {
                    string timeValue = inputStringWords[timeIndex + 1];
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

                    commandTime = timeValue;

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
            }
        }

        private void ExtractCommandText()
        {
            string separator = " ";
            if (inputStringWords.Count > 0)
                commandText = inputStringWords.Aggregate((first, rest) => first + separator + rest);
        }

        private Boolean IsNoCommand()
        {
            return !inputStringWords.First().StartsWith("/");
        }
    }
}
