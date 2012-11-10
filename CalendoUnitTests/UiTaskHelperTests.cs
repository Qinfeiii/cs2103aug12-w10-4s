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
        public void UiOverdueTestDeadlineTaskOverdueJustNow()
        {
            Entry testData = new Entry();
            testData.Type = EntryType.Deadline;
            testData.StartTime = DateTime.Now.Subtract(TimeSpan.FromSeconds(1));

            bool expected = true;
            bool actual = UiTaskHelper.IsTaskOverdue(testData);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UiOverdueTestDeadlineTaskNotOverdueVerySoon()
        {
            Entry testData = new Entry();
            testData.Type = EntryType.Deadline;
            testData.StartTime = DateTime.Now.AddSeconds(1);

            bool expected = false;
            bool actual = UiTaskHelper.IsTaskOverdue(testData);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UiOverdueTestTimedTaskOverdueJustNow()
        {
            Entry testData = new Entry();
            testData.Type = EntryType.Timed;
            testData.EndTime = DateTime.Now.Subtract(TimeSpan.FromSeconds(1));

            bool expected = true;
            bool actual = UiTaskHelper.IsTaskOverdue(testData);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UiOverdueTestTimedTaskNotOverdueVerySoon()
        {
            Entry testData = new Entry();
            testData.Type = EntryType.Timed;
            testData.EndTime = DateTime.Now.AddSeconds(1);

            bool expected = false;
            bool actual = UiTaskHelper.IsTaskOverdue(testData);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UiOverdueTestFloatingTask()
        {
            Entry testData = new Entry();
            testData.Type = EntryType.Floating;

            bool expected = false;
            bool actual = UiTaskHelper.IsTaskOverdue(testData);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UiOngoingTestDeadlineTaskOngoingNow()
        {
            Entry testData = new Entry();
            testData.Type = EntryType.Deadline;
            testData.StartTime = DateTime.Now.AddSeconds(1); // Done to account for slowness in execution.

            bool expected = true;
            bool actual = UiTaskHelper.IsTaskOngoing(testData);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UiOngoingTestDeadlineTaskOngoingUnder24Hours()
        {
            Entry testData = new Entry();
            testData.Type = EntryType.Deadline;
            testData.StartTime = DateTime.Now.AddHours(23).AddMinutes(59);

            bool expected = true;
            bool actual = UiTaskHelper.IsTaskOngoing(testData);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UiOngoingTestDeadlineTaskNotOngoing24Hours()
        {
            Entry testData = new Entry();
            testData.Type = EntryType.Deadline;
            testData.StartTime = DateTime.Now.AddHours(24);

            bool expected = false;
            bool actual = UiTaskHelper.IsTaskOngoing(testData);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UiOngoingTestDeadlineTaskNotOngoingPast()
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
