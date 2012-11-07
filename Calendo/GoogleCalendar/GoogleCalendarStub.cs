//@author A0080933E
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calendo.GoogleCalendar
{
    public class GoogleCalendarStub : GoogleCalendar
    {
        private static string lastRunMessage = "";
        public static string LastRun
        {
            get { return lastRunMessage; }
        }

        public new void Export()
        {
            lastRunMessage = "Export";
        }

        public new void Import()
        {
            lastRunMessage = "Import";
        }
    }
}
