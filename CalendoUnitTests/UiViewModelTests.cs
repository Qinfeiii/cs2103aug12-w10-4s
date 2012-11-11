//@author A0080860H
using System;
using Calendo;
using Calendo.Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CalendoUnitTests
{
    [TestClass]
    public class UiViewModelTests
    {
        private UiViewModel ViewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            ViewModel = new UiViewModel();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            ViewModel.SetSuggestions("");
        }

        [TestMethod]
        public void UiViewModelAutoSuggestRowTest()
        {
            // We need Auto Suggest to be returning a single
            // detail entry.
            ViewModel.SetSuggestions("/add Test");
            Assert.IsTrue(ViewModel.SuggestionList.Count == 1);
            Assert.IsTrue(!ViewModel.SuggestionList[0].IsMaster);

            int expected = 3;
            int actual = ViewModel.AutoSuggestRow;
            Assert.AreEqual(expected, actual);

            // Auto Suggest should now have more than one entry in it.
            ViewModel.SetSuggestions("/");
            Assert.IsTrue(ViewModel.SuggestionList.Count > 1);

            expected = 4;
            actual = ViewModel.AutoSuggestRow;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UiViewModelSorterTest()
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
                ID = 2,
                Meta = null,
                StartTime = DateTime.Now,
                StartTimeFormat = TimeFormat.None,
                Type = EntryType.Floating
            };

            Entry timedTask = new Entry()
            {
                Created = DateTime.Now,
                Description = "Normal Task",
                EndTime = DateTime.Now.AddDays(5),
                EndTimeFormat = TimeFormat.Date,
                ID = 3,
                Meta = null,
                StartTime = DateTime.Now.AddDays(3),
                StartTimeFormat = TimeFormat.Date,
                Type = EntryType.Timed
            };

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

            // Overdue versus everything else.
            int expected = -1;
            int actual = ViewModel.TaskListSorter(overdueTask, floatingTask);
            Assert.AreEqual(expected, actual);
            actual = ViewModel.TaskListSorter(overdueTask, timedTask);
            Assert.AreEqual(expected, actual);
            actual = ViewModel.TaskListSorter(overdueTask, deadlineTask);
            Assert.AreEqual(expected, actual);

            expected = 1;
            actual = ViewModel.TaskListSorter(floatingTask, overdueTask);
            Assert.AreEqual(expected, actual);
            actual = ViewModel.TaskListSorter(timedTask, overdueTask);
            Assert.AreEqual(expected, actual);
            actual = ViewModel.TaskListSorter(deadlineTask, overdueTask);
            Assert.AreEqual(expected, actual);

            // Floating versus everything else.
            expected = 1;
            actual = ViewModel.TaskListSorter(floatingTask, timedTask);
            Assert.AreEqual(expected, actual);
            actual = ViewModel.TaskListSorter(floatingTask, deadlineTask);
            Assert.AreEqual(expected, actual);

            expected = -1;
            actual = ViewModel.TaskListSorter(timedTask, floatingTask);
            Assert.AreEqual(expected, actual);
            actual = ViewModel.TaskListSorter(deadlineTask, floatingTask);
            Assert.AreEqual(expected, actual);

            // Two floating tasks. Tests sorting by description.
            Entry secondFloatingTask = new Entry()
            {
                Created = DateTime.Now,
                Description = "Another Floating Task",
                EndTime = DateTime.Now,
                EndTimeFormat = TimeFormat.None,
                ID = 5,
                Meta = null,
                StartTime = DateTime.Now,
                StartTimeFormat = TimeFormat.None,
                Type = EntryType.Floating
            };

            expected = -1;
            actual = ViewModel.TaskListSorter(secondFloatingTask, floatingTask);
            Assert.AreEqual(expected, actual);

            expected = 1;
            actual = ViewModel.TaskListSorter(floatingTask, secondFloatingTask);
            Assert.AreEqual(expected, actual);

            // Active task.
            DateTime tomorrow = DateTime.Now.AddDays(1);
            Entry activeTask = new Entry()
            {
                Created = DateTime.Now,
                Description = "Active Task",
                EndTime = DateTime.Now,
                EndTimeFormat = TimeFormat.None,
                ID = 6,
                Meta = null,
                StartTime = tomorrow,
                StartTimeFormat = TimeFormat.Date,
                Type = EntryType.Deadline
            };

            expected = -1;
            actual = ViewModel.TaskListSorter(activeTask, floatingTask);
            Assert.AreEqual(expected, actual);
            actual = ViewModel.TaskListSorter(activeTask, timedTask);
            Assert.AreEqual(expected, actual);

            expected = 1;
            actual = ViewModel.TaskListSorter(floatingTask, activeTask);
            Assert.AreEqual(expected, actual);
            actual = ViewModel.TaskListSorter(timedTask, activeTask);
            Assert.AreEqual(expected, actual);

            // Two active tasks.
            expected = -1;
            actual = ViewModel.TaskListSorter(deadlineTask, activeTask);
            Assert.AreEqual(expected, actual);
            expected = 1;
            actual = ViewModel.TaskListSorter(activeTask, deadlineTask);
            Assert.AreEqual(expected, actual);
        }
    }
}
