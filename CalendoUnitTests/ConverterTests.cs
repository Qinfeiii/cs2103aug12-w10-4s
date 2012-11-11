//@author Jerome

using System;
using System.Windows;
using System.Windows.Media;
using Calendo.Converters;
using Calendo.Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CalendoUnitTests
{
    [TestClass]
    public class ConverterTests
    {
        private StringArrayToStringConverter ArrayToStringConverter;
        private EntryToDateTimeStringConverter EntryToDateTimeStringConverter;
        private EntryToBrushConverter EntryToBrushConverter;
        private EntryToDateTimeVisibilityConverter EntryToDateTimeVisibilityConverter;

        [TestInitialize]
        public void TestInitialize()
        {
            ArrayToStringConverter = new StringArrayToStringConverter();
            EntryToDateTimeStringConverter = new EntryToDateTimeStringConverter();
            EntryToBrushConverter = new EntryToBrushConverter();
            EntryToDateTimeVisibilityConverter = new EntryToDateTimeVisibilityConverter();
        }

        [TestMethod]
        public void UiStringArrayToStringTestValidArray()
        {
            string[] testData = new string[] { "first", "second", "third", "fourth" };
            string expected = "second, third, fourth";
            string actual = ArrayToStringConverter.Convert(testData, testData.GetType(), null, null) as string;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UiStringArrayToStringTestNullArray()
        {
            string expected = "none";
            string actual = ArrayToStringConverter.Convert(null, null, null, null) as string;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UiEntryToDateTimeStringTestStartDateToday()
        {
            Entry testEntry = new Entry()
                                  {
                                      Created = DateTime.Now,
                                      Description = "Test Entry",
                                      EndTime = DateTime.Now,
                                      EndTimeFormat = TimeFormat.Date,
                                      ID = 0,
                                      Meta = null,
                                      StartTime = DateTime.Now,
                                      StartTimeFormat = TimeFormat.Date,
                                      Type = EntryType.Deadline
                                  };

            string expected = "Today";
            string actual = EntryToDateTimeStringConverter.Convert(testEntry, testEntry.GetType(), "StartDate", null) as string;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UiEntryToDateTimeStringTestStartDateTomorrow()
        {
            Entry testEntry = new Entry()
                                  {
                                      Created = DateTime.Now,
                                      Description = "Test Entry",
                                      EndTime = DateTime.Now,
                                      EndTimeFormat = TimeFormat.Date,
                                      ID = 0,
                                      Meta = null,
                                      StartTime = DateTime.Now.AddDays(1),
                                      StartTimeFormat = TimeFormat.Date,
                                      Type = EntryType.Deadline
                                  };

            string expected = "Tomorrow";
            string actual = EntryToDateTimeStringConverter.Convert(testEntry, testEntry.GetType(), "StartDate", null) as string;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UiEntryToDateTimeStringTestTime()
        {
            Entry testEntry = new Entry()
                                  {
                                      Created = DateTime.Now,
                                      Description = "Test Entry",
                                      EndTime = DateTime.Now,
                                      EndTimeFormat = TimeFormat.Date,
                                      ID = 0,
                                      Meta = null,
                                      StartTime = new DateTime(2012, 12, 01, 13, 50, 0),
                                      StartTimeFormat = TimeFormat.Time,
                                      Type = EntryType.Deadline
                                  };

            string expected = "13:50";
            string actual =
                EntryToDateTimeStringConverter.Convert(testEntry, testEntry.GetType(), "StartDate", null) as string;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UiEntryToDateTimeStringTestDateTime()
        {
            Entry testEntry = new Entry()
                                  {
                                      Created = DateTime.Now,
                                      Description = "Test Entry",
                                      EndTime = DateTime.Now,
                                      EndTimeFormat = TimeFormat.None,
                                      ID = 0,
                                      Meta = null,
                                      StartTime = new DateTime(2012, 12, 01, 13, 50, 0),
                                      StartTimeFormat = TimeFormat.DateTime,
                                      Type = EntryType.Deadline
                                  };

            string expected = "01/12/2012 13:50";
            string actual =
                EntryToDateTimeStringConverter.Convert(testEntry, testEntry.GetType(), "StartDate", null) as string;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UiEntryToDateTimeStringTestDateTimeEndDate()
        {
            Entry testEntry = new Entry()
            {
                Created = DateTime.Now,
                Description = "Test Entry",
                EndTime = new DateTime(2012, 12, 01, 13, 50, 0),
                EndTimeFormat = TimeFormat.DateTime,
                ID = 0,
                Meta = null,
                StartTime = DateTime.Now,
                StartTimeFormat = TimeFormat.None,
                Type = EntryType.Deadline
            };

            string expected = "01/12/2012 13:50";
            string actual =
                EntryToDateTimeStringConverter.Convert(testEntry, testEntry.GetType(), "EndDate", null) as string;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UiEntryToDateTimeStringTestDateTimeToday()
        {
            int currentYear = DateTime.Now.Year;
            int currentMonth = DateTime.Now.Month;
            int currentDay = DateTime.Now.Day;

            DateTime today = new DateTime(currentYear, currentMonth, currentDay, 13, 50, 0);

            Entry testEntry = new Entry()
            {
                Created = DateTime.Now,
                Description = "Test Entry",
                EndTime = DateTime.Now,
                EndTimeFormat = TimeFormat.None,
                ID = 0,
                Meta = null,
                StartTime = today,
                StartTimeFormat = TimeFormat.DateTime,
                Type = EntryType.Deadline
            };

            string expected = "Today 13:50";
            string actual =
                EntryToDateTimeStringConverter.Convert(testEntry, testEntry.GetType(), "StartDate", null) as string;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UiEntryToDateTimeStringTestDateTimeTomorrow()
        {
            int currentYear = DateTime.Now.Year;
            int currentMonth = DateTime.Now.Month;
            int currentDay = DateTime.Now.Day;

            DateTime today = new DateTime(currentYear, currentMonth, currentDay, 13, 50, 0);
            DateTime tomorrow = today.AddDays(1);

            Entry testEntry = new Entry()
            {
                Created = DateTime.Now,
                Description = "Test Entry",
                EndTime = DateTime.Now,
                EndTimeFormat = TimeFormat.None,
                ID = 0,
                Meta = null,
                StartTime = tomorrow,
                StartTimeFormat = TimeFormat.DateTime,
                Type = EntryType.Deadline
            };

            string expected = "Tomorrow 13:50";
            string actual =
                EntryToDateTimeStringConverter.Convert(testEntry, testEntry.GetType(), "StartDate", null) as string;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UiEntryToDateTimeStringTestDate()
        {
            Entry testEntry = new Entry()
            {
                Created = DateTime.Now,
                Description = "Test Entry",
                EndTime = DateTime.Now,
                EndTimeFormat = TimeFormat.None,
                ID = 0,
                Meta = null,
                StartTime = new DateTime(2012, 12, 01),
                StartTimeFormat = TimeFormat.Date,
                Type = EntryType.Deadline
            };

            string expected = "01/12/2012";
            string actual =
                EntryToDateTimeStringConverter.Convert(testEntry, testEntry.GetType(), "StartDate", null) as string;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UiEntryToDateTimeStringTestNone()
        {
            Entry testEntry = new Entry()
            {
                Created = DateTime.Now,
                Description = "Test Entry",
                EndTime = DateTime.Now,
                EndTimeFormat = TimeFormat.None,
                ID = 0,
                Meta = null,
                StartTime = DateTime.Now,
                StartTimeFormat = TimeFormat.None,
                Type = EntryType.Deadline
            };

            string expected = "";
            string actual =
                EntryToDateTimeStringConverter.Convert(testEntry, testEntry.GetType(), "StartDate", null) as string;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UiEntryToBrushOngoing()
        {
            DateTime today = DateTime.Now;
            DateTime tomorrow = today.AddDays(1);

            Entry testEntry = new Entry()
            {
                Created = DateTime.Now,
                Description = "Test Entry",
                EndTime = DateTime.Now,
                EndTimeFormat = TimeFormat.None,
                ID = 0,
                Meta = null,
                StartTime = tomorrow,
                StartTimeFormat = TimeFormat.DateTime,
                Type = EntryType.Deadline
            };

            Brush expected = Brushes.Orange;
            Brush actual = EntryToBrushConverter.Convert(testEntry, testEntry.GetType(), null, null) as Brush;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UiEntryToBrushOverdue()
        {
            Entry testEntry = new Entry()
            {
                Created = DateTime.Now,
                Description = "Test Entry",
                EndTime = DateTime.Now,
                EndTimeFormat = TimeFormat.None,
                ID = 0,
                Meta = null,
                StartTime = DateTime.Now.Subtract(TimeSpan.FromDays(1)),
                StartTimeFormat = TimeFormat.DateTime,
                Type = EntryType.Deadline
            };

            Brush expected = Brushes.Red;
            Brush actual = EntryToBrushConverter.Convert(testEntry, testEntry.GetType(), null, null) as Brush;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UiEntryToBrushNormal()
        {
            Entry testEntry = new Entry()
            {
                Created = DateTime.Now,
                Description = "Test Entry",
                EndTime = DateTime.Now,
                EndTimeFormat = TimeFormat.None,
                ID = 0,
                Meta = null,
                StartTime = DateTime.Now.AddDays(2),
                StartTimeFormat = TimeFormat.DateTime,
                Type = EntryType.Deadline
            };

            BrushConverter converter = new BrushConverter();
            Brush expectedBrush = converter.ConvertFrom("#FF464646") as Brush;
            Brush actualBrush = EntryToBrushConverter.Convert(testEntry, testEntry.GetType(), null, null) as Brush;
            string expected = expectedBrush.ToString();
            string actual = actualBrush.ToString();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UiEntryToVisibilityStackPanelBothFormatsValid()
        {
            Entry testEntry = new Entry()
            {
                Created = DateTime.Now,
                Description = "Test Entry",
                EndTime = DateTime.Now.AddDays(2),
                EndTimeFormat = TimeFormat.Date,
                ID = 0,
                Meta = null,
                StartTime = DateTime.Now,
                StartTimeFormat = TimeFormat.Date,
                Type = EntryType.Deadline
            };

            Visibility expected = Visibility.Visible;
            Visibility actual =
                (Visibility)EntryToDateTimeVisibilityConverter.Convert(testEntry, testEntry.GetType(), "StackPanel", null);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UiEntryToVisibilityStackPanelStartNone()
        {
            Entry testEntry = new Entry()
            {
                Created = DateTime.Now,
                Description = "Test Entry",
                EndTime = DateTime.Now.AddDays(2),
                EndTimeFormat = TimeFormat.Date,
                ID = 0,
                Meta = null,
                StartTime = DateTime.Now,
                StartTimeFormat = TimeFormat.None,
                Type = EntryType.Deadline
            };

            Visibility expected = Visibility.Visible;
            Visibility actual =
                (Visibility)EntryToDateTimeVisibilityConverter.Convert(testEntry, testEntry.GetType(), "StackPanel", null);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UiEntryToVisibilityStackPanelEndNone()
        {
            Entry testEntry = new Entry()
            {
                Created = DateTime.Now,
                Description = "Test Entry",
                EndTime = DateTime.Now.AddDays(2),
                EndTimeFormat = TimeFormat.None,
                ID = 0,
                Meta = null,
                StartTime = DateTime.Now,
                StartTimeFormat = TimeFormat.Date,
                Type = EntryType.Deadline
            };

            Visibility expected = Visibility.Visible;
            Visibility actual =
                (Visibility)EntryToDateTimeVisibilityConverter.Convert(testEntry, testEntry.GetType(), "StackPanel", null);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UiEntryToVisibilityStackPanelBothNone()
        {
            Entry testEntry = new Entry()
            {
                Created = DateTime.Now,
                Description = "Test Entry",
                EndTime = DateTime.Now.AddDays(2),
                EndTimeFormat = TimeFormat.None,
                ID = 0,
                Meta = null,
                StartTime = DateTime.Now,
                StartTimeFormat = TimeFormat.None,
                Type = EntryType.Deadline
            };

            Visibility expected = Visibility.Collapsed;
            Visibility actual =
                (Visibility)EntryToDateTimeVisibilityConverter.Convert(testEntry, testEntry.GetType(), "StackPanel", null);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UiEntryToVisibilityRangeTextVisible()
        {
            Entry testEntry = new Entry()
            {
                Created = DateTime.Now,
                Description = "Test Entry",
                EndTime = DateTime.Now.AddDays(2),
                EndTimeFormat = TimeFormat.Date,
                ID = 0,
                Meta = null,
                StartTime = DateTime.Now,
                StartTimeFormat = TimeFormat.Date,
                Type = EntryType.Deadline
            };

            Visibility expected = Visibility.Visible;
            Visibility actual =
                (Visibility)EntryToDateTimeVisibilityConverter.Convert(testEntry, testEntry.GetType(), "RangeText", null);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UiEntryToVisibilityRangeTextCollapsed()
        {
            Entry testEntry = new Entry()
            {
                Created = DateTime.Now,
                Description = "Test Entry",
                EndTime = DateTime.Now.AddDays(2),
                EndTimeFormat = TimeFormat.None,
                ID = 0,
                Meta = null,
                StartTime = DateTime.Now,
                StartTimeFormat = TimeFormat.Date,
                Type = EntryType.Deadline
            };

            Visibility expected = Visibility.Collapsed;
            Visibility actual =
                (Visibility)EntryToDateTimeVisibilityConverter.Convert(testEntry, testEntry.GetType(), "RangeText", null);
            Assert.AreEqual(expected, actual);
        }
    }
}
