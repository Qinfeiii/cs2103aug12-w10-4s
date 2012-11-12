//@author A0080933E
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calendo.Logic;
using Calendo.Data;
using Calendo.GoogleCalendar;
using System.IO;

namespace CalendoUnitTests
{
    [TestClass]
    public class DataTest
    {
        /// <summary>
        /// Tests if entries can be loaded from file
        /// </summary>
        [TestMethod]
        public void DataLoad()
        {
            Storage<State<Entry>> UTStorage = new Storage<State<Entry>>();
            Assert.IsNotNull(UTStorage);

            UTStorage.Load();
            Assert.IsNotNull(UTStorage.Entries);
        }

        /// <summary>
        /// Tests if entries persist after saving
        /// </summary>
        [TestMethod]
        public void DataSave()
        {
            Storage<List<string>> UTStorage = new Storage<List<string>>("test1.txt");
            Assert.IsNotNull(UTStorage);

            // Check for entry persistance
            string testValue = "Test";

            UTStorage.Save();
            UTStorage.Load();
            Assert.IsTrue(UTStorage.Entries.Count == 0);
            UTStorage.Entries.Add(testValue);
            UTStorage.Save();
            Assert.IsTrue(UTStorage.Entries[0] == testValue);
            UTStorage.Load();
            Assert.IsTrue(UTStorage.Entries[0] == testValue);

            // Checks if entries can be modified directly
            List<string> testList = new List<string>();
            testList.Add("A");
            testList.Add("B");

            UTStorage.Entries = testList;
            UTStorage.Save();
            UTStorage.Load();
            Assert.IsTrue(UTStorage.Entries.Count == 2);

            // Checks clear functionality
            UTStorage.Entries.Clear();
            UTStorage.Save();
            UTStorage.Load();
            Assert.IsTrue(UTStorage.Entries.Count == 0);
        }

        /// <summary>
        /// Tests situations where the data file is corrupted
        /// </summary>
        [TestMethod]
        public void DataIncompatible()
        {
            // Populate entries
            Storage<List<string>> UTStorage = new Storage<List<string>>("test2.txt");
            UTStorage.Load();
            UTStorage.Entries.Clear();
            UTStorage.Entries.Add("Test");
            UTStorage.Entries.Add("Test2");
            UTStorage.Save();
            UTStorage.Load();
            Assert.IsTrue(UTStorage.Entries.Count == 2);

            // Checks concurrency operations
            Storage<State<int>> UTStorageINT = new Storage<State<int>>("test2.txt");
            UTStorageINT.Load();
            UTStorageINT.Save();
            UTStorage.Load();
            Assert.IsTrue(UTStorage.Entries.Count != 2);

            // Checks unserializable case
            Storage<Stack<string>> UnsupportedStorage = new Storage<Stack<string>>("test3.txt");
            Assert.IsTrue(UnsupportedStorage.Entries != null);
        }

        /// <summary>
        /// Tests situations where the data file is unreadable or locked
        /// </summary>
        [TestMethod]
        public void DataUnwritable()
        {
            string fileToLock = "test3.txt";

            Storage<List<string>> UTStoragePre = new Storage<List<string>>(fileToLock);
            UTStoragePre.Load();
            UTStoragePre.Entries.Clear();
            UTStoragePre.Entries.Add("Test1");
            UTStoragePre.Entries.Add("Test2");
            UTStoragePre.Save();

            // Lock the file to prevent other processes from modifying it
            Stream fileStream = new FileStream(fileToLock, FileMode.OpenOrCreate);
            fileStream.ReadByte();

            // Checks backup functionality
            Storage<List<string>> UTStorage = new Storage<List<string>>(fileToLock);
            UTStorage.Load();
            UTStorage.Save();
            Assert.IsTrue(UTStorage.Entries.Count == 0);
            UTStorage.Entries.Add("Test Add Lock");
            UTStorage.Save();
            UTStorage.Load();
            Assert.IsTrue(UTStorage.Entries.Count == 1);
            UTStorage.Entries.Clear();
            UTStorage.Save();
            fileStream.Close();

            // Check if original file is altered (it should not)
            UTStorage.Load();
            Assert.IsTrue(UTStorage.Entries.Count == 2);
        }

        /// <summary>
        /// Tests if entries can be modified
        /// </summary>
        [TestMethod]
        public void DataEntry()
        {
            Entry testEntry = new Entry();
            testEntry.Created = DateTime.Today;
            testEntry.ID = 0;
            testEntry.Description = "";
            testEntry.EndTime = DateTime.Today;
            testEntry.StartTime = DateTime.Today;
            testEntry.StartTimeFormat = TimeFormat.Date;
            testEntry.EndTimeFormat = TimeFormat.DateTime;
            testEntry.Type = EntryType.Deadline;
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
        }

        /// <summary>
        /// Tests State class functionality
        /// </summary>
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

        /// <summary>
        /// Tests StateStorage class functionality
        /// </summary>
        [TestMethod]
        public void DataStateStorage()
        {
            StateStorage<List<Entry>> storage = new StateStorage<List<Entry>>();
            storage.Entries.Clear();
            storage.Save();

            Entry testEntry = new Entry();
            testEntry.ID = 15;
            storage.Entries.Add(testEntry);
            storage.Save();
            storage.Entries.Add((Entry)testEntry.Clone());
            storage.Save();
            storage.Load();
            Assert.IsTrue(storage.Entries.Count == 2);
            storage.Undo();
            Assert.IsTrue(storage.Entries.Count == 1);
            storage.Redo();
            Assert.IsTrue(storage.Entries.Count == 2);
            storage.Save();

            StateStorage<List<Entry>> storage2 = new StateStorage<List<Entry>>("data.txt");
            storage2.Clear(); // Remove all states

            // There should be no undo or redo
            Assert.IsFalse(storage2.Undo());
            Assert.IsFalse(storage2.Redo());

            storage2.Save();

            // Replace current entries
            storage2.Entries = new List<Entry>();
            Entry testEntry2 = new Entry();
            testEntry2.Description = "Test";
            storage2.Entries.Add(testEntry2);
            storage2.Save();
            Assert.IsTrue(storage2.HasUndo);
            Assert.IsFalse(storage2.HasRedo);
            storage2.Undo();
            Assert.IsTrue(storage2.HasRedo);
        }

        /// <summary>
        /// Tests the JSON serialization and deserialization methods
        /// </summary>
        [TestMethod]
        public void GCJSON()
        {
            Entry testEntry = new Entry();
            string testDescription = "Test json";
            testEntry.Description = testDescription;
            JSON<Entry> jsonParser = new JSON<Entry>();
            string jsonString = jsonParser.Serialize(testEntry);
            Entry duplicate = jsonParser.Deserialize(jsonString);
            string jsonDup = jsonParser.Serialize(duplicate);
            Assert.IsTrue(duplicate.Description == testDescription);
            Assert.IsTrue(jsonDup == jsonString);
        }

        /// <summary>
        /// Tests JSON time conversion
        /// </summary>
        [TestMethod]
        public void GCJSONTime()
        {
            JSON<Entry> jsonParser = new JSON<Entry>();
            DateTime nowTime = DateTime.Now;
            // JSON time does not include milli and micro seconds
            DateTime properTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, nowTime.Hour, nowTime.Minute, nowTime.Second);

            string jsonTime = jsonParser.DateToJSON(properTime);
            DateTime parsedTime = jsonParser.JSONToDate(jsonTime).ToLocalTime();
            Assert.IsTrue(properTime == parsedTime);

            DateTime invalidTime = jsonParser.JSONToDate(null);
            Assert.IsTrue(invalidTime == new DateTime(0));
        }
    }
}
