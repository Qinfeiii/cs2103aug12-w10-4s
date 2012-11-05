//@author A0080860H
using Calendo.AutoSuggest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UiUnitTests
{
    [TestClass]
    public class AutoSuggestTest
    {
        private AutoSuggest AutoSuggest;

        [TestInitialize]
        public void TestInitialize()
        {
            Calendo.Logic.CommandProcessor temporaryCommandProcessor = new Calendo.Logic.CommandProcessor();
            AutoSuggest = new AutoSuggest(temporaryCommandProcessor.GetInputCommandList());
        }

        [TestMethod]
        public void UI_AutoSuggestMatchAlias()
        {
            AutoSuggestEntry testEntry = new AutoSuggestEntry("test", "test description", EntryType.Master, new string[] { "alpha", "beta", "charlie" });
            bool actual = AutoSuggest.CheckAliasesForCommand("al", testEntry);
            bool expected = true;
            Assert.AreEqual(expected, actual);

            actual = AutoSuggest.CheckAliasesForCommand("no", testEntry);
            expected = false;
            Assert.AreEqual(expected, actual);
        }
    }
}
