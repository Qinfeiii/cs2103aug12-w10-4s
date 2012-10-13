using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calendo.Data;

namespace CalendoUnitTests
{
    [TestClass]
    public class DataTest
    {
        [TestMethod]
        public void TestLoad()
        {
            Storage<State<Entry>> UTStorage = new Storage<State<Entry>>();
            Assert.IsNotNull(UTStorage);

            UTStorage.Load();
            Assert.IsNotNull(UTStorage.Entries);
        }
        [TestMethod]
        public void TestSave()
        {
            Storage<List<string>> UTStorage = new Storage<List<string>>("test1.txt");
            Assert.IsNotNull(UTStorage);

            UTStorage.Save();
            UTStorage.Load();
            Assert.IsTrue(UTStorage.Entries.Count == 0);
            UTStorage.Entries.Add("Test");
            UTStorage.Save();
            Assert.IsTrue(UTStorage.Entries[0] == "Test");
            UTStorage.Load();
            Assert.IsTrue(UTStorage.Entries[0] == "Test");

            List<string> testList = new List<string>();
            testList.Add("A");
            testList.Add("B");

            UTStorage.Entries = testList;
            UTStorage.Save();
            UTStorage.Load();
            Assert.IsTrue(UTStorage.Entries.Count == 2);

            UTStorage.Entries.Clear();
            UTStorage.Save();
            Assert.IsTrue(UTStorage.Entries.Count == 0);
        }
        [TestMethod]
        public void TestIncompatible()
        {
            Calendo.DebugTool.Debug.Enable = false;
            Storage<List<string>> UTStorage = new Storage<List<string>>("test2.txt");
            UTStorage.Entries.Clear();
            UTStorage.Entries.Add("Test");
            UTStorage.Save();
            
            /*
            Storage<State<int>> UTStorageINT = new Storage<State<int>>("test2.txt");
            UTStorageINT.Load();
            UTStorageINT.Save();
            UTStorage.Load();
            Assert.IsTrue(UTStorage.Entries.Count == 0);
            */
            
            Calendo.DebugTool.Debug.Enable = true;
        }
        [TestMethod]
        public void TestEntry()
        {
            Entry testEntry = new Entry();
            testEntry.Created = DateTime.Today;
            testEntry.ID = 0;
            testEntry.Description = "";
            testEntry.EndTime = DateTime.Today;
            testEntry.StartTime = DateTime.Today;
            testEntry.StartTimeFormat = TimeFormat.DATE;
            testEntry.EndTimeFormat = TimeFormat.DATETIME;
            testEntry.Type = EntryType.DEADLINE;
            Entry cloneEntry = testEntry.clone();
            Assert.IsFalse(cloneEntry == testEntry);
            Assert.IsTrue(cloneEntry.ID == testEntry.ID);
            Assert.IsTrue(cloneEntry.Created == testEntry.Created);
            Assert.IsTrue(cloneEntry.Description == testEntry.Description);
            Assert.IsTrue(cloneEntry.EndTime == testEntry.EndTime);
            Assert.IsTrue(cloneEntry.StartTime == testEntry.StartTime);
            
            Assert.IsTrue(cloneEntry.StartTimeFormat == testEntry.StartTimeFormat);
            Assert.IsTrue(cloneEntry.EndTimeFormat == testEntry.EndTimeFormat);
            Assert.IsTrue(cloneEntry.Type == testEntry.Type);

            testEntry.ID = 1;
            Assert.IsFalse(cloneEntry.ID == testEntry.ID);
        }
        [TestMethod]
        public void TestState()
        {
            Calendo.DebugTool.Debug.Enable = false;
            StateStorage<List<Entry>> UTState = new StateStorage<List<Entry>>();
            UTState.Entries.Clear();
            UTState.Save();

            Entry testEntry = new Entry();
            testEntry.ID = 15;
            UTState.Entries.Add(testEntry);
            UTState.Save();
            UTState.Entries.Add(testEntry.clone());
            UTState.Save();
            UTState.Load();
            Assert.IsTrue(UTState.Entries.Count == 2);
            UTState.Undo();
            Assert.IsTrue(UTState.Entries.Count == 1);
            UTState.Redo();
            Assert.IsTrue(UTState.Entries.Count == 2);
            UTState.Save();
            
            StateStorage<List<Entry>> UTState2 = new StateStorage<List<Entry>>("data.txt");
            UTState2.Clear();
            Assert.IsFalse(UTState2.Undo());
            Assert.IsFalse(UTState2.Redo());
            UTState2.Save();
            /*
            UTState2.Entries = new List<Entry>();
            UTState2.Save();
            Assert.IsTrue(UTState2.Undo());
            Assert.IsTrue(UTState2.Redo());
            Assert.IsTrue(UTState2.Undo());
             * */
            Calendo.DebugTool.Debug.Enable = true;
        }
    }
}
