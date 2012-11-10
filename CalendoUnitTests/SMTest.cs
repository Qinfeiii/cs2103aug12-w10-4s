//@author A0080933E
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calendo.Logic;

namespace CalendoUnitTests
{
    [TestClass]
    public class SMTest
    {
        [TestMethod]
        public void SMLoad()
        {
            SettingsManager settingsManager = new SettingsManager();
            settingsManager.Clear();
            Assert.IsNull(settingsManager.GetSetting("non-existant"));
        }

        [TestMethod]
        public void SMAdd()
        {
            SettingsManager settingsManager = new SettingsManager();
            settingsManager.Clear();
            settingsManager.SetSetting("test 1", "test1 value");
            Assert.IsTrue(settingsManager.GetSetting("test 1") == "test1 value");

            // Test if setting can be accessed
            SettingsManager sm2 = new SettingsManager();
            Assert.IsTrue(sm2.GetSetting("test 1") == "test1 value");
        }

        [TestMethod]
        public void SMModify()
        {
            SettingsManager settingsManager = new SettingsManager();
            settingsManager.Clear();
            settingsManager.SetSetting("test 2", "test2 value");
            settingsManager.SetSetting("test 2", "test2 new value");
            Assert.IsTrue(settingsManager.GetSetting("test 2") == "test2 new value");
        }
    }
}
