//@author A0080860H

using System.Collections.Generic;
using Calendo.AutoSuggest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UiUnitTests
{
    [TestClass]
    public class AutoSuggestTests
    {
        private AutoSuggest AutoSuggest;

        [TestInitialize]
        public void TestInitialize()
        {
            Dictionary<string, string[]> aliasDictionary = new Dictionary<string, string[]>();
            aliasDictionary.Add("add", new string[] {"/add", "/new"});
            aliasDictionary.Add("remove", new string[] {"/remove", "/rm"});
            aliasDictionary.Add("change", new string[] {"/change", "/update"});
            aliasDictionary.Add("undo", new string[] {"/undo"});
            aliasDictionary.Add("redo", new string[] { "/redo" });
            aliasDictionary.Add("sync", new string[] { "/sync" });
            aliasDictionary.Add("export", new string[] { "/export", "push" });
            aliasDictionary.Add("import", new string[] { "/import", "pull" });

            AutoSuggest = new AutoSuggest(aliasDictionary);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            AutoSuggest.SetSuggestions("");
        }

        [TestMethod]
        public void UiAutoSuggestMatchAlias()
        {
            AutoSuggestEntry testEntry = new AutoSuggestEntry("test", "test description", EntryType.Master, new string[] { "alpha", "beta", "charlie" });
            bool actual = AutoSuggest.CheckAliasesForCommand("al", testEntry);
            bool expected = true;
            Assert.AreEqual(expected, actual);

            actual = AutoSuggest.CheckAliasesForCommand("no", testEntry);
            expected = false;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UiAutoSuggestEntryHasNoAliases()
        {
            AutoSuggestEntry testEntry = new AutoSuggestEntry("test", "test description", EntryType.Master, null);
            bool expected = false;
            bool actual = testEntry.HasAliases;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UiAutoSuggestEntryHasAliases()
        {
            AutoSuggestEntry testEntry = new AutoSuggestEntry("another test", "another description", EntryType.Master,
                                                              new string[] { "first", "second", "third" });
            bool expected = true;
            bool actual = testEntry.HasAliases;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UiAutoSuggestSetSuggestionFullCommand()
        {
            AutoSuggest.SetSuggestions("/add");
            Assert.IsTrue(AutoSuggest.SuggestionList.Count == 1);

            AutoSuggestEntry suggestion = AutoSuggest.SuggestionList[0];
            Assert.IsTrue(suggestion.Command == "/add");
        }

        [TestMethod]
        public void UiAutoSuggestSetSuggestionSearchString()
        {
            AutoSuggest.SetSuggestions("searching");
            Assert.IsTrue(AutoSuggest.SuggestionList.Count == 0);
        }

        [TestMethod]
        public void UiAutoSuggestSetSuggestionCommandAndWords()
        {
            AutoSuggest.SetSuggestions("/remove 1");
            Assert.IsTrue(AutoSuggest.SuggestionList.Count == 1);

            AutoSuggestEntry suggestion = AutoSuggest.SuggestionList[0];
            Assert.IsTrue(suggestion.Type == EntryType.Detail);
            Assert.IsTrue(suggestion.Command == "/remove");
        }
    }
}
