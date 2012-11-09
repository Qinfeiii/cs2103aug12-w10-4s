//@author A0080933E
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.IO;
using System.Diagnostics;

namespace Calendo.Diagnostics
{
    public enum MessageType
    {
        Critical,
        Alert,
        Info
    }

    public class DebugTool
    {
        private const string LOG_FILEPATH = "log.txt";
        private const string LOG_FORMAT = "{0:G}: [{1}] {2}";
        private const string CONFIG_FILEPATH = "debugcfg.txt";
        private const string MESSAGE_DEBUG_LOAD = "Debug file loaded, debugging state = {0}";
        private static bool IsEnable = true;
        private static bool IsConfigLoaded = false;
        private static bool IsHasNotification = false;

        /// <summary>
        /// Gets whether if there are notifications. Value will be changed to false after being accessed.
        /// </summary>
        public static bool HasNotification
        {
            get
            {
                bool pastValue = IsHasNotification;
                IsHasNotification = false;
                return pastValue;
            }
        }

        private static string CurrentMessage = "";
        /// <summary>
        /// Gets the latest notification message
        /// </summary>
        public static string NotificationMessage
        {
            get
            {
                return CurrentMessage;
            }
        }
        

        /// <summary>
        /// Notify handler for DebugTool notifications
        /// </summary>
        /// <param name="message"></param>
        public delegate void NotifyHandler(string message);
        private static List<NotifyHandler> SubscriberList = new List<NotifyHandler>();

        /// <summary>
        /// Adds a subscriber
        /// </summary>
        /// <param name="method">Method to be invoked on notification</param>
        public static void AddSubscriber(NotifyHandler method)
        {
            SubscriberList.Add(method);
        }

        /// <summary>
        /// Call each subscriber alert methods
        /// </summary>
        private static void UpdateSubscribers(string message)
        {
            CurrentMessage = message;
            IsHasNotification = true;
            foreach (NotifyHandler notifyMethod in SubscriberList)
            {
                notifyMethod(message);
            }
        }


        /// <summary>
        /// Get or set debug enable switch
        /// </summary>
        public static bool Enable {
            get { return IsEnable; }
            set { IsEnable = value; }
        }

        /// <summary>
        /// Load debug configuration file
        /// </summary>
        public static void LoadConfig()
        {
            try
            {
                Stream fileStream = new FileStream(CONFIG_FILEPATH, FileMode.OpenOrCreate);
                StreamReader sr = new StreamReader(fileStream);
                string config = sr.ReadLine();
                if (config == "1")
                {
                    Enable = true;
                }
                else if (config == "0")
                {
                    Enable = false;
                }
                sr.Close();
                fileStream.Close();
                IsConfigLoaded = true;

                WriteLog(String.Format(MESSAGE_DEBUG_LOAD, Enable.ToString()));
            }
            catch (Exception e)
            {
                // Invalid file
                MessageBox.Show("Error: " + e.ToString());
            }
             
        }

        /// <summary>
        /// Displays a message box to user
        /// </summary>
        /// <param name="message">Message to display</param>
        public static void Alert(string message)
        {
            if (!IsConfigLoaded)
            {
                LoadConfig();
            }
            if (DebugTool.Enable)
            {
                MessageBox.Show(message);
                UpdateSubscribers(message);
            }
            WriteLog(message, MessageType.Alert);
        }

        /// <summary>
        /// Writes a log message
        /// </summary>
        /// <param name="message">Message string</param>
        public static void WriteLog(string message, MessageType type = MessageType.Info)
        {
            if (!IsConfigLoaded)
            {
                LoadConfig();
            }
            string timeStamp = DateTime.Now.ToString();
            string prefix = type.ToString();
            StreamWriter file = System.IO.File.AppendText(LOG_FILEPATH);
            file.WriteLine(String.Format(LOG_FORMAT, timeStamp, prefix, message));
            file.Close();
        }

        /// <summary>
        /// Erase the log contents
        /// </summary>
        public static void ClearLog()
        {
            System.IO.File.WriteAllText(LOG_FILEPATH, "");
        }
    }
}
