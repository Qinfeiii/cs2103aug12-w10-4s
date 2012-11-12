//@author A0080933E
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Calendo.Data;
using Calendo.Diagnostics;

namespace Calendo.Logic
{
    public class TimeConverter
    {
        private const string ERROR_INVALID_DATE = "Invalid Date specified";
        private const string ERROR_INVALID_TIME = "Invalid Time specified";
        private const int INVALID_VALUE = -1;
        private readonly string[] KEYWORDS = new string[] { "-" };
        private readonly char[] DELIMITTER_DATE = new char[] { '.', '/', '-' };
        private readonly char[] DELIMITTER_TIME = new char[] { ':', '.' };
        private TimeHelper timeHelper = new TimeHelper();

        /// <summary>
        /// Gets the TimeFormat associated with the date and time
        /// </summary>
        /// <param name="hasDate">Format has a date</param>
        /// <param name="hasTime">Format has a time</param>
        /// <returns>Returns TimeFormat value</returns>
        private TimeFormat GetFormat(bool hasDate, bool hasTime)
        {
            TimeFormat newTimeFormat = TimeFormat.None;
            if (hasDate)
            {
                newTimeFormat = newTimeFormat.AddDate();
            }
            if (hasTime)
            {
                newTimeFormat = newTimeFormat.AddTime();
            }
            return newTimeFormat;
        }

        /// <summary>
        /// Converts a string date and time to DateTime object
        /// </summary>
        /// <param name="date">Date in Day/Month/Year</param>
        /// <param name="time">Time in Hour/Minutes (24 hour)</param>
        /// <returns>Returns TaskTime object</returns>
        public TaskTime Convert(string date, string time)
        {
            date = SanitizeString(date);
            time = SanitizeString(time);

            bool isValidDate = true;
            bool isValidTime = true;
            bool hasError = false;
            StringBuilder errorMessage = new StringBuilder();

            DateTime convertedTime = DateTime.Today;
            ConvertDate(date, ref convertedTime, ref errorMessage, ref isValidDate);
            ConvertTime(time, ref convertedTime, ref errorMessage, ref isValidTime);

            if (errorMessage.Length > 0)
            {
                hasError = true;
                DebugTool.Alert(errorMessage.ToString());
            }

            TaskTime taskDuration = new TaskTime();
            taskDuration.Format = GetFormat(isValidDate, isValidTime);

            if (convertedTime < DateTime.Now)
            {
                if (taskDuration.Format == TimeFormat.Time)
                {
                    // Date is actually the next day
                    convertedTime = convertedTime.AddDays(1);
                    taskDuration.Format = TimeFormat.DateTime;
                }
            }
            taskDuration.Time = convertedTime;
            taskDuration.HasError = hasError;
            return taskDuration;
        }

        /// <summary>
        /// Replace null strings with a default
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>Returns string if not null, empty string if null</returns>
        public static string SanitizeString(string input)
        {
            string convertedString = "";
            if (input != null)
            {
                convertedString = input;
            }

            Debug.Assert(convertedString != null);
            return convertedString;
        }

        /// <summary>
        /// Determines if the input string has a keyword
        /// </summary>
        /// <param name="inputString">Input string</param>
        /// <returns>Returns true if contains a keyword</returns>
        private bool HasKeyword(string inputString)
        {
            inputString = inputString.Trim();
            foreach (string keyword in KEYWORDS)
            {
                if (inputString == keyword)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Converts time to DateTime
        /// </summary>
        /// <param name="timeString">Input time</param>
        /// <param name="newTime">DateTime object to modify</param>
        /// <param name="hasError">Has Error</param>
        /// <param name="isValidTime">Is Valid Time</param>
        private void ConvertTime(string timeString, ref DateTime newTime, ref StringBuilder errorMessage, ref bool isValidTime)
        {
            Debug.Assert(timeString != null);

            if (HasKeyword(timeString))
            {
                newTime = new DateTime(0);
                isValidTime = true;
                return;
            }

            int hour = INVALID_VALUE;
            int minute = 0;

            // Handle AM or PM
            HourFormat hourFormat = timeHelper.GetHourFormat(ref timeString);

            string[] timeFragment = timeString.Split(DELIMITTER_TIME, 2);

            // Hour
            if (timeFragment.Length > 0)
            {
                hour = timeHelper.GetHour(timeFragment[0], hourFormat);
            }
            // Minute
            if (timeFragment.Length > 1)
            {
                minute = timeHelper.GetMinute(timeFragment[1]);
            }

            if (timeString == "")
            {
                isValidTime = false;
            }
            else if (IsInvalid(hour, minute))
            {
                // There are errors
                errorMessage.AppendLine(ERROR_INVALID_TIME);
                isValidTime = false;
            }

            if (isValidTime)
            {
                newTime = newTime.AddHours(hour);
                newTime = newTime.AddMinutes(minute);
            }
        }

        /// <summary>
        /// Converts date to DateTime
        /// </summary>
        /// <param name="time">Input date</param>
        /// <param name="newTime">DateTime object to modify</param>
        /// <param name="errorMessage">Has Error</param>
        /// <param name="isValidTime">Is Valid Time</param>
        private void ConvertDate(string dateString, ref DateTime newDate, ref StringBuilder errorMessage, ref bool isValidDate)
        {
            Debug.Assert(dateString != null);

            if (HasKeyword(dateString))
            {
                newDate = new DateTime(0);
                isValidDate = true;
                return;
            }

            bool isYearProvided = false;
            int year = DateTime.Today.Year;
            int day = INVALID_VALUE;
            int month = INVALID_VALUE;

            // Date: Day/Month[/Year]
            string[] dateFragment = dateString.Split(DELIMITTER_DATE, 3);
            // Year
            if (dateFragment.Length > 2)
            {
                year = timeHelper.GetYear(dateFragment[2]);
                isYearProvided = true;
            }
            // Month
            if (dateFragment.Length > 1)
            {
                month = timeHelper.GetMonth(dateFragment[1]);
            }
            // Day
            if (dateFragment.Length > 0)
            {
                day = timeHelper.GetDay(dateFragment[0], year, month);
            }

            if (dateString == "")
            {
                isValidDate = false;
            }
            else if (IsInvalid(year, month, day))
            {
                // There are errors
                errorMessage.AppendLine(ERROR_INVALID_DATE);
                isValidDate = false;
            }

            if (isValidDate)
            {
                newDate = new DateTime(year, month, day);
                if (!isYearProvided && newDate < DateTime.Today)
                {
                    // Event occurs next year
                    newDate = newDate.AddYears(1);
                }
            }
        }

        /// <summary>
        /// Checks if any value is invalid
        /// </summary>
        /// <param name="values">Integer values to check</param>
        /// <returns>Returns true if at least one value is invalid</returns>
        private bool IsInvalid(params int[] values)
        {
            foreach (int value in values)
            {
                if (value == INVALID_VALUE)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Merge changes in times
        /// </summary>
        /// <param name="source">Source time</param>
        /// <param name="destination">Target time</param>
        /// <param name="modifyFlag">Modify Flag</param>
        /// <returns>Returns merged times</returns>
        public static TaskTime MergeTime(TaskTime source, TaskTime destination, ModifyFlag modifyFlag)
        {
            int day = destination.Time.Day;
            int month = destination.Time.Month;
            int year = destination.Time.Year;
            int minute = destination.Time.Minute;
            int hour = destination.Time.Hour;
            TimeFormat format = destination.Format;

            // Override changed fields
            if (modifyFlag.Contains(ModifyFlag.StartDate | ModifyFlag.EndDate))
            {
                day = source.Time.Day;
                month = source.Time.Month;
                year = source.Time.Year;
                format = format.AddDate();
            }
            if (modifyFlag.Contains(ModifyFlag.StartTime | ModifyFlag.EndTime))
            {
                minute = source.Time.Minute;
                hour = source.Time.Hour;
                format = format.AddTime();
            }
            if (modifyFlag.Contains(ModifyFlag.EraseStartDate | ModifyFlag.EraseEndDate))
            {
                format = format.RemoveDate();
            }
            if (modifyFlag.Contains(ModifyFlag.EraseStartTime | ModifyFlag.EraseEndTime))
            {
                format = format.RemoveTime();
            }

            DateTime newTime = new DateTime(year, month, day, hour, minute, 0);
            TaskTime mergedTime = new TaskTime(newTime, format);

            // Carry over error
            mergedTime.HasError = source.HasError || destination.HasError;

            return mergedTime;
        }
    }
}
