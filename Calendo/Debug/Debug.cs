using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.IO;
using System.Diagnostics;

namespace Calendo.DebugTool
{
    public class Debug
    {
        public const string logFile = "log.txt";
        public static bool Enable = true;

        /// <summary>
        /// Displays a message box to user
        /// </summary>
        /// <param name="message">Message to display</param>
        public static void Alert(string message)
        {
            if (Debug.Enable)
            {
                MessageBox.Show(message);
            }
        }

        /// <summary>
        /// Writes a log message
        /// </summary>
        /// <param name="message">Message string</param>
        public static void Log(string message)
        {
            if (Debug.Enable)
            {
                StreamWriter file = new StreamWriter(logFile);
                file.WriteLine(message);
                file.Close();
            }
        }

        /// <summary>
        /// Assert a condition
        /// </summary>
        /// <param name="condition"></param>
        public static void Assert(bool condition)
        {
            if (Debug.Enable && !condition)
            {
                System.Diagnostics.Debug.Assert(condition);
            }
        }
    }
}
