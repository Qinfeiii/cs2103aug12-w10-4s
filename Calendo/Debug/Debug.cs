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
        private const string LOG_FILEPATH = "log.txt";
        private const string CONFIG_FILEPATH = "debugcfg.txt";
        private static bool _Enable = true;
        private static bool loaded = false;

        /// <summary>
        /// Get or set debug enable switch
        /// </summary>
        public static bool Enable {
            get { return _Enable; }
            set { _Enable = value; }
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
                loaded = true;
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
            if (!loaded)
            {
                LoadConfig();
            }
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
            if (!loaded)
            {
                LoadConfig();
            }
            if (Debug.Enable)
            {
                StreamWriter file = new StreamWriter(LOG_FILEPATH);
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
            if (!loaded)
            {
                LoadConfig();
            }
            if (Debug.Enable && !condition)
            {
                System.Diagnostics.Debug.Assert(condition);
            }
        }
    }
}
