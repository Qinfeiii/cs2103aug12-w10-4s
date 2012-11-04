//@author Jerome
using System.Collections.Generic;
using Calendo.AutoSuggest;
using Calendo.Logic;
using System.ComponentModel;

namespace Calendo
{
    class UiViewModel : INotifyPropertyChanged
    {
        private AutoSuggest.AutoSuggest AutoSuggestSystem { get; set; }
        private CommandProcessor CommandProcessor { get; set; }

        public List<AutoSuggestEntry> SuggestionList { get { return AutoSuggestSystem.SuggestionList; } }
        public Dictionary<int, Entry> TaskList { get; set; }

        public int AutoSuggestRow
        {
            get
            {
                bool isOnlyOneSuggestion = SuggestionList.Count == 1;
                bool isSuggestionDetail = (SuggestionList.Count > 0) && !SuggestionList[0].IsMaster;
                return isOnlyOneSuggestion && isSuggestionDetail ? 2 : 3;
            }
        }

        public UiViewModel()
        {
            CommandProcessor = new CommandProcessor();
            AutoSuggestSystem = new AutoSuggest.AutoSuggest(CommandProcessor.GetInputCommandList());
            TaskManager.UpdateHandler updateHandler = UpdateItemsList;
            TaskManager.Instance.AddSubscriber(updateHandler);
            UpdateItemsList();
        }

        public void ExecuteCommand(string command)
        {
            CommandProcessor.ExecuteCommand(command);
            UpdateItemsList();
        }

        public void SetSuggestions(string input)
        {
            AutoSuggestSystem.SetSuggestions(input);
            OnPropertyChanged("SuggestionList");
            OnPropertyChanged("AutoSuggestRow");
        }

        private void UpdateItemsList()
        {
            List<Entry> entries = new List<Entry>(CommandProcessor.TaskList);
            entries.Sort(TaskListSorter);

            int count = 1;
            Dictionary<int, Entry> itemDictionary = new Dictionary<int, Entry>();
            Dictionary<int, int> indexMap = new Dictionary<int, int>();

            foreach (Entry currentEntry in entries)
            {
                int originalIndex = CommandProcessor.TaskList.IndexOf(currentEntry) + 1;
                indexMap.Add(count, originalIndex);
                itemDictionary.Add(count, currentEntry);
                count++;
            }

            TaskList = itemDictionary;
            CommandProcessor.IndexMap = indexMap;
            OnPropertyChanged("TaskList");
        }

        private int TaskListSorter(Entry first, Entry second)
        {
            // We want items sorted by Overdue -> Active -> Floating
            bool isFirstOverdue = UiTaskHelper.IsTaskOverdue(first);
            bool isSecondOverdue = UiTaskHelper.IsTaskOverdue(second);

            bool isFirstActive = UiTaskHelper.IsTaskOngoing(first);
            bool isSecondActive = UiTaskHelper.IsTaskOngoing(second);

            bool isFirstFloating = UiTaskHelper.IsTaskFloating(first);
            bool isSecondFloating = UiTaskHelper.IsTaskFloating(second);

            bool isBothInSameCategory = isFirstOverdue && isSecondOverdue || isFirstActive && isSecondActive;

            int order;

            if (isFirstFloating && isSecondFloating)
            {
                order = UiTaskHelper.CompareByDescription(first, second);
            }
            else if (isBothInSameCategory)
            {
                order = UiTaskHelper.Compare(first, second);
            }
            else if (isFirstOverdue)
            {
                // The first task is overdue, but the second isn't.
                order = -1;
            }
            else if (isSecondOverdue)
            {
                // The second task is overdue, but the first isn't.
                order = 1;
            }
            // Neither is overdue.
            else if (isFirstActive)
            {
                // The first task is active and the second isn't.
                // Second is either floating or inactive. Regardless,
                order = -1;
            }
            else if (isSecondActive)
            {
                // The second task is active and the first isn't.
                // First is either floating or inactive.
                order = 1;
            }
            // Neither is active.
            else if (!isFirstFloating && !isSecondFloating)
            {
                // Neither are floating.
                order = UiTaskHelper.Compare(first, second);
            }
            else if (isFirstFloating)
            {
                // First is floating, second isn't.
                order = 1;
            }
            else
            {
                // Second is floating, first isn't.
                order = -1;
            }

            return order;
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
