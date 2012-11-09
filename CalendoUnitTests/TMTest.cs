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
        TaskManager tm = TaskManager.Instance;

        [TestMethod]
        public void TMCreate()
        {
            tm.Load();
            tm.Add("Test1", null, null, null, null);
            tm.Save();
        }
        [TestMethod]
        public void TMAdd()
        {
            // Prevent past tests from affecting this test
            tm.Entries.Clear();

            // Add Valid
            tm.Add("Test Floating", null, null, null, null);
            tm.Add("Test Deadline 1", "31/12", "", null, null);
            tm.Add("Test Deadline 2", "", "23:59", null, null);
            tm.Add("Test Deadline 3", "1/1/" + (DateTime.Today.Year + 1).ToString(), "12:59 AM", null, null);
            tm.Add("Test Timed", "1/12", "14.00", "31.1", "3:02PM");
            tm.Add("Test Deadline 4", "", "0:01", null, null);
            tm.Add("Test Timed 2", "1/12-31/12", "14.00", null, null); // Functionality removed in favor of new parameters
            tm.Add("Test Timed 3", "28/2/2100-29/2/2400", "", "", "");

            // Test floating
            Assert.IsTrue(tm.Entries[0].Description == "Test Floating");
            Assert.IsTrue(tm.Entries[0].StartTimeFormat == TimeFormat.None);
            Assert.IsTrue(tm.Entries[0].Type == EntryType.Floating);
            
            // Test Deadline with only date
            Assert.IsTrue(tm.Entries[1].Description == "Test Deadline 1");
            Assert.IsTrue(tm.Entries[1].StartTime.Year >= DateTime.Today.Year);
            Assert.IsTrue(tm.Entries[1].StartTime.Month == 12);
            Assert.IsTrue(tm.Entries[1].StartTime.Day == 31);
            Assert.IsTrue(tm.Entries[1].StartTimeFormat == TimeFormat.Date);
            Assert.IsTrue(tm.Entries[1].Type == EntryType.Deadline);

            // Test Deadline with only time (in future)
            Assert.IsTrue(tm.Entries[2].Description == "Test Deadline 2");
            Assert.IsTrue(tm.Entries[2].StartTime.Hour == 23);
            Assert.IsTrue(tm.Entries[2].StartTime.Minute == 59);
            Assert.IsTrue(tm.Entries[2].StartTimeFormat == TimeFormat.Time);

            // Test Deadline with both date and time
            Assert.IsTrue(tm.Entries[3].Description == "Test Deadline 3");
            Assert.IsTrue(tm.Entries[3].StartTime.Month == 1);
            Assert.IsTrue(tm.Entries[3].StartTime.Day == 1);
            Assert.IsTrue(tm.Entries[3].StartTime.Hour == 0);
            Assert.IsTrue(tm.Entries[3].StartTime.Minute == 59);
            Assert.IsTrue(tm.Entries[3].StartTimeFormat == TimeFormat.DateTime);

            // Test Timed task
            Assert.IsTrue(tm.Entries[4].Description == "Test Timed");
            Assert.IsTrue(tm.Entries[4].StartTime.Month == 12);
            Assert.IsTrue(tm.Entries[4].StartTime.Day == 1);
            Assert.IsTrue(tm.Entries[4].StartTime.Hour == 14);
            Assert.IsTrue(tm.Entries[4].StartTime.Minute == 0);
            Assert.IsTrue(tm.Entries[4].StartTimeFormat == TimeFormat.DateTime);
            Assert.IsTrue(tm.Entries[4].EndTime.Year > tm.Entries[4].StartTime.Year);
            Assert.IsTrue(tm.Entries[4].EndTime.Month == 1);
            Assert.IsTrue(tm.Entries[4].EndTime.Day == 31);
            Assert.IsTrue(tm.Entries[4].EndTime.Hour == 15);
            Assert.IsTrue(tm.Entries[4].EndTime.Minute == 2);
            Assert.IsTrue(tm.Entries[4].EndTimeFormat == TimeFormat.DateTime);
            Assert.IsTrue(tm.Entries[4].Type == EntryType.Timed);

            // Test Deadline with only time (in past)
            Assert.IsTrue(tm.Entries[5].Description == "Test Deadline 4");
            Assert.IsTrue(tm.Entries[5].StartTime.Hour == 0);
            Assert.IsTrue(tm.Entries[5].StartTime.Minute == 1);
            Assert.IsTrue(tm.Entries[5].StartTime > DateTime.Today);
            Assert.IsTrue(tm.Entries[5].StartTimeFormat == TimeFormat.DateTime);

            // Test Timed task
            Assert.IsTrue(tm.Entries[6].Description == "Test Timed 2");
            Assert.IsTrue(tm.Entries[6].StartTime.Month == 12);
            Assert.IsTrue(tm.Entries[6].StartTime.Day == 1);
            Assert.IsTrue(tm.Entries[6].StartTime.Hour == 14);
            Assert.IsTrue(tm.Entries[6].StartTime.Minute == 0);
            Assert.IsTrue(tm.Entries[6].StartTimeFormat == TimeFormat.DateTime);
            Assert.IsTrue(tm.Entries[6].EndTime.Month == 12);
            Assert.IsTrue(tm.Entries[6].EndTime.Day == 31);
            Assert.IsTrue(tm.Entries[6].EndTimeFormat == TimeFormat.Date);
            Assert.IsTrue(tm.Entries[6].Type == EntryType.Timed);

            // Test Timed task
            Assert.IsTrue(tm.Entries[7].Description == "Test Timed 3");
            Assert.IsTrue(tm.Entries[7].StartTime.Month == 2);
            Assert.IsTrue(tm.Entries[7].StartTime.Day == 28);
            Assert.IsTrue(tm.Entries[7].StartTime.Year == 2100);
            Assert.IsTrue(tm.Entries[7].StartTimeFormat == TimeFormat.Date);
            Assert.IsTrue(tm.Entries[7].EndTime.Month == 2);
            Assert.IsTrue(tm.Entries[7].EndTime.Day == 29);
            Assert.IsTrue(tm.Entries[7].EndTime.Year == 2400);
            Assert.IsTrue(tm.Entries[7].EndTimeFormat == TimeFormat.Date);
            Assert.IsTrue(tm.Entries[7].Type == EntryType.Timed);
        }

        [TestMethod]
        public void TMAddInvalid()
        {
            // Prevent past tests from affecting this test
            tm.Entries.Clear();

            // Non-existant date
            tm.Add("Test Invalid 1", "32/12", "0:00 PM", null, null);  // Bad daate and invalid time
            tm.Add("Test Invalid 2", "1/2/" + (DateTime.Today.Year.ToString()), "25:00", null, null); // Day in past (same year) and invalid time
            tm.Add("Test Invalid 3", "1/1/2011", null, "1/1/2010", null); // Day in past and null string
            tm.Add("Test Invalid 4", "a/b/c", "-1:m", null, null);
            
            // Invalid fields should be ignored
            Assert.IsTrue(tm.Entries[0].Type == EntryType.Floating);
            //Assert.IsTrue(tm.Entries[1].Type == EntryType.FLOATING);
            //Assert.IsTrue(tm.Entries[2].Type == EntryType.FLOATING);
            Assert.IsTrue(tm.Entries[3].Type == EntryType.Floating);

            // Partially valid
            tm.Add("Test Invalid 5", "a/b/c", "23:59", null, null);
            Assert.IsTrue(tm.Entries[4].Type == EntryType.Deadline);
            Assert.IsTrue(tm.Entries[4].StartTimeFormat == TimeFormat.Time);

            tm.Add("Test Invalid 6", "1/1", "a:-1", null, null);
            Assert.IsTrue(tm.Entries[5].Type == EntryType.Deadline);
            Assert.IsTrue(tm.Entries[5].StartTimeFormat == TimeFormat.Date);

            tm.Add("Test Invalid 5", null, null, "1/1", null);
            Assert.IsTrue(tm.Entries[6].Type == EntryType.Floating);
        }

        [TestMethod]
        public void TMChange()
        {
            // Prevent past tests from affecting this test
            tm.Entries.Clear();
            tm.Add("Test Timed", "1/12", "14:00", "31/1", "3:02PM");
            tm.Add("Test Floating", null, null, null, null);
            tm.Add("Test Floating 2", "1/1", "12:00PM", "2/1", null);

            // Out of range (do nothing)
            tm.Change(0, "a", "", "", "", "");
            tm.Change(tm.Entries.Count + 1, "b", "", "", "", "");
            tm.Change(-1, "c", "", "", "", "");

            // Note: Task ID is 1-based
            tm.Change(1, "Test Changed 1", null, null, null, null);
            Assert.IsTrue(tm.Entries[0].Description == "Test Changed 1");

            tm.Change(2, "", "6/6/2012", "", "1/1/2013", "12:00AM");
            Assert.IsTrue(tm.Entries[1].Description == "Test Floating");
            Assert.IsTrue(tm.Entries[1].StartTime.Day == 6);
            Assert.IsTrue(tm.Entries[1].StartTime.Month == 6);
            Assert.IsTrue(tm.Entries[1].StartTimeFormat == TimeFormat.Date);
            Assert.IsTrue(tm.Entries[1].EndTime.Hour == 0);
            Assert.IsTrue(tm.Entries[1].EndTime.Minute == 0);
            Assert.IsTrue(tm.Entries[1].EndTimeFormat == TimeFormat.DateTime);

            tm.Change(3, "Test Floating 2 changed", "", "-", "-", "-");
            Assert.IsTrue(tm.Entries[2].Description == "Test Floating 2 changed");
            Assert.IsTrue(tm.Entries[2].StartTimeFormat == TimeFormat.Date);
            Assert.IsTrue(tm.Entries[2].EndTimeFormat == TimeFormat.None);
        }

        [TestMethod]
        public void TMRemove()
        {
            // Prevent past tests from affecting this test
            tm.Entries.Clear();
            tm.Add("Test Floating 1", null, null, null, null);
            tm.Add("Test Floating 2", null, null, null, null);
            tm.Add("Test Floating 3", null, null, null, null);

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
            
            // Prevent past tests from affecting this test
            tm.Entries.Clear();
            tm.Add("Test Floating 1", null, null, null, null);
            tm.Add("Test Floating 2", null, null, null, null);
            tm.Add("Test Floating 3", null, null, null, null);

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

        [TestMethod]
        public void TMSubscriber()
        {
            bool isUpdated = false;
            tm.Entries.Clear();
            Delegate d = new TaskManager.UpdateHandler(delegate() { isUpdated = true; });
            tm.AddSubscriber(d);
            tm.Add("Test");
            Assert.IsTrue(isUpdated == true);
        }

        [TestMethod]
        public void TMGoogleCalendar()
        {
            ThreadedGoogleCalendar.GoogleCalendarType = typeof(GoogleCalendarStub);
            ThreadedGoogleCalendar.AuthorizationMethod = new ThreadedGoogleCalendar.AuthorizationCall(delegate() { });
            tm.Export();
            ThreadedGoogleCalendar.CurrentThread.Join();
            Assert.IsTrue(GoogleCalendarStub.LastRun == "Export");
            tm.Import();
            ThreadedGoogleCalendar.CurrentThread.Join();
            Assert.IsTrue(GoogleCalendarStub.LastRun == "Import");
        }
    }
}
