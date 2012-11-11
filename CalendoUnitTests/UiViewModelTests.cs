//@author A0080860H
using System;
using Calendo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CalendoUnitTests
{
    [TestClass]
    public class UiViewModelTests
    {
        private UiViewModel ViewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            ViewModel = new UiViewModel();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            ViewModel.SetSuggestions("");
        }

        [TestMethod]
        public void UiAutoSuggestRowTest()
        {
            // We need Auto Suggest to be returning a single
            // detail entry.
            ViewModel.SetSuggestions("/add Test");
            Assert.IsTrue(ViewModel.SuggestionList.Count == 1);
            Assert.IsTrue(!ViewModel.SuggestionList[0].IsMaster);
            
            int expected = 3;
            int actual = ViewModel.AutoSuggestRow;
            Assert.AreEqual(expected, actual);

            // Auto Suggest should now have more than one entry in it.
            ViewModel.SetSuggestions("/");
            Assert.IsTrue(ViewModel.SuggestionList.Count > 1);

            expected = 4;
            actual = ViewModel.AutoSuggestRow;
            Assert.AreEqual(expected, actual);
        }
    }
}
