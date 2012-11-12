//@author A0080933E
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calendo.Diagnostics;

namespace CalendoUnitTests
{
    [TestClass]
    public class DebugTest
    {
        [TestMethod]
        public void DebugSubscriber()
        {
            string message = "";
            DebugTool.NotifyHandler subscriberDelegate = new DebugTool.NotifyHandler(delegate(string msg) { message = msg; });
            DebugTool.AddSubscriber(subscriberDelegate);
            string testMessage = "Test Message";
            DebugTool.Alert(testMessage);
            Assert.IsTrue(DebugTool.HasNotification == true);
            Assert.IsTrue(message == testMessage);
            Assert.IsTrue(DebugTool.NotificationMessage == testMessage);
        }
    }
}
