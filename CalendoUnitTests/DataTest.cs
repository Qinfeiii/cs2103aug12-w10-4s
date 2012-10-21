using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calendo.Data;
using System.IO;

namespace CalendoUnitTests
{
    [TestClass]
    public class DataTest
    {
        [TestMethod]
        public void DataLoad()
        {
            Storage<State<Entry>> UTStorage = new Storage<State<Entry>>();
            Assert.IsNotNull(UTStorage);

            UTStorage.Load();
            Assert.IsNotNull(UTStorage.Entries);
        }
        [TestMethod]
        public void DataSave()
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
        public void DataIncompatible()
        {   
            Storage<List<string>> UTStorage = new Storage<List<string>>("test2.txt");
            UTStorage.Load();
            UTStorage.Entries.Clear();
            UTStorage.Entries.Add("Test");
            UTStorage.Entries.Add("Test2");
            UTStorage.Save();
            UTStorage.Load();
            Assert.IsTrue(UTStorage.Entries.Count == 2);
            
            Storage<State<int>> UTStorageINT = new Storage<State<int>>("test2.txt");
            UTStorageINT.Load();
            UTStorageINT.Save();
            UTStorage.Load();
            // The contents should be overriden with the new one
            Assert.IsTrue(UTStorage.Entries.Count != 2);
        }
        [TestMethod]
        public void DataUnwritable()
        {
            Storage<List<string>> UTStoragePre = new Storage<List<string>>("test3.txt");
            UTStoragePre.Load();
            UTStoragePre.Entries.Clear();
            UTStoragePre.Entries.Add("Test1");
            UTStoragePre.Entries.Add("Test2");
            UTStoragePre.Save();

            // Lock the file to prevent other processes from modifying it
            Stream fileStream = new FileStream("test3.txt", FileMode.OpenOrCreate);
            fileStream.ReadByte();
            Storage<List<string>> UTStorage = new Storage<List<string>>("test3.txt");
            UTStorage.Load();
            UTStorage.Save();
            // Should not be able to load anything
            Assert.IsTrue(UTStorage.Entries.Count == 0);
            fileStream.Close();

            // Check if file is altered (it should not)
            UTStorage.Load();
            Assert.IsTrue(UTStorage.Entries.Count == 2);
        }
        [TestMethod]
        public void DataEntry()
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
            testEntry.Meta = "meta test";
            Entry cloneEntry = (Entry)testEntry.Clone();
            Assert.IsFalse(cloneEntry == testEntry);
            Assert.IsTrue(cloneEntry.ID == testEntry.ID);
            Assert.IsTrue(cloneEntry.Created == testEntry.Created);
            Assert.IsTrue(cloneEntry.Description == testEntry.Description);
            Assert.IsTrue(cloneEntry.EndTime == testEntry.EndTime);
            Assert.IsTrue(cloneEntry.StartTime == testEntry.StartTime);
            
            Assert.IsTrue(cloneEntry.StartTimeFormat == testEntry.StartTimeFormat);
            Assert.IsTrue(cloneEntry.EndTimeFormat == testEntry.EndTimeFormat);
            Assert.IsTrue(cloneEntry.Type == testEntry.Type);
            Assert.IsTrue(cloneEntry.Meta == testEntry.Meta);

            testEntry.ID = 1;
            Assert.IsFalse(cloneEntry.ID == testEntry.ID);
        }
        [TestMethod]
        public void DataState()
        {
            // State with class
            State<Data<int>> state = new State<Data<int>>();
            Assert.IsNotNull(state.Value);
            state.Value.Value = 2;
            state.AddState();
            state.Value.Value = 4;
            state.AddState();
            state.Undo();
            Assert.IsTrue(state.Value.Value == 2);

            // State with variable
            State<int> statevar = new State<int>();
            statevar.Value = 3;
            statevar.AddState();
            statevar.Value = 5;
            statevar.AddState();
            statevar.Undo();
            Assert.IsTrue(statevar.Value == 3);
            statevar.States.Clear();
            statevar.AddState();
            Assert.IsFalse(statevar.Value == 0);
        }
        [TestMethod]
        public void DataStateStorage()
        {
            StateStorage<List<Entry>> UTState = new StateStorage<List<Entry>>();
            UTState.Entries.Clear();
            UTState.Save();

            Entry testEntry = new Entry();
            testEntry.ID = 15;
            UTState.Entries.Add(testEntry);
            UTState.Save();
            UTState.Entries.Add((Entry)testEntry.Clone());
            UTState.Save();
            UTState.Load();
            Assert.IsTrue(UTState.Entries.Count == 2);
            UTState.Undo();
            Assert.IsTrue(UTState.Entries.Count == 1);
            UTState.Redo();
            Assert.IsTrue(UTState.Entries.Count == 2);
            UTState.Save();
            
            StateStorage<List<Entry>> UTState2 = new StateStorage<List<Entry>>("data.txt");
            UTState2.Clear(); // Remove all states

            // There should be no undo or redo
            Assert.IsFalse(UTState2.Undo());
            Assert.IsFalse(UTState2.Redo());

            UTState2.Save();

            // Replace current entries
            UTState2.Entries = new List<Entry>();
            Entry testEntry2 = new Entry();
            testEntry2.Description = "A";
            UTState2.Entries.Add(testEntry2);
            UTState2.Save();
            Assert.IsTrue(UTState2.HasUndo);
            Assert.IsFalse(UTState2.HasRedo);
            UTState2.Undo();
            Assert.IsTrue(UTState2.HasRedo);
        }
        [TestMethod]
        public void DataJSON()
        {
            Entry testEntry = new Entry();
            testEntry.Description = "test";
            JSON<Entry> jsonParser = new JSON<Entry>();
            string jsonString = jsonParser.Serialize(testEntry);
            Entry duplicate = jsonParser.Deserialize(jsonString);
            string jsonDup = jsonParser.Serialize(duplicate);
            Assert.IsTrue(duplicate.Description == "test");
            Assert.IsTrue(jsonDup == jsonString);
        }
    }
}
