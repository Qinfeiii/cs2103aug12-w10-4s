//@author A0080933E
using System;
using System.Collections.Generic;
using System.Text;

namespace Calendo.GoogleCalendar
{
    public class GoogleCalendarStub : GoogleCalendar
    {
        private static string LastRunMessage = "";
        public static string LastRun
        {
            get { return LastRunMessage; }
        }

        public override void Export()
        {
            LastRunMessage = "Export";
        }

        public override string Import()
        {
            LastRunMessage = "Import";
            return "";
        }
    }
}
