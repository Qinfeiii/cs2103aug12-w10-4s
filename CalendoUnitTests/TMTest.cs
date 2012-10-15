﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calendo;
namespace CalendoUnitTests
{
    [TestClass]
    public class TMTest
    {
        [TestMethod]
        public void TMCreate()
        {
            TaskManager tm = new TaskManager();
            tm.Add("Test1");
        }
        [TestMethod]
        public void TMAdd()
        {
            TaskManager tm = new TaskManager();
            // Prevent past tests from affecting this test
            tm.Entries.Clear();

            // Add Valid
            tm.Add("Test Floating");
            tm.Add("Test Deadline 1", "31/12", "");
            tm.Add("Test Deadline 2", "", "23:59");
            tm.Add("Test Deadline 3", "1/1/" + DateTime.Today.Year.ToString(), "12:59 AM");
            tm.Add("Test Timed", "1/12", "14.00", "31.1", "3:02PM");
            tm.Add("Test Deadline 4", "", "0:01");
            tm.Add("Test Timed 2", "1/12-31/12", "14.00");
            tm.Add("Test Timed 3", "28/2/2100-29/2/2400", "", "", "");

            // Test floating
            Assert.IsTrue(tm.Entries[0].Description == "Test Floating");
            Assert.IsTrue(tm.Entries[0].StartTimeFormat == Calendo.Data.TimeFormat.NONE);
            Assert.IsTrue(tm.Entries[0].Type == Calendo.Data.EntryType.FLOATING);
            
            // Test Deadline with only date
            Assert.IsTrue(tm.Entries[1].Description == "Test Deadline 1");
            Assert.IsTrue(tm.Entries[1].StartTime.Year >= DateTime.Today.Year);
            Assert.IsTrue(tm.Entries[1].StartTime.Month == 12);
            Assert.IsTrue(tm.Entries[1].StartTime.Day == 31);
            Assert.IsTrue(tm.Entries[1].StartTimeFormat == Calendo.Data.TimeFormat.DATE);
            Assert.IsTrue(tm.Entries[1].Type == Calendo.Data.EntryType.DEADLINE);

            // Test Deadline with only time (in future)
            Assert.IsTrue(tm.Entries[2].Description == "Test Deadline 2");
            Assert.IsTrue(tm.Entries[2].StartTime.Hour == 23);
            Assert.IsTrue(tm.Entries[2].StartTime.Minute == 59);
            Assert.IsTrue(tm.Entries[2].StartTimeFormat == Calendo.Data.TimeFormat.TIME);

            // Test Deadline with both date and time
            Assert.IsTrue(tm.Entries[3].Description == "Test Deadline 3");
            Assert.IsTrue(tm.Entries[3].StartTime.Month == 1);
            Assert.IsTrue(tm.Entries[3].StartTime.Day == 1);
            Assert.IsTrue(tm.Entries[3].StartTime.Hour == 0);
            Assert.IsTrue(tm.Entries[3].StartTime.Minute == 59);
            Assert.IsTrue(tm.Entries[3].StartTimeFormat == Calendo.Data.TimeFormat.DATETIME);

            // Test Timed task
            Assert.IsTrue(tm.Entries[4].Description == "Test Timed");
            Assert.IsTrue(tm.Entries[4].StartTime.Month == 12);
            Assert.IsTrue(tm.Entries[4].StartTime.Day == 1);
            Assert.IsTrue(tm.Entries[4].StartTime.Hour == 14);
            Assert.IsTrue(tm.Entries[4].StartTime.Minute == 0);
            Assert.IsTrue(tm.Entries[4].StartTimeFormat == Calendo.Data.TimeFormat.DATETIME);
            Assert.IsTrue(tm.Entries[4].EndTime.Year > tm.Entries[4].StartTime.Year);
            Assert.IsTrue(tm.Entries[4].EndTime.Month == 1);
            Assert.IsTrue(tm.Entries[4].EndTime.Day == 31);
            Assert.IsTrue(tm.Entries[4].EndTime.Hour == 15);
            Assert.IsTrue(tm.Entries[4].EndTime.Minute == 2);
            Assert.IsTrue(tm.Entries[4].EndTimeFormat == Calendo.Data.TimeFormat.DATETIME);
            Assert.IsTrue(tm.Entries[4].Type == Calendo.Data.EntryType.TIMED);

            // Test Deadline with only time (in past)
            Assert.IsTrue(tm.Entries[5].Description == "Test Deadline 4");
            Assert.IsTrue(tm.Entries[5].StartTime.Hour == 0);
            Assert.IsTrue(tm.Entries[5].StartTime.Minute == 1);
            Assert.IsTrue(tm.Entries[5].StartTime > DateTime.Today);
            Assert.IsTrue(tm.Entries[5].StartTimeFormat == Calendo.Data.TimeFormat.DATETIME);

            // Test Timed task
            Assert.IsTrue(tm.Entries[6].Description == "Test Timed 2");
            Assert.IsTrue(tm.Entries[6].StartTime.Month == 12);
            Assert.IsTrue(tm.Entries[6].StartTime.Day == 1);
            Assert.IsTrue(tm.Entries[6].StartTime.Hour == 14);
            Assert.IsTrue(tm.Entries[6].StartTime.Minute == 0);
            Assert.IsTrue(tm.Entries[6].StartTimeFormat == Calendo.Data.TimeFormat.DATETIME);
            Assert.IsTrue(tm.Entries[6].EndTime.Month == 12);
            Assert.IsTrue(tm.Entries[6].EndTime.Day == 31);
            // End time not implemented yet
            Assert.IsTrue(tm.Entries[6].EndTimeFormat == Calendo.Data.TimeFormat.DATE);
            Assert.IsTrue(tm.Entries[6].Type == Calendo.Data.EntryType.TIMED);

            // Test Timed task
            Assert.IsTrue(tm.Entries[7].Description == "Test Timed 3");
            Assert.IsTrue(tm.Entries[7].StartTime.Month == 2);
            Assert.IsTrue(tm.Entries[7].StartTime.Day == 28);
            Assert.IsTrue(tm.Entries[7].StartTime.Year == 2100);
            Assert.IsTrue(tm.Entries[7].StartTimeFormat == Calendo.Data.TimeFormat.DATE);
            Assert.IsTrue(tm.Entries[7].EndTime.Month == 2);
            Assert.IsTrue(tm.Entries[7].EndTime.Day == 29);
            Assert.IsTrue(tm.Entries[7].EndTime.Year == 2400);
            Assert.IsTrue(tm.Entries[7].EndTimeFormat == Calendo.Data.TimeFormat.DATE);
            Assert.IsTrue(tm.Entries[7].Type == Calendo.Data.EntryType.TIMED);
        }

        [TestMethod]
        public void TestAddInvalid()
        {
            TaskManager tm = new TaskManager();
            // Prevent past tests from affecting this test
            tm.Entries.Clear();

            // Non-existant date
            tm.Add("Test Invalid 1", "32/12", ""); 
            tm.Add("Test Invalid 2", "29/2/2011", "25:00"); // Bad date and invalid time
            tm.Add("Test Invalid 3", "1/1/2011", null); // Day in past and null string
            tm.Add("Test Invalid 4", "a/b/c", "-1:m");
            // Invalid fields should be ignored
            Assert.IsTrue(tm.Entries[0].Type == Calendo.Data.EntryType.FLOATING);
            Assert.IsTrue(tm.Entries[1].Type == Calendo.Data.EntryType.FLOATING);
            Assert.IsTrue(tm.Entries[2].Type == Calendo.Data.EntryType.FLOATING);
            Assert.IsTrue(tm.Entries[3].Type == Calendo.Data.EntryType.FLOATING);

            // Partially valid
            tm.Add("Test Invalid 5", "a/b/c", "23:59");
            Assert.IsTrue(tm.Entries[4].Type == Calendo.Data.EntryType.DEADLINE);
            Assert.IsTrue(tm.Entries[4].StartTimeFormat == Calendo.Data.TimeFormat.TIME);

            tm.Add("Test Invalid 6", "1/1", "a:-1");
            Assert.IsTrue(tm.Entries[5].Type == Calendo.Data.EntryType.DEADLINE);
            Assert.IsTrue(tm.Entries[5].StartTimeFormat == Calendo.Data.TimeFormat.DATE);
        }

        [TestMethod]
        public void TMChange()
        {
            TaskManager tm = new TaskManager();
            // Prevent past tests from affecting this test
            tm.Entries.Clear();
            tm.Add("Test Timed", "1/12", "14:00", "31/1", "3:02PM");
            tm.Add("Test Floating");
            tm.Add("Test Floating 2");

            // Out of range (do nothing)
            tm.Change(0, "a", "", "", "", "");
            tm.Change(tm.Entries.Count + 1, "b", "", "", "", "");
            tm.Change(-1, "c", "", "", "", "");

            // Note: Task ID is 1-based
            tm.Change(1, "Test Changed 1");
            Assert.IsTrue(tm.Entries[0].Description == "Test Changed 1");

            tm.Change(2, "", "6/6", "", "1/1", "12:00AM");
            Assert.IsTrue(tm.Entries[1].Description == "Test Floating");
            Assert.IsTrue(tm.Entries[1].StartTime.Day == 6);
            Assert.IsTrue(tm.Entries[1].StartTime.Month == 6);
            Assert.IsTrue(tm.Entries[1].StartTimeFormat == Calendo.Data.TimeFormat.DATE);
            Assert.IsTrue(tm.Entries[1].EndTime.Hour == 0);
            Assert.IsTrue(tm.Entries[1].EndTime.Minute == 0);
            Assert.IsTrue(tm.Entries[1].EndTimeFormat == Calendo.Data.TimeFormat.DATETIME);

            tm.Change(3, "Test Floating 2 changed", "", "", "", "");
            Assert.IsTrue(tm.Entries[2].Description == "Test Floating 2 changed");
            Assert.IsTrue(tm.Entries[2].StartTimeFormat == Calendo.Data.TimeFormat.NONE);
            Assert.IsTrue(tm.Entries[2].EndTimeFormat == Calendo.Data.TimeFormat.NONE);
        }

        [TestMethod]
        public void TMRemove()
        {
            TaskManager tm = new TaskManager();
            // Prevent past tests from affecting this test
            tm.Entries.Clear();
            tm.Add("Test Floating 1");
            tm.Add("Test Floating 2");
            tm.Add("Test Floating 3");

            // Out of range (do nothing)
            tm.Remove(0);
            tm.Remove(tm.Entries.Count + 1);
            tm.Remove(-1);

            tm.Remove(2);
            Assert.IsTrue(tm.Entries.Count == 2);
            Assert.IsTrue(tm.Entries[0].Description == "Test Floating 1");
            Assert.IsTrue(tm.Entries[1].Description == "Test Floating 3");
        }

        [TestMethod]
        public void TMUndoRedo()
        {
            TaskManager tm = new TaskManager();
            // Prevent past tests from affecting this test
            tm.Entries.Clear();
            tm.Add("Test Floating 1");
            tm.Add("Test Floating 2");
            tm.Add("Test Floating 3");

            tm.Remove(1);
            tm.Undo();
            Assert.IsTrue(tm.Entries.Count == 3);
            Assert.IsTrue(tm.Entries[0].Description == "Test Floating 1");
            Assert.IsTrue(tm.Entries[1].Description == "Test Floating 2");
            Assert.IsTrue(tm.Entries[2].Description == "Test Floating 3");

            tm.Redo();
            Assert.IsTrue(tm.Entries.Count == 2);
            Assert.IsTrue(tm.Entries[0].Description == "Test Floating 2");
            Assert.IsTrue(tm.Entries[1].Description == "Test Floating 3");
        }
    }
}