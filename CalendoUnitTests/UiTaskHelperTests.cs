//@author A0080860H
using System;
using Calendo;
using Calendo.Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UiUnitTests
{
    [TestClass]
    public class UiTaskHelperTests
    {
        [TestMethod]
        public void UI_OverdueTest_DeadlineTaskOverdue_JustNow()
        {
            Entry testData = new Entry();
            testData.Type = EntryType.Deadline;
            testData.StartTime = DateTime.Now.Subtract(TimeSpan.FromSeconds(1));

            bool expected = true;
            bool actual = UiTaskHelper.IsTaskOverdue(testData);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UI_OverdueTest_DeadlineTaskNotOverdue_VerySoon()
        {
            Entry testData = new Entry();
            testData.Type = EntryType.Deadline;
            testData.StartTime = DateTime.Now.AddSeconds(1);

            bool expected = false;
            bool actual = UiTaskHelper.IsTaskOverdue(testData);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UI_OverdueTest_TimedTaskOverdue_JustNow()
        {
            Entry testData = new Entry();
            testData.Type = EntryType.Timed;
            testData.EndTime = DateTime.Now.Subtract(TimeSpan.FromSeconds(1));

            bool expected = true;
            bool actual = UiTaskHelper.IsTaskOverdue(testData);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UI_OverdueTest_TimedTaskNotOverdue_VerySoon()
        {
            Entry testData = new Entry();
            testData.Type = EntryType.Timed;
            testData.EndTime = DateTime.Now.AddSeconds(1);

            bool expected = false;
            bool actual = UiTaskHelper.IsTaskOverdue(testData);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UI_OverdueTest_FloatingTask()
        {
            Entry testData = new Entry();
            testData.Type = EntryType.Floating;

            bool expected = false;
            bool actual = UiTaskHelper.IsTaskOverdue(testData);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UI_OngoingTest_DeadlineTaskOngoing_Now()
        {
            Entry testData = new Entry();
            testData.Type = EntryType.Deadline;
            testData.StartTime = DateTime.Now.AddSeconds(1); // Done to account for slowness in execution.

            bool expected = true;
            bool actual = UiTaskHelper.IsTaskOngoing(testData);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UI_OngoingTest_DeadlineTaskOngoing_Under24Hours()
        {
            Entry testData = new Entry();
            testData.Type = EntryType.Deadline;
            testData.StartTime = DateTime.Now.AddHours(23).AddMinutes(59);

            bool expected = true;
            bool actual = UiTaskHelper.IsTaskOngoing(testData);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UI_OngoingTest_DeadlineTaskNotOngoing_24Hours()
        {
            Entry testData = new Entry();
            testData.Type = EntryType.Deadline;
            testData.StartTime = DateTime.Now.AddHours(24);

            bool expected = false;
            bool actual = UiTaskHelper.IsTaskOngoing(testData);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UI_OngoingTest_DeadlineTaskNotOngoing_Past()
        {
            Entry testData = new Entry();
            testData.Type = EntryType.Deadline;
            testData.StartTime = DateTime.Now.Subtract(TimeSpan.FromSeconds(1));

            bool expected = false;
            bool actual = UiTaskHelper.IsTaskOngoing(testData);
            Assert.AreEqual(expected, actual);
        }
    }
}
