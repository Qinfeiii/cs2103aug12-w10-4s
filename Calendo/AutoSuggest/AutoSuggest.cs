//@author Jerome
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Calendo.AutoSuggest
{
    public class AutoSuggest
    {
        public const char COMMAND_INDICATOR = '/';

        public List<AutoSuggestEntry> SuggestionList { get; set; }

        private List<AutoSuggestEntry> MasterList;

        public AutoSuggest(Dictionary<string, string[]> aliasDictionary)
        {
            IEnumerable<string> commandTypes = aliasDictionary.Keys.AsEnumerable<string>();
            MasterList = new List<AutoSuggestEntry>();

            foreach (string currentCommand in commandTypes)
            {
                AddEntryFromString(aliasDictionary, currentCommand);
            }

            SuggestionList = new List<AutoSuggestEntry>();
        }

        private void AddEntryFromString(Dictionary<string, string[]> aliasDictionary, string currentCommand)
        {
            string commandDescription = "no description";
            string commandInstruction = null;
            string[] commandAliases;

            aliasDictionary.TryGetValue(currentCommand, out commandAliases);

            switch (currentCommand)
            {
                case "add":
                    commandDescription = "add a new item";
                    commandInstruction = "[description] /date [DD/MM] /time [HH:MM] /enddate [DD/MM] /endtime [HH:MM]";
                    break;
                case "change":
                    commandDescription = "edit an item";
                    commandInstruction = "[number] [description] /date [DD/MM] /time [HH:MM] /enddate [DD/MM] /endtime [HH:MM]";
                    break;
                case "remove":
                    commandDescription = "remove an item";
                    commandInstruction = "[number]";
                    break;
                case "undo":
                    commandDescription = "undo the last action";
                    break;
                case "redo":
                    commandDescription = "revert an undone action";
                    break;
                case "sync":
                    commandDescription = "synchronize with Google Calendar";
                    break;
                case "import":
                    commandDescription = "import tasks from Google Calendar";
                    break;
                case "export":
                    commandDescription = "export tasks to Google Calendar";
                    break;
            }

            AutoSuggestEntry mainEntry = new AutoSuggestEntry("/" + currentCommand, commandDescription, EntryType.MASTER, commandAliases);
            MasterList.Add(mainEntry);

            AutoSuggestEntry detailEntry;
            if (commandInstruction != null)
            {
                foreach (string alias in commandAliases)
                {
                    detailEntry = new AutoSuggestEntry(alias, commandInstruction, EntryType.DETAIL, null);
                    MasterList.Add(detailEntry);
                }
            }
        }

        public void SetSuggestions(string input)
        {
            SuggestionList.Clear();
            bool isInputValid = input.Length > 0;

            if (isInputValid)
            {
                GenerateSuggestionsFromInput(input);
            }
        }

        private void GenerateSuggestionsFromInput(string input)
        {
            string[] inputWords = input.Split();
            string inputCommand = inputWords[0];

            bool isInputCommand = input.First() == COMMAND_INDICATOR;

            if (isInputCommand)
            {
                if (inputWords.Length == 1)
                {
                    MatchInputToCommandSuggestion(inputCommand);
                }
                else if (inputWords.Length > 1)
                {
                    MatchInputToInstruction(inputCommand);
                }
            }
        }

        private void MatchInputToInstruction(string inputCommand)
        {
            // Full command has been entered. Show parameter suggestions.
            SuggestionList = new List<AutoSuggestEntry>(MasterList.Where(o => o.Type == EntryType.DETAIL && o.Command.Equals(inputCommand)));
        }

        private void MatchInputToCommandSuggestion(string inputCommand)
        {
            // User is entering a command.
            IEnumerable<AutoSuggestEntry> commandMatches = MasterList.Where(
                delegate(AutoSuggestEntry entry)
                {
                    bool isEntryMaster = entry.IsMaster;
                    bool isCommandMatch = entry.Command.Contains(inputCommand);
                    bool isAliasesMatch = CheckAliasesForCommand(inputCommand, entry);

                    return entry.IsMaster && (isCommandMatch || isAliasesMatch);
                });

            SuggestionList = new List<AutoSuggestEntry>(commandMatches);
        }

        public bool CheckAliasesForCommand(string inputCommand, AutoSuggestEntry entry)
        {
            if (entry.Aliases == null)
            {
                return false;
            }

            foreach (string currentAlias in entry.Aliases)
            {
                if (currentAlias.StartsWith(inputCommand))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
