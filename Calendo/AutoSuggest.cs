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
            if (isInputValid)
            {
                if (input.First() == COMMAND_INDICATOR)
                {
                    SuggestionList.Add("/add - add a new item");
                    SuggestionList.Add("/change - edit an item");
                    SuggestionList.Add("/remove - remove an item");
                    SuggestionList.Add("/undo - undo the last action");
                    SuggestionList.Add("/import - import from Google Calendar");
                    SuggestionList.Add("/sync - export to Google Calendar");
                }
                else
                {
                    SuggestionList.Add("search for " + input);
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
