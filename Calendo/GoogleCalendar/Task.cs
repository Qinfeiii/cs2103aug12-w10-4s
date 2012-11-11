//@author A0080933E
using System;
using System.Collections.Generic;
using System.Text;

namespace Calendo.GoogleCalendar
{
    /// <summary>
    /// Google API response
    /// </summary>
    public class TaskResponse
    {
        /// <summary>
        /// Kind of response
        /// </summary>
        public string Kind
        {
            get;
            set;
        }

        /// <summary>
        /// Entity tag
        /// </summary>
        public string Etag
        {
            get;
            set;
        }

        /// <summary>
        /// List of resources
        /// </summary>
        public List<TaskEntry> Items
        {
            get;
            set;
        }

    }

    /// <summary>
    /// Google API resource representation
    /// </summary>
    public class TaskEntry
    {
        /// <summary>
        /// Kind of resource
        /// </summary>
        public string Kind
        {
            get;
            set;
        }

        /// <summary>
        /// Identifier
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Title
        /// </summary>
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// Updated date
        /// </summary>
        public string Updated
        {
            get;
            set;
        }

        /// <summary>
        /// Self link
        /// </summary>
        public string SelfLink
        {
            get;
            set;
        }

        //@author A0091539X
        /// <summary>
        /// Due date
        /// </summary>
        public string due
        {
            get;
            set;
        }
    }
}
