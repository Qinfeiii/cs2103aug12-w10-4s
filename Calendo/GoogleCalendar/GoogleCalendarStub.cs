//@author A0080933E
using System;
using System.Collections.Generic;
using System.Text;

namespace Calendo.GoogleCalendar
{
    public class GoogleCalendarStub : GoogleCalendar
    {
        private static string LastRunMessage = "";

        /// <summary>
        /// Gets the last run message
        /// </summary>
        public static string LastRun
        {
            get { return LastRunMessage; }
        }

        /// <summary>
        /// Export stub, sets last run message to "Export"
        /// </summary>
        /// <returns>Returns true</returns>
        public override bool Export()
        {
            LastRunMessage = "Export";
            return true;
        }

        /// <summary>
        /// Import stub, sets last run message to "Import"
        /// </summary>
        /// <returns>Returns true</returns>
        public override bool Import()
        {
            LastRunMessage = "Import";
            return true;
        }
    }
}
