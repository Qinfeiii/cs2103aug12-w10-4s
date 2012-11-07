//@author A0080933E
using System;
using System.Collections.Generic;
using System.Text;

namespace Calendo.GoogleCalendar
{
    public class TaskResponse
    {
        public string Kind
        {
            get;
            set;
        }
        public string Etag
        {
            get;
            set;
        }
        public List<TaskEntry> Items
        {
            get;
            set;
        }

    }
    public class TaskEntry
    {
        public string Kind
        {
            get;
            set;
        }
        public string Id
        {
            get;
            set;
        }
        public string Title
        {
            get;
            set;
        }
        public string Updated
        {
            get;
            set;
        }
        public string SelfLink
        {
            get;
            set;
        }

        //@author A0091539X
        public string due
        {
            get;
            set;
        }
    }
}
