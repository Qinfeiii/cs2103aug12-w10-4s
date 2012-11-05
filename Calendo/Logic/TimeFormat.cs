//@author A0080933E
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calendo.Logic
{
    /// <summary>
    /// Time Format
    /// </summary>
    public enum TimeFormat
    {
        None = 0,
        Date = 1,
        Time = 2,
        DateTime = 3
    }

    public static class TimeFormatExtensions
    {
        /// <summary>
        /// Add date to format
        /// </summary>
        /// <param name="format">Time Format</param>
        /// <returns></returns>
        public static TimeFormat AddDate(this TimeFormat format)
        {
            format |= TimeFormat.Date;
            return format;
        }

        /// <summary>
        /// Add time to format
        /// </summary>
        /// <param name="format">Time Format</param>
        /// <returns></returns>
        public static TimeFormat AddTime(this TimeFormat format)
        {
            format |= TimeFormat.Time;
            return format;
        }

        /// <summary>
        /// Remove Date from format
        /// </summary>
        /// <param name="format">Time Format</param>
        /// <returns></returns>
        public static TimeFormat RemoveDate(this TimeFormat format)
        {
            format |= TimeFormat.Date;
            format ^= TimeFormat.Date;
            return format;
        }

        /// <summary>
        /// Remove time from format
        /// </summary>
        /// <param name="format">Time Format</param>
        /// <returns></returns>
        public static TimeFormat RemoveTime(this TimeFormat format)
        {
            format |= TimeFormat.Time;
            format ^= TimeFormat.Time;
            return format;
        }
    }
}
