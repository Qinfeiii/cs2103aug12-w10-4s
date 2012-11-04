//@author A0080933E
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Calendo.GoogleCalendar
{
    public class ThreadedGoogleCalendar
    {
        /// <summary>
        /// Export to Google Calendar
        /// </summary>
        public static void Export()
        {
            GoogleCalendar.Authorize();
            RunThread(new ThreadStart(ThreadedExport));
        }

        /// <summary>
        /// Import from Google Calendar
        /// </summary>
        public static void Import()
        {
            GoogleCalendar.Authorize();
            RunThread(new ThreadStart(ThreadedImport));
        }

        /// <summary>
        /// Performs the operation in a separate thread
        /// </summary>
        /// <param name="method"></param>
        private static void RunThread(ThreadStart method)
        {
            Thread threadInstance = new Thread(method);
            threadInstance.Start();
        }

        /// <summary>
        /// Wrapper method for multithreading export
        /// </summary>
        private static void ThreadedExport()
        {
            GoogleCalendar googleCalendar = new GoogleCalendar();
            googleCalendar.Export();
        }

        /// <summary>
        /// Wrapper method for multithreading import
        /// </summary>
        private static void ThreadedImport()
        {
            GoogleCalendar googleCalendar = new GoogleCalendar();
            googleCalendar.Import();
        }
    }
}
