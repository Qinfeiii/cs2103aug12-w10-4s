//@author A0080933E
using System;
using System.Collections.Generic;
using System.Text;

namespace Calendo.Logic
{
    public class TaskTime
    {
        public TaskTime()
        {
            this.Format = TimeFormat.None;
            this.Time = DateTime.Today;
            this.HasError = true;
        }
        public TaskTime(DateTime Time, TimeFormat Format)
        {
            this.Time = Time;
            this.Format = Format;
            this.HasError = false;
        }
        public TimeFormat Format
        {
            get;
            set;
        }
        public DateTime Time
        {
            get;
            set;
        }
        public bool HasError
        {
            get;
            set;
        }
    }
}
