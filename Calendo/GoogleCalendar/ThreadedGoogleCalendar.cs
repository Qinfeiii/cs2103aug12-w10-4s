//@author A0080933E
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Calendo.GoogleCalendar
{
    public class ThreadedGoogleCalendar
    {
        private static Type GoogleCalendarClassType = typeof(GoogleCalendar);
        /// <summary>
        /// Sets the Google Calendar class used
        /// </summary>
        public static Type GoogleCalendarType
        {
            set { GoogleCalendarClassType = value; }
        }

        public delegate void AuthorizationCall();
        private static Delegate AuthMethod = new AuthorizationCall(delegate() { GoogleCalendar.Authorize(); });

        public static Delegate AuthorizationMethod
        {
            set { AuthMethod = value; }
        }

        /// <summary>
        /// Export to Google Calendar
        /// </summary>
        public static void Export()
        {
            AuthMethod.DynamicInvoke();
            RunThread(new ThreadStart(ThreadedExport));
        }

        /// <summary>
        /// Import from Google Calendar
        /// </summary>
        public static void Import()
        {
            AuthMethod.DynamicInvoke();
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
            GoogleCalendar googleCalendar = (GoogleCalendar)Activator.CreateInstance(GoogleCalendarClassType);
            googleCalendar.Export();
            
        }

        /// <summary>
        /// Wrapper method for multithreading import
        /// </summary>
        private static void ThreadedImport()
        {
            GoogleCalendar googleCalendar = (GoogleCalendar)Activator.CreateInstance(GoogleCalendarClassType);
            googleCalendar.Import();
        }
    }
}
