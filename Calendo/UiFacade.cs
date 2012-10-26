using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Calendo.AutoSuggest;
using Calendo.Logic;

namespace Calendo
{
    class UiFacade
    {
        public AutoSuggest.AutoSuggest AutoSuggestSystem { get; set; }
        public CommandProcessor CommandProcessor { get; set; }

        public UiFacade(Delegate updateDelegate)
        {
            AutoSuggestSystem = new AutoSuggest.AutoSuggest();
            CommandProcessor = new CommandProcessor();
            
        }

        public void ExecuteCommand(string command)
        {
            CommandProcessor.ExecuteCommand(command);
        }

        public void SetSuggestions(string input)
        {
            AutoSuggestSystem.SetSuggestions(input);
        }
    }
}
