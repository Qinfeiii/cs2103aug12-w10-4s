using System;
using Calendo;
using Calendo.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UiUnitTests
{
    [TestClass]
    public class UiTaskHelperTests
    {
        [TestMethod]
        public void OverdueTest_DeadlineTaskOverdue()
        {
            Entry testData = new Entry();
            testData.Type = EntryType.DEADLINE;
            testData.StartTime = new DateTime(1990, 10, 20);

            bool expected = true;
            bool actual = UiTaskHelper.IsTaskOverdue(testData);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void OverdueTest_DeadlineTaskNotOverdue()
        {
            Entry testData = new Entry();
            testData.Type = EntryType.DEADLINE;
            testData.StartTime = DateTime.Now.AddDays(5);

            bool expected = false;
            bool actual = UiTaskHelper.IsTaskOverdue(testData);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void OverdueTest_TimedTaskOverdue()
        {
            Entry testData = new Entry();
            testData.Type = EntryType.TIMED;
            testData.EndTime = new DateTime(1990, 10, 20);

            bool expected = true;
            bool actual = UiTaskHelper.IsTaskOverdue(testData);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void OverdueTest_TimedTaskNotOverdue()
        {
            Entry testData = new Entry();
            testData.Type = EntryType.TIMED;
            testData.EndTime = DateTime.Now.AddDays(5);

            bool expected = false;
            bool actual = UiTaskHelper.IsTaskOverdue(testData);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void OverdueTest_FloatingTask()
        {
            Entry testData = new Entry();
            testData.Type = EntryType.FLOATING;

            bool expected = false;
            bool actual = UiTaskHelper.IsTaskOverdue(testData);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void OngoingTest_DeadlineTaskOngoing_Now()
        {
            Entry testData = new Entry();
            testData.Type = EntryType.DEADLINE;
            testData.StartTime = DateTime.Now;

            bool expected = true;
            bool actual = UiTaskHelper.IsTaskOngoing(testData);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void OngoingTest_DeadlineTaskOngoing_Under24Hours()
        {
            Entry testData = new Entry();
            testData.Type = EntryType.DEADLINE;
            testData.StartTime = DateTime.Now.AddHours(23).AddMinutes(59);

            bool expected = true;
            bool actual = UiTaskHelper.IsTaskOngoing(testData);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void OngoingTest_DeadlineTaskNotOngoing_24Hours()
        {
            Entry testData = new Entry();
            testData.Type = EntryType.DEADLINE;
            testData.StartTime = DateTime.Now.AddHours(24);

            bool expected = false;
            bool actual = UiTaskHelper.IsTaskOngoing(testData);
            Assert.AreEqual(expected, actual);
        }
    }
}
