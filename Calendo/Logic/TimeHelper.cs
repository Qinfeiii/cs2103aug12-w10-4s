//@author A0080933E
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Calendo.Diagnostics;

namespace Calendo.Logic
{
    /// <summary>
    /// Hour format
    /// </summary>
    public enum HourFormat
    {
        Military = 0,
        AM = 1,
        PM = 2
    }

    /// <summary>
    /// Helper class for Time Converter
    /// </summary>
    public class TimeHelper
    {
        private const int INVALID_VALUE = -1;

        /// <summary>
        /// Is it a leap year?
        /// </summary>
        /// <param name="year">Year</param>
        /// <returns>True if it is a leap year, false otherwise</returns>
        private bool IsLeapYear(int year)
        {
            bool isLeap = false;
            if (year % 4 == 0)
            {
                isLeap = true;
            }
            if (year % 100 == 0)
            {
                isLeap = false;
            }
            if (year % 400 == 0)
            {
                isLeap = true;
            }
            return isLeap;
        }

        /// <summary>
        /// Get the maximum number of days in the specified month
        /// </summary>
        /// <param name="month">Month</param>
        /// <param name="year">Year</param>
        /// <returns>Maximum number of days</returns>
        private int MaxDaysInMonth(int month, int year)
        {
            if (month >= 1 && month <= 12 && year >= 0)
            {
                // Max days of each month
                int[] maxDays = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
                if (IsLeapYear(year))
                {
                    // February has an extra day
                    maxDays[1] = 29;
                }
                return maxDays[month - 1];
            }
            else
            {
                return INVALID_VALUE;
            }
        }

        /// <summary>
        /// Convert hour to 24 hour format
        /// </summary>
        /// <param name="hour">Hour in integer</param>
        /// <param name="hourFormat">Hour Format</param>
        /// <returns>Hour in 24 hour clock</returns>
        private int ConvertHour(int hour, HourFormat hourFormat)
        {
            bool isAM = (hourFormat == HourFormat.AM);
            bool isPM = (hourFormat == HourFormat.PM);

            if (hour == 12 && isAM)
            {
                // 12 Midnight case
                hour = 0;
            }
            else if (hour == 12 && isPM)
            {
                // 12 Noon case
                hour = 12;
            }
            else if (hour > 0 && isPM)
            {
                // 1 to 11PM case
                hour += 12;
            }
            else if (hour == 0 && isPM)
            {
                // 0 PM case (0 AM is valid)
                hour = INVALID_VALUE;
            }

            Debug.Assert(hour <= 24);
            return hour;
        }

        /// <summary>
        /// Gets the hour format
        /// </summary>
        /// <param name="timeString">String representation of hour</param>
        /// <returns>Hour format</returns>
        public HourFormat GetHourFormat(ref string timeString)
        {
            Debug.Assert(timeString != null);

            HourFormat hourFormat = HourFormat.Military;
            if (timeString.Length >= 2)
            {
                // Get last 2 letters
                string timeMeta = GetSubstring(timeString, timeString.Length - 2);
                timeMeta = timeMeta.ToUpper();

                if (timeMeta == "AM")
                {
                    hourFormat = HourFormat.AM;
                }
                if (timeMeta == "PM")
                {
                    hourFormat = HourFormat.PM;
                }
            }

            if (hourFormat != HourFormat.Military)
            {
                // Get the remainder excluding AM and PM
                timeString = timeString.Substring(0, timeString.Length - 2);
                timeString = timeString.Trim();
            }
            return hourFormat;
        }

        /// <summary>
        /// Gets the year from string
        /// </summary>
        /// <param name="yearFragment">String representation of year</param>
        /// <returns>Year in numeric format</returns>
        public int GetYear(string yearFragment)
        {
            int year = ConvertValue(yearFragment, 0, 9999);
            int century = DateTime.Today.Year / 100;

            if (year < 100)
            {
                // 2 digit year specified
                year += century * 100;
            }
            if (year < 1000)
            {
                year = INVALID_VALUE;
            }
            return year;
        }

        /// <summary>
        /// Gets the month from string
        /// </summary>
        /// <param name="monthFragment">String representation of month</param>
        /// <returns>Numeric representation of month</returns>
        public int GetMonth(string monthFragment)
        {
            return ConvertValue(monthFragment, 1, 12);
        }

        /// <summary>
        /// Gets the day from string
        /// </summary>
        /// <param name="dayFragment">String representation of day</param>
        /// <param name="year">Year</param>
        /// <param name="month">Month</param>
        /// <returns>Numeric representation of day</returns>
        public int GetDay(string dayFragment, int year = 0, int month = 0)
        {
            int maxDays = MaxDaysInMonth(month, year);
            int day = INVALID_VALUE;
            if (maxDays > 0)
            {
                day = ConvertValue(dayFragment, 1, maxDays);
            }
            return day;
        }

        /// <summary>
        /// Gets the hour from string
        /// </summary>
        /// <param name="hourFragment">String representation of hour</param>
        /// <param name="hourFormat">Hour format</param>
        /// <returns>Numeric representation of hour</returns>
        public int GetHour(string hourFragment, HourFormat hourFormat = HourFormat.Military)
        {
            int maxHour = 24;
            if (hourFormat != HourFormat.Military)
            {
                maxHour = 12;
            }
            int hour = ConvertValue(hourFragment, 0, maxHour);
            hour = ConvertHour(hour, hourFormat);
            return hour;
        }

        /// <summary>
        /// Gets the minutes from string
        /// </summary>
        /// <param name="minuteFragment">String representation of minutes</param>
        /// <returns>Numeric representation of minutes</returns>
        public int GetMinute(string minuteFragment)
        {
            return ConvertValue(minuteFragment, 0, 59);
        }

        /// <summary>
        /// Converts a string to a non-negative integer
        /// </summary>
        /// <param name="str">Integer in string format</param>
        /// <returns>Converted numeric value or -1 if conversion failed</returns>
        private int ConvertInt(string str)
        {
            try
            {
                int convertedValue = int.Parse(str);
                if (convertedValue < INVALID_VALUE)
                {
                    convertedValue = INVALID_VALUE;
                }
                return convertedValue;
            }
            catch
            {
                return INVALID_VALUE;
            }
        }

        /// <summary>
        /// Attempts to convert a value to an integer
        /// </summary>
        /// <param name="value">Value to be converted</param>
        /// <param name="lowerBound">Converted value should be at least this amount</param>
        /// <param name="upperBound">Converted value should be at most this amount</param>
        /// <returns>Converted value, otherwise -1 on failure</returns>
        private int ConvertValue(string value, int lowerBound, int upperBound)
        {
            Debug.Assert(lowerBound > INVALID_VALUE);
            Debug.Assert(upperBound >= lowerBound);

            int convertedValue = ConvertInt(value);
            if (convertedValue < lowerBound)
            {
                return INVALID_VALUE;
            }
            if (convertedValue > upperBound)
            {
                return INVALID_VALUE;
            }
            return convertedValue;
        }

        /// <summary>
        /// Gets the substring spanning from start to end of string
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="start">Start of substring</param>
        /// <returns>Substring spanning from start to end</returns>
        private string GetSubstring(string input, int start)
        {
            Debug.Assert(input != null);
            Debug.Assert(start >= 0);

            if (input.Length >= start)
            {
                input = input.Substring(start);
            }
            return input;
        }
    }
}
