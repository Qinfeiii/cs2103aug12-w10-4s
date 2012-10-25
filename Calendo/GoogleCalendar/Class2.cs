using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calendo.GoogleCalendar
{
    class TaskResponse
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
        public List<TaskIndividual> items
        {
            get;
            set;
        }

    }
    public class TaskIndividual
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
        public string etag
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
        public string position
        {
            get;
            set;
        }
        public string status
        {
            get;
            set;
        }
        public string due
        {
            get;
            set;
        }
    }
}
