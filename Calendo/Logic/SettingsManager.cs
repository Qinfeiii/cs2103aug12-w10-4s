using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Calendo.Data;

namespace Calendo.Logic
{
    [Serializable]
    public class KeyPair<K, V>
    {
        /// <summary>
        /// Creates a new serializable KeyPair class
        /// </summary>
        public KeyPair()
        {
        }

        /// <summary>
        /// Creates a new serializable KeyPair class
        /// </summary>
        /// <param name="Key">Key represented by KeyPair</param>
        /// <param name="Value">Value represented by KeyPair</param>
        public KeyPair(K Key, V Value)
        {
            this.Key = Key;
            this.Value = Value;
        }

        /// <summary>
        /// Key represented by KeyPair
        /// </summary>
        public K Key
        {
            get;
            set;
        }
        
        /// <summary>
        /// Value represented by KeyPair
        /// </summary>
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
            this.settingsStorage = new Storage<List<KeyPair<string, string>>>("settings.txt");
            this.settingsDictionary = new Dictionary<string, int>();
            this.settingsStorage.Load();
            Debug.Assert(settingsStorage.Entries != null, "Settings entries cannot be null");
            this.LoadList();
        }

        /// <summary>
        /// Load the list into dictionary
        /// </summary>
        private void LoadList()
        {
            List<KeyPair<string, string>> settingsList = this.settingsStorage.Entries;
            for (int i = 0; i < settingsList.Count; i++)
            {
                if (settingsList[i].Key != null)
                {
                    this.settingsDictionary.Add(settingsList[i].Key, i);
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
            if (this.settingsDictionary.ContainsKey(settingName))
            {
                int settingID = this.settingsDictionary[settingName];
                return this.settingsStorage.Entries[settingID].Value;
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
            KeyPair<string, string> newSetting = new KeyPair<string, string>(settingName, settingValue);
            if (this.settingsDictionary.ContainsKey(settingName))
            {
                // Override existing setting
                int settingID = this.settingsDictionary[settingName];
                this.settingsStorage.Entries[settingID] = newSetting;
            }
            else
            {
                // No existing setting, add new one
                this.settingsDictionary.Add(settingName, settingsStorage.Entries.Count);
                this.settingsStorage.Entries.Add(newSetting);
            }
            this.settingsStorage.Save();
        }

        /// <summary>
        /// Erase all settings
        /// </summary>
        public void Clear()
        {
            this.settingsStorage.Entries = new List<KeyPair<string, string>>();
            this.settingsStorage.Save();
            this.settingsStorage.Load();
            this.LoadList();
        }
    }
}
