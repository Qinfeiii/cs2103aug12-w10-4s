//@author A0080933E
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calendo;
using Calendo.Logic;
using Calendo.GoogleCalendar;

namespace CalendoUnitTests
{
    [TestClass]
    public class TMTest
    {
        TaskManager taskManager = TaskManager.Instance;

        [TestMethod]
        public void TMCreate()
        {
            taskManager.Load();
            taskManager.Add("Test1");
            taskManager.Save();
        }

        [TestMethod]
        public void TMAdd()
        {
            // Prevent past tests from affecting this test
            taskManager.Entries.Clear();

            // Add Valid
            taskManager.Add("Test Floating", null, null, null, null);
            taskManager.Add("Test Deadline 1", "31/12");
            taskManager.Add("Test Deadline 2", "", "23:59");
            taskManager.Add("Test Deadline 3", "1/1/" + (DateTime.Today.Year + 1).ToString(), "12:59 AM");
            taskManager.Add("Test Timed", "1/12", "14.00", "31.1", "3:02PM");
            taskManager.Add("Test Deadline 4", "", "0:01");
            taskManager.Add("Test Timed 2", "1/12-31/12", "14.00"); // Functionality removed in favor of new parameters
            taskManager.Add("Test Timed 3", "28/2/2100-29/2/2400", "", "", "");

            // Test floating
            Assert.IsTrue(taskManager.Entries[0].Description == "Test Floating");
            Assert.IsTrue(taskManager.Entries[0].StartTimeFormat == TimeFormat.None);
            Assert.IsTrue(taskManager.Entries[0].Type == EntryType.Floating);

            // Test Deadline with only date
            Assert.IsTrue(taskManager.Entries[1].Description == "Test Deadline 1");
            Assert.IsTrue(taskManager.Entries[1].StartTime.Year >= DateTime.Today.Year);
            Assert.IsTrue(taskManager.Entries[1].StartTime.Month == 12);
            Assert.IsTrue(taskManager.Entries[1].StartTime.Day == 31);
            Assert.IsTrue(taskManager.Entries[1].StartTimeFormat == TimeFormat.Date);
            Assert.IsTrue(taskManager.Entries[1].Type == EntryType.Deadline);

            // Test Deadline with only time (in future)
            Assert.IsTrue(taskManager.Entries[2].Description == "Test Deadline 2");
            Assert.IsTrue(taskManager.Entries[2].StartTime.Hour == 23);
            Assert.IsTrue(taskManager.Entries[2].StartTime.Minute == 59);
            Assert.IsTrue(taskManager.Entries[2].StartTimeFormat == TimeFormat.Time);

            // Test Deadline with both date and time
            Assert.IsTrue(taskManager.Entries[3].Description == "Test Deadline 3");
            Assert.IsTrue(taskManager.Entries[3].StartTime.Month == 1);
            Assert.IsTrue(taskManager.Entries[3].StartTime.Day == 1);
            Assert.IsTrue(taskManager.Entries[3].StartTime.Hour == 0);
            Assert.IsTrue(taskManager.Entries[3].StartTime.Minute == 59);
            Assert.IsTrue(taskManager.Entries[3].StartTimeFormat == TimeFormat.DateTime);

            // Test Timed task
            Assert.IsTrue(taskManager.Entries[4].Description == "Test Timed");
            Assert.IsTrue(taskManager.Entries[4].StartTime.Month == 12);
            Assert.IsTrue(taskManager.Entries[4].StartTime.Day == 1);
            Assert.IsTrue(taskManager.Entries[4].StartTime.Hour == 14);
            Assert.IsTrue(taskManager.Entries[4].StartTime.Minute == 0);
            Assert.IsTrue(taskManager.Entries[4].StartTimeFormat == TimeFormat.DateTime);
            Assert.IsTrue(taskManager.Entries[4].EndTime.Year > taskManager.Entries[4].StartTime.Year);
            Assert.IsTrue(taskManager.Entries[4].EndTime.Month == 1);
            Assert.IsTrue(taskManager.Entries[4].EndTime.Day == 31);
            Assert.IsTrue(taskManager.Entries[4].EndTime.Hour == 15);
            Assert.IsTrue(taskManager.Entries[4].EndTime.Minute == 2);
            Assert.IsTrue(taskManager.Entries[4].EndTimeFormat == TimeFormat.DateTime);
            Assert.IsTrue(taskManager.Entries[4].Type == EntryType.Timed);

            // Test Deadline with only time (in past)
            Assert.IsTrue(taskManager.Entries[5].Description == "Test Deadline 4");
            Assert.IsTrue(taskManager.Entries[5].StartTime.Hour == 0);
            Assert.IsTrue(taskManager.Entries[5].StartTime.Minute == 1);
            Assert.IsTrue(taskManager.Entries[5].StartTime > DateTime.Today);
            Assert.IsTrue(taskManager.Entries[5].StartTimeFormat == TimeFormat.DateTime);

            // Test Timed task
            Assert.IsTrue(taskManager.Entries[6].Description == "Test Timed 2");
            Assert.IsTrue(taskManager.Entries[6].StartTime.Month == 12);
            Assert.IsTrue(taskManager.Entries[6].StartTime.Day == 1);
            Assert.IsTrue(taskManager.Entries[6].StartTime.Hour == 14);
            Assert.IsTrue(taskManager.Entries[6].StartTime.Minute == 0);
            Assert.IsTrue(taskManager.Entries[6].StartTimeFormat == TimeFormat.DateTime);
            Assert.IsTrue(taskManager.Entries[6].EndTime.Month == 12);
            Assert.IsTrue(taskManager.Entries[6].EndTime.Day == 31);
            Assert.IsTrue(taskManager.Entries[6].EndTimeFormat == TimeFormat.Date);
            Assert.IsTrue(taskManager.Entries[6].Type == EntryType.Timed);

            // Test Timed task
            Assert.IsTrue(taskManager.Entries[7].Description == "Test Timed 3");
            Assert.IsTrue(taskManager.Entries[7].StartTime.Month == 2);
            Assert.IsTrue(taskManager.Entries[7].StartTime.Day == 28);
            Assert.IsTrue(taskManager.Entries[7].StartTime.Year == 2100);
            Assert.IsTrue(taskManager.Entries[7].StartTimeFormat == TimeFormat.Date);
            Assert.IsTrue(taskManager.Entries[7].EndTime.Month == 2);
            Assert.IsTrue(taskManager.Entries[7].EndTime.Day == 29);
            Assert.IsTrue(taskManager.Entries[7].EndTime.Year == 2400);
            Assert.IsTrue(taskManager.Entries[7].EndTimeFormat == TimeFormat.Date);
            Assert.IsTrue(taskManager.Entries[7].Type == EntryType.Timed);
        }

        [TestMethod]
        public void TMAddInvalid()
        {
            // Prevent past tests from affecting this test
            taskManager.Entries.Clear();

            // Non-existant date
            taskManager.Add("Test Invalid 1", "32/12", "0:00 PM");  // Bad date and invalid time
            Assert.IsTrue(taskManager.Entries.Count == 0);

            taskManager.Add("Test Invalid 2", "1/2/" + (DateTime.Today.Year.ToString()), "25:00"); // Day in past (same year) and invalid time
            Assert.IsTrue(taskManager.Entries.Count == 0);

            taskManager.Add("Test Invalid 3", "1/1/2011", null, "1/1/2010", null); // Day in past and null string
            Assert.IsTrue(taskManager.Entries.Count == 0);

            taskManager.Add("Test Invalid 4", "a/b/c", "-1:m");
            Assert.IsTrue(taskManager.Entries.Count == 0);

            // Partially valid should also be rejected
            taskManager.Add("Test Invalid 5", "a/b/c", "23:59");
            Assert.IsTrue(taskManager.Entries.Count == 0);

            taskManager.Add("Test Invalid 6", "1/1", "a:-1");
            Assert.IsTrue(taskManager.Entries.Count == 0);

            taskManager.Add("Test Invalid 5", null, null, "1/1", null);
            Assert.IsTrue(taskManager.Entries.Count == 0);
        }

        [TestMethod]
        public void TMChange()
        {
            // Prevent past tests from affecting this test
            taskManager.Entries.Clear();
            taskManager.Add("Test Timed", "1/12", "14:00", "31/1", "3:02PM");
            taskManager.Add("Test Floating");
            taskManager.Add("Test Floating 2", "1/1", "12:00PM", "2/1", null);

            // Out of range (do nothing)
            taskManager.Change(0, "a", "", "", "", "");
            taskManager.Change(taskManager.Entries.Count + 1, "b", "", "", "", "");
            taskManager.Change(-1, "c", "", "", "", "");

            taskManager.Change(1, "Test Changed 1");
            Assert.IsTrue(taskManager.Entries[0].Description == "Test Changed 1");

            taskManager.Change(2, "", "6/6/2012", "", "1/1/2013", "12:00AM");
            Assert.IsTrue(taskManager.Entries[1].Description == "Test Floating");
            Assert.IsTrue(taskManager.Entries[1].StartTime.Day == 6);
            Assert.IsTrue(taskManager.Entries[1].StartTime.Month == 6);
            Assert.IsTrue(taskManager.Entries[1].StartTimeFormat == TimeFormat.Date);
            Assert.IsTrue(taskManager.Entries[1].EndTime.Hour == 0);
            Assert.IsTrue(taskManager.Entries[1].EndTime.Minute == 0);
            Assert.IsTrue(taskManager.Entries[1].EndTimeFormat == TimeFormat.DateTime);

            taskManager.Change(3, "Test Floating 2 changed", "", "-", "-", "-");
            Assert.IsTrue(taskManager.Entries[2].Description == "Test Floating 2 changed");
            Assert.IsTrue(taskManager.Entries[2].StartTimeFormat == TimeFormat.Date);
            Assert.IsTrue(taskManager.Entries[2].EndTimeFormat == TimeFormat.None);
        }

        [TestMethod]
        public void TMChangeInvalid()
        {
            taskManager.Entries.Clear();
            taskManager.Add("Test Timed", "1/12/2012", "14:00", "31/1/2013", "3:02PM");

            // Invalid change operations should not alter the task

            // Invalid date
            taskManager.Change(1, null, "31/2");
            Assert.IsTrue(taskManager.Entries[0].StartTime.Day == 1);
            Assert.IsTrue(taskManager.Entries[0].StartTime.Month == 12);

            // Invalid date and time
            taskManager.Change(1, null, "2/2/123", "-1:00", "12/12");
            Assert.IsTrue(taskManager.Entries[0].StartTime.Day == 1);
            Assert.IsTrue(taskManager.Entries[0].StartTime.Month == 12);

            // Start date after end
            taskManager.Change(1, null, "2/2/13");
            Assert.IsTrue(taskManager.Entries[0].StartTime.Day == 1);
            Assert.IsTrue(taskManager.Entries[0].StartTime.Month == 12);

            // End date before start
            taskManager.Change(1, null, null, null, null, "2/2/2012");
            Assert.IsTrue(taskManager.Entries[0].StartTime.Day == 1);
            Assert.IsTrue(taskManager.Entries[0].StartTime.Month == 12);
        }

        [TestMethod]
        public void TMRemove()
        {
            // Prevent past tests from affecting this test
            taskManager.Entries.Clear();
            taskManager.Add("Test Floating 1");
            taskManager.Add("Test Floating 2");
            taskManager.Add("Test Floating 3");

            // Out of range (do nothing)
            taskManager.Remove(0);
            taskManager.Remove(taskManager.Entries.Count + 1);
            taskManager.Remove(-1);

            taskManager.Remove(2);
            Assert.IsTrue(taskManager.Entries.Count == 2);
            Assert.IsTrue(taskManager.Entries[0].Description == "Test Floating 1");
            Assert.IsTrue(taskManager.Entries[1].Description == "Test Floating 3");
        }

        [TestMethod]
        public void TMUndoRedo()
        {
            // Prevent past tests from affecting this test
            taskManager.Entries.Clear();
            taskManager.Add("Test Floating 1");
            taskManager.Add("Test Floating 2");
            taskManager.Add("Test Floating 3");

            taskManager.Remove(1);
            taskManager.Undo();
            Assert.IsTrue(taskManager.Entries.Count == 3);
            Assert.IsTrue(taskManager.Entries[0].Description == "Test Floating 1");
            Assert.IsTrue(taskManager.Entries[1].Description == "Test Floating 2");
            Assert.IsTrue(taskManager.Entries[2].Description == "Test Floating 3");

            taskManager.Redo();
            Assert.IsTrue(taskManager.Entries.Count == 2);
            Assert.IsTrue(taskManager.Entries[0].Description == "Test Floating 2");
            Assert.IsTrue(taskManager.Entries[1].Description == "Test Floating 3");
        }

        [TestMethod]
        public void TMSubscriber()
        {
            bool isUpdated = false;
            taskManager.Entries.Clear();
            Delegate subscriberDelegate = new TaskManager.UpdateHandler(delegate() { isUpdated = true; });
            taskManager.AddSubscriber(subscriberDelegate);
            taskManager.Add("Test");
            Assert.IsTrue(isUpdated == true);
        }

        [TestMethod]
        public void TMGoogleCalendar()
        {
            ThreadedGoogleCalendar.GoogleCalendarType = typeof(GoogleCalendarStub);
            ThreadedGoogleCalendar.AuthorizationMethod = new ThreadedGoogleCalendar.AuthorizationCall(delegate() { });
            taskManager.Export();
            ThreadedGoogleCalendar.CurrentThread.Join();
            Assert.IsTrue(GoogleCalendarStub.LastRun == "Export");
            taskManager.Import();
            ThreadedGoogleCalendar.CurrentThread.Join();
            Assert.IsTrue(GoogleCalendarStub.LastRun == "Import");
        }
    }
}
