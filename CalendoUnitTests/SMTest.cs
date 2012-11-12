//@author A0080933E
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calendo.Logic;

namespace CalendoUnitTests
{
    [TestClass]
    public class SMTest
    {
        /// <summary>
        /// Tests if settings can be added
        /// </summary>
        [TestMethod]
        public void SMAdd()
        {
            string testSettingName = "test 1";
            string testSettingValue = "test1 value";
            
            // Save a setting
            SettingsManager settingsManager = new SettingsManager();
            settingsManager.Clear();
            settingsManager.SetSetting(testSettingName, testSettingValue);
            Assert.IsTrue(settingsManager.GetSetting(testSettingName) == testSettingValue);

            // Test if setting can be accessed from another instance
            SettingsManager secondaryInstance = new SettingsManager();
            Assert.IsTrue(secondaryInstance.GetSetting(testSettingName) == testSettingValue);
        }

        /// <summary>
        /// Tests if settings can be loaded from file
        /// </summary>
        [TestMethod]
        public void SMLoad()
        {
            SettingsManager settingsManager = new SettingsManager();
            settingsManager.Clear();
            Assert.IsNull(settingsManager.GetSetting(""));
            Assert.IsNull(settingsManager.GetSetting("non-existant"));
        }

        /// <summary>
        /// Tests if settings can be modified and persist after saving
        /// </summary>
        [TestMethod]
        public void SMModify()
        {
            string testSettingName = "Test modify";
            string testSettingValue = "test modify value";
            string testSettingNewValue = "test new value";

            SettingsManager settingsManager = new SettingsManager();
            settingsManager.Clear();
            settingsManager.SetSetting(testSettingName, testSettingValue);
            settingsManager.SetSetting(testSettingName, testSettingNewValue);
            Assert.IsTrue(settingsManager.GetSetting(testSettingName) == testSettingNewValue);
        }
    }
}
