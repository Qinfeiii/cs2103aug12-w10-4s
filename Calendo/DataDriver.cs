using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calendo
{
    class DataDriver
    {
        //private DataStorage storage;
        private Storage<List<Entry>> storage;
        private Storage<State<Entry>> test;
        private List<Entry> entryList;
        public DataDriver()
        {
            entryList = new List<Entry>();
            storage = new Storage<List<Entry>>();
            test = new Storage<State<Entry>>();

        }
        public void Clear()
        {
            storage.Load();
            storage.Data.Clear();
            storage.Save();
        }
        public void Test()
        {
            // Load
            storage.Load();
            entryList = storage.Data;

            // Add
            entryList.Add(new Entry());

            // Modify
            entryList[0].Description = "Test";

            // Save
            storage.Data = entryList; //optional
            storage.Save();

            // Reload
            storage.Load();
            entryList = storage.Data;
            
            // Remove
            entryList.RemoveAt(0);

            // Save
            storage.Save();
        }

        // Test undo states
        public void TestUndo()
        {
            // Load
            ChronicStorage cs = new ChronicStorage();
            // Return to clean state
            cs.Load();
            cs.Clear(); // Clearing undo states cannot be undone!

            cs.Load();
            cs.Entries.Add(new Entry());
            cs.Entries[0].Description = "A";
            cs.Save(); // Save so that the state can be reverted

            
            cs.Entries.Add(new Entry());
            cs.Entries[1].Description = "B";
            cs.Save();
            cs.Undo(); // Undo should revert to last save (i.e. remove entry B)

            cs.Entries[0].Description = "C"; // Change properties
            cs.Save();

            cs.Entries[0].Description = "D";
            cs.Save();
            cs.Undo(); // Undo a property change
        }
    }
}
