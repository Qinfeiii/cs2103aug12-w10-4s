using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Calendo.AutoSuggest
{
    class AutoSuggest
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
                string commandDescription = "no description";
                string commandInstruction = null;
                string[] commandAliases;

                aliasDictionary.TryGetValue(currentCommand, out commandAliases);

                switch (currentCommand)
                {
                    case "add":
                        commandDescription = "add a new item";
                        commandInstruction = "[description] /date [DD/MM] /time [HH:MM]";
                        break;
                    case "change":
                        commandDescription = "edit an item";
                        commandInstruction = "[number] [description]";
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

            SuggestionList = new List<AutoSuggestEntry>();
        }

        public void SetSuggestions(string input)
        {
            SuggestionList.Clear();
            bool isInputValid = input.Length > 0;

            string[] inputWords = input.Split();
            string inputCommand = inputWords[0];

            if (isInputValid && input.First() == COMMAND_INDICATOR)
            {
                if (inputWords.Length == 1)
                {
                    // Only a command has been entered.
                    IEnumerable<AutoSuggestEntry> commandMatches = MasterList.Where(
                        delegate(AutoSuggestEntry entry)
                        {
                            bool isEntryMaster = entry.IsMaster;
                            bool isCommandMatch = entry.Command.Contains(inputCommand);
                            bool isAliasesMatch = false;

                            if (entry.Aliases != null)
                            {
                                foreach (string alias in entry.Aliases)
                                {
                                    if (isAliasesMatch = alias.StartsWith(inputCommand))
                                    {
                                        break;
                                    }
                                }
                            }

                            return entry.IsMaster && (isCommandMatch || isAliasesMatch);
                        });

                    SuggestionList = new List<AutoSuggestEntry>(commandMatches);
                }
                else if (inputWords.Length > 1)
                {
                    // Command has been entered. Show parameter suggestions.
                    SuggestionList = new List<AutoSuggestEntry>(MasterList.Where(o => o.Type == EntryType.DETAIL && o.Command.Equals(inputCommand)));
                }
            }
        }
    }
}
