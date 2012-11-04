//@author Jerome
using System;
using System.Diagnostics;
using Calendo.Logic;

namespace Calendo
{
    public class UiTaskHelper
    {
        public static bool IsTaskOverdue(Entry currentEntry)
        {
            bool isOverdue = false;
            DateTime relevantTime;

            if (currentEntry.Type == EntryType.Timed)
            {
                relevantTime = currentEntry.EndTime;
            }
            else if (currentEntry.Type == EntryType.Deadline)
            {
                relevantTime = currentEntry.StartTime;
            }
            else
            {
                // Floating tasks can't be overdue.
                return false;
            }
            isOverdue = relevantTime.CompareTo(DateTime.Now) < 0;
            return isOverdue;
        }

        public static bool IsTaskOngoing(Entry currentEntry)
        {
            bool isOngoing = false;

            TimeSpan nowAndTaskStartDifference = currentEntry.StartTime.Subtract(DateTime.Now);

            bool isTaskDifferencePositive = 0 <= nowAndTaskStartDifference.TotalHours;
            bool isTaskADayAway = nowAndTaskStartDifference.TotalHours < 24;

            bool isTaskStarting = isTaskDifferencePositive && isTaskADayAway;

            if (currentEntry.Type == EntryType.Timed)
            {
                bool isStartBeforeNow = currentEntry.StartTime.CompareTo(DateTime.Now) < 0;
                bool isEndAfterNow = DateTime.Now.CompareTo(currentEntry.EndTime) < 0;

                bool isNowBetweenStartAndEnd = isStartBeforeNow && isEndAfterNow;
                isOngoing = isTaskStarting || isNowBetweenStartAndEnd;
            }
            else if (currentEntry.Type == EntryType.Deadline)
            {
                isOngoing = isTaskStarting;
            }

            // Floating tasks can't be ongoing.
            return isOngoing;
        }

        public static int CompareByStartTime(Entry first, Entry second)
        {
            // The given tasks must be either Timed or Deadline tasks.
            // Floating tasks have no start time, and so can't be compared this way.
            Debug.Assert(first.Type != EntryType.Floating && second.Type != EntryType.Floating);

            return first.StartTime.CompareTo(second.StartTime);
        }

        public static int CompareByEndTime(Entry first, Entry second)
        {
            // The given tasks must be either Timed or Deadline tasks.
            // Floating tasks have no end time, and so can't be compared this way.
            Debug.Assert(first.Type != EntryType.Floating && second.Type != EntryType.Floating);

            return first.EndTime.CompareTo(second.EndTime);
        }

        public static int Compare(Entry first, Entry second)
        {
            bool isFirstFloating = first.Type == EntryType.Floating;
            bool isSecondFloating = second.Type == EntryType.Floating;
            int comparisonByDescription = CompareByDescription(first, second);

            if (isFirstFloating && isSecondFloating)
            {
                return comparisonByDescription;
            }
            else if (isFirstFloating && !isSecondFloating)
            {
                return 1;
            }
            else if (!isFirstFloating && isSecondFloating)
            {
                return -1;
            }

            int comparisonByStartTime = CompareByStartTime(first, second);
            int comparisonByEndTime = CompareByEndTime(first, second);

            if (comparisonByStartTime == 0 && comparisonByEndTime == 0)
            {
                return comparisonByDescription;
            }
            else if (comparisonByStartTime != 0)
            {
                return comparisonByStartTime;
            }
            return comparisonByEndTime;
        }

        public static int CompareByDescription(Entry first, Entry second)
        {
            int descriptionComparisonValue = first.Description.CompareTo(second.Description);
            return descriptionComparisonValue;
        }

        public static bool IsTaskFloating(Entry task)
        {
            bool isTaskFloating = task.Type == EntryType.Floating;
            return isTaskFloating;
        }
    }
}
