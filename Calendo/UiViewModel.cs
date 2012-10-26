using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Calendo.AutoSuggest;
using Calendo.Logic;
using Calendo.Data;
using System.ComponentModel;

namespace Calendo
{
    class UiViewModel : INotifyPropertyChanged
    {
        private AutoSuggest.AutoSuggest AutoSuggestSystem { get; set; }
        private CommandProcessor CommandProcessor { get; set; }

        public List<AutoSuggestEntry> SuggestionList { get; set; }
        public Dictionary<int, Entry> TaskList { get; set; }

        private delegate void UpdateDelegate();

        public UiViewModel()
        {
            CommandProcessor = new CommandProcessor();
            AutoSuggestSystem = new AutoSuggest.AutoSuggest(CommandProcessor.GetInputCommandList());
            UpdateDelegate updateDelegate = new UpdateDelegate(UpdateItemsList);
            TaskManager.Instance.Subscribers.Add(updateDelegate);
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
            SuggestionList = AutoSuggestSystem.SuggestionList;
            OnPropertyChanged("SuggestionList");
        }

        private void UpdateItemsList()
        {
            Dictionary<int, Entry> itemDictionary = new Dictionary<int, Entry>();

            int count = 1;

            List<Entry> entries = new List<Entry>(CommandProcessor.TaskList);
            entries.Sort(delegate(Entry first, Entry second)
            {
                // We want items sorted by Overdue -> Active -> Floating
                bool isFirstOverdue = UiTaskHelper.IsTaskOverdue(first);
                bool isSecondOverdue = UiTaskHelper.IsTaskOverdue(second);

                bool isFirstActive = UiTaskHelper.IsTaskOngoing(first);
                bool isSecondActive = UiTaskHelper.IsTaskOngoing(second);

                bool isFirstFloating = UiTaskHelper.IsTaskFloating(first);
                bool isSecondFloating = UiTaskHelper.IsTaskFloating(second);

                int orderByDate;

                // If both are floating, this is irrelevant.
                if (isFirstFloating && isSecondFloating)
                {
                    orderByDate = 0;
                }
                else if (isFirstOverdue && isSecondOverdue || isFirstActive && isSecondActive)
                {
                    orderByDate = UiTaskHelper.CompareByDate(first, second);
                }
                else if (isFirstOverdue)
                {
                    // The first task is overdue, but the second isn't.
                    orderByDate = -1;
                }
                else if (isSecondOverdue)
                {
                    // The second task is overdue, but the first isn't.
                    orderByDate = 1;
                }
                // Neither is overdue.
                else if (isFirstActive)
                {
                    // The first task is active and the second isn't.
                    // Second is either floating or inactive. Regardless,
                    orderByDate = -1;
                }
                else if (isSecondActive)
                {
                    // The second task is active and the first isn't.
                    // First is either floating or inactive.
                    orderByDate = 1;
                }
                // Neither is active.
                else if (!isFirstFloating && !isSecondFloating)
                {
                    // Neither are floating.
                    orderByDate = UiTaskHelper.CompareByDate(first, second);
                }
                else if (isFirstFloating)
                {
                    // First is floating, second isn't.
                    orderByDate = 1;
                }
                else
                {
                    // Second is floating, first isn't.
                    orderByDate = -1;
                }

                if (orderByDate == 0)
                {
                    return UiTaskHelper.CompareByDescription(first, second);
                }
                return orderByDate;
            });

            CommandProcessor.IndexMap = new Dictionary<int, int>();
            foreach (Entry currentEntry in entries)
            {
                int originalIndex = CommandProcessor.TaskList.IndexOf(currentEntry) + 1;
                CommandProcessor.IndexMap.Add(count, originalIndex);
                itemDictionary.Add(count, currentEntry);
                count++;
            }

            TaskList = itemDictionary;
            OnPropertyChanged("TaskList");
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
