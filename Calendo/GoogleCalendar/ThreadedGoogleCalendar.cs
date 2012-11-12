//@author A0080933E
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Calendo.GoogleCalendar
{
    /// <summary>
    /// Performs multithreaded operations
    /// </summary>
    public class ThreadedGoogleCalendar
    {
        private static Type GoogleCalendarClassType = typeof(GoogleCalendar);
        private static Delegate AuthMethod = new AuthorizationCall(delegate() { GoogleCalendar.Authorize(); });
        private static Thread ActiveThread = null;

        public delegate void AuthorizationCall();

        /// <summary>
        /// Sets the Google Calendar class used
        /// </summary>
        public static Type GoogleCalendarType
        {
            set { GoogleCalendarClassType = value; }
        }

        /// <summary>
        /// Sets the authorization method used
        /// </summary>
        public static Delegate AuthorizationMethod
        {
            set { AuthMethod = value; }
        }

        /// <summary>
        /// Gets the current thread
        /// </summary>
        public static Thread CurrentThread
        {
            get { return ActiveThread; }
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
        /// <param name="method">Method to run on separate thread</param>
        private static void RunThread(ThreadStart method)
        {
            Thread threadInstance = new Thread(method);
            ActiveThread = threadInstance;
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
