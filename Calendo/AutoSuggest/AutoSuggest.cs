using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Calendo.AutoSuggest
{
    class AutoSuggest : INotifyPropertyChanged
    {
        public const char COMMAND_INDICATOR = '/';

        public List<AutoSuggestEntry> SuggestionList { get; set; }

        private List<AutoSuggestEntry> MasterList; 

        public AutoSuggest()
        {
            MasterList = new List<AutoSuggestEntry>
                                 {
                                     new AutoSuggestEntry("/add", "add a new item", EntryType.MASTER),
                                     new AutoSuggestEntry("/change", "edit an item", EntryType.MASTER),
                                     new AutoSuggestEntry("/remove", "remove an item", EntryType.MASTER),
                                     new AutoSuggestEntry("/undo", "undo the last action", EntryType.MASTER),
                                     new AutoSuggestEntry("/redo", "revert an undone action", EntryType.MASTER),
                                     new AutoSuggestEntry("/sync", "synchronize with Google Calendar", EntryType.MASTER),
                                     
                                     new AutoSuggestEntry("/add", "[description] /date [DD/MM] /time [HH:MM]", EntryType.DETAIL),
                                     new AutoSuggestEntry("/change", "[number] [description]", EntryType.DETAIL),
                                     new AutoSuggestEntry("/remove", "[number]", EntryType.DETAIL)
                                 };
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
                if(inputWords.Length == 1)
                {
                    // Only a command has been entered.
                    SuggestionList = new List<AutoSuggestEntry>(MasterList.Where(o => o.Type == EntryType.MASTER && o.Command.Contains(inputCommand)));
                }
                else if (inputWords.Length > 1)
                {
                    // Command has been entered. Show parameter suggestions.
                    SuggestionList = new List<AutoSuggestEntry>(MasterList.Where(o => o.Type == EntryType.DETAIL && o.Command.Contains(inputCommand)));
                }
            }

            OnPropertyChanged("SuggestionList");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventArgs args = new PropertyChangedEventArgs(propertyName);
            PropertyChangedEventHandler changed = PropertyChanged;
            if (changed != null)
            {
                changed(this, args);
            }
        }
    }
}
