//@author Jerome
using System;
using Calendo.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UiUnitTests
{
    [TestClass]
    public class ConverterTests
    {
        StringArrayToStringConverter ArrayToStringConverter;
        EntryToDateTimeStringConverter EntryToDateTimeStringConverter;

        [TestInitialize]
        public void TestInitialize()
        {
            ArrayToStringConverter = new StringArrayToStringConverter();
            EntryToDateTimeStringConverter = new EntryToDateTimeStringConverter();
        }

        [TestMethod]
        public void StringArrayToStringTest()
        {
            string[] testData = new string[] { "first", "second", "third", "fourth" };
            string expected = "second, third, fourth";
            string actual = ArrayToStringConverter.Convert(testData, testData.GetType(), null, null) as string;
            Assert.AreEqual(expected, actual);

            expected = "none";
            actual = ArrayToStringConverter.Convert(null, null, null, null) as string;
            Assert.AreEqual(expected, actual);
        }
    }
}
