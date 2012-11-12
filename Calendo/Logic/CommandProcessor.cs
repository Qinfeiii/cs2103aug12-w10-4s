//@author A0091571E
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Calendo.Logic
{
    public class CommandProcessor
    {
        string inputString;
        Command command;

        TaskManager taskManager;
        CommandExtractors extractors;

        //The following is public so that it can be "read" by the UI for auto-completion
        public Dictionary<string, string[]> GetInputCommandList()
        {
            return extractors.INPUT_COMMANDS_BY_COMMAND_TYPE;
        }
        public List<Entry> TaskList { get { return taskManager.Entries; } }

        // Used to map the UI index (Key) to the actual index in the task list (Value).
        public Dictionary<int, int> IndexMap { get; set; }

        #region execution
        private void HandleCommand()
        {
            // TaskManager.ExecuteCommand(commandType, commandDate, commandTime, commandText);
            if (command.Type == null)
            {
                // Non-matching command
                return;
            }
            switch (command.Type.ToLower())
            {
                case CommandExtractors.COMMAND_TYPE_ADD:
                    ExecuteAdd();
                    break;
                case CommandExtractors.COMMAND_TYPE_REMOVE:
                    ExecuteRemove();
                    break;
                case CommandExtractors.COMMAND_TYPE_CHANGE:
                    ExecuteChange();
                    break;
                case CommandExtractors.COMMAND_TYPE_UNDO:
                    ExecuteUndo();
                    break;
                case CommandExtractors.COMMAND_TYPE_REDO:
                    ExecuteRedo();
                    break;
                case CommandExtractors.COMMAND_TYPE_EXPORT:
                    ExecuteSync();
                    break;
                case CommandExtractors.COMMAND_TYPE_IMPORT:
                    ExecuteImport();
                    break;
                default:
                    break;
            }
        }

        private void ExecuteSync()
        {
            taskManager.Export();
        }

        private void ExecuteImport()
        {
            taskManager.Import();
        }

        private void ExecuteRemove()
        {
            if (command.Text == null)
            {
                // Command without parameter
                return;
            }
            int inputValue = 0;
            int index;
            try
            {
                inputValue = Convert.ToInt32(command.Text);
                IndexMap.TryGetValue(inputValue, out index);
            }
            catch
            {
                // Invalid ID
                return;
            }
            taskManager.Remove(index);
        }

        private void ExecuteChange()
        {
            if (command.Text == null)
            {
                // Command without parameter
                return;
            }
            string[] commandTextPieces = command.Text.Split();
            int taskNumberToChange = 0;
            int inputValue;
            try
            {
                inputValue = Convert.ToInt32(commandTextPieces.First());
                IndexMap.TryGetValue(inputValue, out taskNumberToChange);
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
            taskManager.Change(taskNumberToChange, newTaskName, command.StartDate, command.StartTime, command.EndDate, command.EndTime);
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
            taskManager.Add(command.Text, command.StartDate, command.StartTime, command.EndDate, command.EndTime);
        }
        #endregion execution

        public CommandProcessor()
        {
            extractors = new CommandExtractors();
            taskManager = TaskManager.Instance;
        }

        // Public method called by UI to execute user-input command
        public void ExecuteCommand(string userInput)
        {
            Debug.Assert(userInput != null);
            inputString = userInput;
            command = new Command(userInput, ref extractors);
            HandleCommand();
        }
    }
}