using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Calendo
{
    class AutoSuggest : INotifyPropertyChanged
    {
        public const char COMMAND_INDICATOR = '/';

        public List<string> SuggestionList { get; set; }

        public AutoSuggest()
        {
            SuggestionList = new List<string>();
        }

        public void SetSuggestions(string input)
        {
            SuggestionList.Clear();
            bool isInputValid = input.Length > 0;
            if (isInputValid && input.First() == COMMAND_INDICATOR)
            {
                SuggestionList.Add("/add - add a new item");
                SuggestionList.Add("/change - edit an item");
                SuggestionList.Add("/remove - remove an item");
                SuggestionList.Add("/undo - undo the last action");
                SuggestionList.Add("/redo - revert an undone action");
                SuggestionList.Add("/sync - synchronize with Google Calendar");
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
