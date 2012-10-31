//@author Jerome
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Calendo.Logic;

namespace Calendo
{
    public class UiTaskHelper
    {
        public static bool IsTaskOverdue(Entry currentEntry)
        {
            bool isOverdue = false;
            DateTime relevantTime;

            if (currentEntry.Type == EntryType.TIMED)
            {
                relevantTime = currentEntry.EndTime;
            }
            else if (currentEntry.Type == EntryType.DEADLINE)
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

            if (currentEntry.Type == EntryType.TIMED)
            {
                bool isStartBeforeNow = currentEntry.StartTime.CompareTo(DateTime.Now) < 0;
                bool isEndAfterNow = DateTime.Now.CompareTo(currentEntry.EndTime) < 0;

                bool isNowBetweenStartAndEnd = isStartBeforeNow && isEndAfterNow;
                isOngoing = isTaskStarting || isNowBetweenStartAndEnd;
            }
            else if (currentEntry.Type == EntryType.DEADLINE)
            {
                isOngoing = isTaskStarting;
            }

            // Floating tasks can't be ongoing.
            return isOngoing;
        }

        public static int CompareByDate(Entry first, Entry second)
        {
            // We assume the given tasks are either Timed or Deadline tasks.
            // Floating tasks have no date, and hence wouldn't need to be compared this way.
            bool isFirstTimed = first.Type == EntryType.TIMED;
            bool isSecondTimed = second.Type == EntryType.TIMED;

            bool isFirstOverdue = IsTaskOverdue(first);
            bool isSecondOverdue = IsTaskOverdue(second);

            DateTime firstRelevantTime;
            DateTime secondRelevantTime;

            if (isFirstTimed && isFirstOverdue)
            {
                firstRelevantTime = first.EndTime;
            }
            else
            {
                firstRelevantTime = first.StartTime;
            }

            if (isSecondTimed && isSecondOverdue)
            {
                secondRelevantTime = second.EndTime;
            }
            else
            {
                secondRelevantTime = second.StartTime;
            }

            int firstCompareSecond = firstRelevantTime.CompareTo(secondRelevantTime);
            return firstCompareSecond;
        }

        public static int CompareByDescription(Entry first, Entry second)
        {
            return first.Description.CompareTo(second.Description);
        }

        public static bool IsTaskFloating(Entry task)
        {
            return task.Type == EntryType.FLOATING;
        }
    }
}
