//@author A0080933E
using System;
using System.Collections.Generic;
using System.Text;

namespace Calendo.Logic
{
    public class TaskTime
    {
        /// <summary>
        /// Create a new instance of TaskTime
        /// </summary>
        public TaskTime()
        {
            this.Format = TimeFormat.None;
            this.Time = DateTime.Today;
            // Default TaskTime used as error triggers
            this.HasError = true;
        }

        /// <summary>
        /// Create a new instance of TaskTime
        /// </summary>
        /// <param name="Time">DateTime object representing the time</param>
        /// <param name="Format">Time format</param>
        public TaskTime(DateTime Time, TimeFormat Format)
        {
            this.Time = Time;
            this.Format = Format;
            this.HasError = false;
        }

        /// <summary>
        /// Gets or sets the time format
        /// </summary>
        public TimeFormat Format
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the DateTime object
        /// </summary>
        public DateTime Time
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether conversion process encountered errors
        /// </summary>
        public bool HasError
        {
            get;
            set;
        }
    }
}
