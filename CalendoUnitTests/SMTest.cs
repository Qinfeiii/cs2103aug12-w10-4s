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
            SettingsManager sm = new SettingsManager();
            sm.Clear();
            Assert.IsNull(sm.GetSetting("non-existant"));
        }
        [TestMethod]
        public void SMAdd()
        {
            SettingsManager sm = new SettingsManager();
            sm.Clear();
            sm.SetSetting("test 1", "test1 value");
            Assert.IsTrue(sm.GetSetting("test 1") == "test1 value");

            // Test if setting can be accessed
            SettingsManager sm2 = new SettingsManager();
            Assert.IsTrue(sm2.GetSetting("test 1") == "test1 value");
        }
        [TestMethod]
        public void SMModify()
        {
            SettingsManager sm = new SettingsManager();
            sm.Clear();
            sm.SetSetting("test 2", "test2 value");
            sm.SetSetting("test 2", "test2 new value");
            Assert.IsTrue(sm.GetSetting("test 2") == "test2 new value");
        }
    }
}
