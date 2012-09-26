using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calendo
{
    class DataDriver
    {
        private DataStorage storage;
        private List<Entry> entryList;
        public DataDriver()
        {
            entryList = new List<Entry>();
            storage = new DataStorage();
        }
        public void Clear()
        {
            storage.Load();
            storage.Entries.Clear();
            storage.Save();
        }
        public void Test()
        {
            // Load
            storage.Load();
            entryList = storage.Entries;

            // Add
            entryList.Add(new Entry());

            // Modify
            entryList[0].Description = "Test";

            // Save
            storage.Entries = entryList; //optional
            storage.Save();

            // Reload
            storage.Load();
            entryList = storage.Entries;
            
            // Remove
            entryList.RemoveAt(0);

            // Save
            storage.Save();
        }
    }
}
