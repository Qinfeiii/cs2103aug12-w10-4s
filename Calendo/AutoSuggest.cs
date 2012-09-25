using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Calendo
{
    class AutoSuggest : INotifyPropertyChanged
    {
        private const string COMMAND_INDICATOR = "/";

        public ObservableCollection<string> SuggestionList { get; set; }

        public AutoSuggest()
        {
            SuggestionList = new ObservableCollection<string>();
        }

        public void SetSuggestions(string input)
        {
            SuggestionList.Clear();
            if (input == COMMAND_INDICATOR)
            {
                SuggestionList = new ObservableCollection<string>();
                SuggestionList.Add("/add - add a new item");
                SuggestionList.Add("/change - edit an item");
                SuggestionList.Add("/remove - remove an item");
                SuggestionList.Add("/undo - undo the last action");
                SuggestionList.Add("/import - import from Google Calendar");
                SuggestionList.Add("/sync - export to Google Calendar");
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
