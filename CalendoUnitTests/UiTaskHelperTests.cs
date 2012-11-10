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

        [TestMethod]
        public void UiCompareTaskFloatingOverdue()
        {
            DateTime yesterday = DateTime.Now.Subtract(TimeSpan.FromDays(1));
            Entry overdueTask = new Entry()
            {
                Created = yesterday,
                Description = "Overdue Task",
                EndTime = yesterday,
                EndTimeFormat = TimeFormat.None,
                ID = 1,
                Meta = null,
                StartTime = yesterday,
                StartTimeFormat = TimeFormat.Date,
                Type = EntryType.Deadline
            };

            Entry floatingTask = new Entry()
            {
                Created = DateTime.Now,
                Description = "Floating Task",
                EndTime = DateTime.Now,
                EndTimeFormat = TimeFormat.None,
                ID = 0,
                Meta = null,
                StartTime = DateTime.Now,
                StartTimeFormat = TimeFormat.None,
                Type = EntryType.Floating
            };

            int expected = 1;
            int actual = UiTaskHelper.Compare(floatingTask, overdueTask);
            Assert.AreEqual(expected, actual);

            expected = -1;
            actual = UiTaskHelper.Compare(overdueTask, floatingTask);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UiCompareTaskBothFloating()
        {
            Entry firstFloatingTask = new Entry()
            {
                Created = DateTime.Now,
                Description = "A Floating Task",
                EndTime = DateTime.Now,
                EndTimeFormat = TimeFormat.None,
                ID = 0,
                Meta = null,
                StartTime = DateTime.Now,
                StartTimeFormat = TimeFormat.None,
                Type = EntryType.Floating
            };

            Entry secondFloatingTask = new Entry()
            {
                Created = DateTime.Now,
                Description = "Second Floating Task",
                EndTime = DateTime.Now,
                EndTimeFormat = TimeFormat.None,
                ID = 1,
                Meta = null,
                StartTime = DateTime.Now,
                StartTimeFormat = TimeFormat.None,
                Type = EntryType.Floating
            };

            int expected = -1;
            int actual = UiTaskHelper.Compare(firstFloatingTask, secondFloatingTask);
            Assert.AreEqual(expected, actual);

            expected = 1;
            actual = UiTaskHelper.Compare(secondFloatingTask, firstFloatingTask);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UiCompareTaskDifferentDates()
        {
            Entry firstTask = new Entry()
            {
                Created = DateTime.Now,
                Description = "Normal Task",
                EndTime = DateTime.Now.AddDays(5),
                EndTimeFormat = TimeFormat.Date,
                ID = 0,
                Meta = null,
                StartTime = DateTime.Now.AddDays(3),
                StartTimeFormat = TimeFormat.Date,
                Type = EntryType.Timed
            };

            Entry secondTask = new Entry()
            {
                Created = DateTime.Now,
                Description = "Normal Task",
                EndTime = DateTime.Now.AddDays(6),
                EndTimeFormat = TimeFormat.Date,
                ID = 1,
                Meta = null,
                StartTime = DateTime.Now.AddDays(4),
                StartTimeFormat = TimeFormat.Date,
                Type = EntryType.Timed
            };

            int expected = -1;
            int actual = UiTaskHelper.Compare(firstTask, secondTask);
            Assert.AreEqual(expected, actual);

            expected = 1;
            actual = UiTaskHelper.Compare(secondTask, firstTask);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UiCompareTaskSameDates()
        {
            Entry firstTask = new Entry()
            {
                Created = DateTime.Now,
                Description = "A Normal Task",
                EndTime = DateTime.Now.AddDays(5),
                EndTimeFormat = TimeFormat.Date,
                ID = 0,
                Meta = null,
                StartTime = DateTime.Now.AddDays(3),
                StartTimeFormat = TimeFormat.Date,
                Type = EntryType.Timed
            };

            Entry secondTask = new Entry()
            {
                Created = DateTime.Now,
                Description = "Second Task",
                EndTime = DateTime.Now.AddDays(5),
                EndTimeFormat = TimeFormat.Date,
                ID = 1,
                Meta = null,
                StartTime = DateTime.Now.AddDays(3),
                StartTimeFormat = TimeFormat.Date,
                Type = EntryType.Timed
            };

            int expected = -1;
            int actual = UiTaskHelper.Compare(firstTask, secondTask);
            Assert.AreEqual(expected, actual);

            expected = 1;
            actual = UiTaskHelper.Compare(secondTask, firstTask);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UiTaskFloating()
        {
            Entry floatingTask = new Entry()
            {
                Created = DateTime.Now,
                Description = "Floating Task",
                EndTime = DateTime.Now,
                EndTimeFormat = TimeFormat.None,
                ID = 1,
                Meta = null,
                StartTime = DateTime.Now,
                StartTimeFormat = TimeFormat.None,
                Type = EntryType.Floating
            };

            bool expected = true;
            bool actual = UiTaskHelper.IsTaskFloating(floatingTask);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UiTaskFloatingDeadline()
        {
            Entry deadlineTask = new Entry()
            {
                Created = DateTime.Now,
                Description = "Deadline Task",
                EndTime = DateTime.Now,
                EndTimeFormat = TimeFormat.None,
                ID = 1,
                Meta = null,
                StartTime = DateTime.Now,
                StartTimeFormat = TimeFormat.Date,
                Type = EntryType.Deadline
            };

            bool expected = false;
            bool actual = UiTaskHelper.IsTaskFloating(deadlineTask);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UiTaskFloatingTimed()
        {
            Entry timedTask = new Entry()
            {
                Created = DateTime.Now,
                Description = "Timed Task",
                EndTime = DateTime.Now.AddDays(6),
                EndTimeFormat = TimeFormat.Date,
                ID = 1,
                Meta = null,
                StartTime = DateTime.Now,
                StartTimeFormat = TimeFormat.Date,
                Type = EntryType.Timed
            };

            bool expected = false;
            bool actual = UiTaskHelper.IsTaskFloating(timedTask);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UiTimedTaskOngoing()
        {
            Entry ongoingTask = new Entry()
            {
                Created = DateTime.Now,
                Description = "Ongoing Task",
                EndTime = DateTime.Now,
                EndTimeFormat = TimeFormat.None,
                ID = 1,
                Meta = null,
                StartTime = DateTime.Now,
                StartTimeFormat = TimeFormat.Date,
                Type = EntryType.Timed
            };

            bool expected = true;
            bool actual = UiTaskHelper.IsTaskOngoing(ongoingTask);
            Assert.AreEqual(expected, actual);


            DateTime justBeforeNonActive = DateTime.Now.AddHours(23.99);

            ongoingTask = new Entry()
            {
                Created = DateTime.Now,
                Description = "Barely Ongoing Task",
                EndTime = DateTime.Now,
                EndTimeFormat = TimeFormat.None,
                ID = 0,
                Meta = null,
                StartTime = justBeforeNonActive,
                StartTimeFormat = TimeFormat.Date,
                Type = EntryType.Timed     
            };

            expected = true;
            actual = UiTaskHelper.IsTaskOngoing(ongoingTask);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UiTimedTaskNotOngoing()
        {
            Entry normalTask = new Entry()
            {
                Created = DateTime.Now,
                Description = "Barely Normal Task",
                EndTime = DateTime.Now,
                EndTimeFormat = TimeFormat.None,
                ID = 0,
                Meta = null,
                StartTime = DateTime.Now.AddDays(1),
                StartTimeFormat = TimeFormat.Date,
                Type = EntryType.Timed
            };

            bool expected = false;
            bool actual = UiTaskHelper.IsTaskOngoing(normalTask);
            Assert.AreEqual(expected, actual);

            normalTask = new Entry()
            {
                Created = DateTime.Now,
                Description = "Normal Task",
                EndTime = DateTime.Now.AddDays(6),
                EndTimeFormat = TimeFormat.Date,
                ID = 0,
                Meta = null,
                StartTime = DateTime.Now.AddDays(5),
                StartTimeFormat = TimeFormat.Date,
                Type = EntryType.Timed
            };

            expected = false;
            actual = UiTaskHelper.IsTaskOngoing(normalTask);
            Assert.AreEqual(expected, actual);
        }
    }
}
