using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calendo.GoogleCalendar
{
    public class TaskResponse
    {
        public string kind
        {
            get;
            set;
        }
        public string etag
        {
            get;
            set;
        }
        public List<TaskEntry> items
        {
            get;
            set;
        }

    }
    public class TaskEntry
    {
        public string kind
        {
            get;
            set;
        }
        public string id
        {
            get;
            set;
        }
        public string title
        {
            get;
            set;
        }
        public string updated
        {
            get;
            set;
        }
        public string selfLink
        {
            get;
            set;
        }
    }
}
