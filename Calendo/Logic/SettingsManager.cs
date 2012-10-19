using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Calendo.Data;

namespace Calendo.Logic
{
    [Serializable]
    public class KeyPair<K, V>
    {
        public KeyPair()
        {
        }
        public KeyPair(K Key, V Value)
        {
            this.Key = Key;
            this.Value = Value;
        }
        public K Key
        {
            get;
            set;
        }

        public V Value
        {
            get;
            set;
        }
    }
    public class SettingsManager
    {
        private Storage<List<KeyPair<string, string>>> settingsStorage;
        private Dictionary<string, int> settingsDictionary;

        /// <summary>
        /// Creates a new instance of SettingsManager
        /// </summary>
        public SettingsManager() {
            settingsStorage = new Storage<List<KeyPair<string, string>>>("settings.txt");
            settingsDictionary = new Dictionary<string, int>();
            settingsStorage.Load();
            Debug.Assert(settingsStorage.Entries != null, "Settings entries cannot be null");
            settingsStorage.Entries = new List<KeyPair<string, string>>();
            LoadList();
        }

        /// <summary>
        /// Load the list into dictionary
        /// </summary>
        private void LoadList()
        {
            List<KeyPair<string, string>> settingsList = settingsStorage.Entries;
            for (int i = 0; i < settingsList.Count; i++)
            {
                if (settingsList[i].Key != null)
                {
                    settingsDictionary.Add(settingsList[i].Key, i);
                }
            }
        }

        /// <summary>
        /// Gets the setting specified
        /// </summary>
        /// <param name="settingName">Name of the setting</param>
        /// <returns>Returns the value of the setting, null otherwise</returns>
        public string GetSetting(string settingName)
        {
            if (settingsDictionary.ContainsKey(settingName))
            {
                return settingsStorage.Entries[settingsDictionary[settingName]].Value;
            }
            else
            {
                // No setting by this name
                return null;
            }
        }

        /// <summary>
        /// Add or modify the setting specified
        /// </summary>
        /// <param name="settingName">Name of the setting</param>
        /// <param name="settingValue">Value of the setting</param>
        public void SetSetting(string settingName, string settingValue)
        {
            if (settingsDictionary.ContainsKey(settingName))
            {
                // Override existing setting
                settingsStorage.Entries[settingsDictionary[settingName]] = new KeyPair<string, string>(settingName, settingValue);
            }
            else
            {
                // No existing setting, add new one
                settingsDictionary.Add(settingName, settingsStorage.Entries.Count);
                settingsStorage.Entries.Add(new KeyPair<string, string>(settingName, settingValue));
            }
            settingsStorage.Save();
        }

        /// <summary>
        /// Erase all settings
        /// </summary>
        public void Clear()
        {
            settingsStorage.Entries = new List<KeyPair<string, string>>();
            settingsStorage.Save();
        }
    }
}
