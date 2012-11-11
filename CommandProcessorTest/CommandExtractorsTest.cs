//@author A0091571E
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calendo.Logic;

namespace CommandProcessorTest
{
    [TestClass]
    public class CommandExtractorsTest
    {

        static private string COMMAND_ADD_FLOATING_TASK = "/add Work on Calendo";
        static private string RESULT_ADD_FLOATING_TASK_TEXT = "Work on Calendo";
        static private string COMMAND_ADD_DEADLINE_TASK = "/add Finish Calendo /date 10/10/2012 /time 10:30";
        static private string RESULT_ADD_DEADLINE_TASK_TEXT = "Finish Calendo";
        static private string RESULT_ADD_DEADLINE_TASK_DATE = "10/10/2012";
        static private string RESULT_ADD_DEADLINE_TASK_TIME = "10:30";
        static private string COMMAND_ADD_DURATION_TASK = "/add Add fancy stuff to Calendo /startdate 11/10/2012 /starttime 9:00 /enddate 9/11/2012 /endtime 21:00";
        static private string RESULT_ADD_DURATION_TASK_TEXT = "Add fancy stuff to Calendo";
        static private string RESULT_ADD_DURATION_TASK_START_DATE = "11/10/2012";
        static private string RESULT_ADD_DURATION_TASK_START_TIME = "9:00";
        static private string RESULT_ADD_DURATION_TASK_END_DATE = "9/11/2012";
        static private string RESULT_ADD_DURATION_TASK_END_TIME = "21:00";

        static private string COMMAND_REMOVE_FIRST_TASK = "/remove 1";
        static private string RESULT_REMOVE_FIRST_TASK_TEXT = "1";

        static private string COMMAND_CHANGE_FIRST_TEXT = "/change 1 Meeting at Starbucks";
        static private string RESULT_CHANGE_FIRST_TEXT_TEXT = "1 Meeting at Starbucks";

        static private string COMMAND_UNDO = "/undo";

        static private string COMMAND_REDO = "/redo";

        static private string COMMAND_SYNC = "/export";

        static private string COMMAND_IMPORT = "/import";

        private string testType;
        private string testStartDate;
        private string testStartTime;
        private string testEndDate;
        private string testEndTime;
        private string testText;

        [TestMethod]
        public void Add()
        {
            CommandExtractors extractor = new CommandExtractors();

            // Test floating task
            extractor.Extract(COMMAND_ADD_FLOATING_TASK,
                ref testType,
                ref testStartDate,
                ref testStartTime,
                ref testEndDate,
                ref testEndTime,
                ref testText);
            Assert.AreEqual(CommandExtractors.COMMAND_TYPE_ADD, testType);
            Assert.AreEqual(RESULT_ADD_FLOATING_TASK_TEXT, testText);

            //Test deadline task
            extractor.Extract(COMMAND_ADD_DEADLINE_TASK,
                ref testType,
                ref testStartDate,
                ref testStartTime,
                ref testEndDate,
                ref testEndTime,
                ref testText);
            Assert.AreEqual(CommandExtractors.COMMAND_TYPE_ADD, testType);
            Assert.AreEqual(RESULT_ADD_DEADLINE_TASK_TEXT, testText);
            Assert.AreEqual(RESULT_ADD_DEADLINE_TASK_DATE, testStartDate);
            Assert.AreEqual(RESULT_ADD_DEADLINE_TASK_TIME, testStartTime);

            //Test Duration task
            extractor.Extract(COMMAND_ADD_DURATION_TASK,
                ref testType,
                ref testStartDate,
                ref testStartTime,
                ref testEndDate,
                ref testEndTime,
                ref testText);
            Assert.AreEqual(CommandExtractors.COMMAND_TYPE_ADD, testType);
            Assert.AreEqual(RESULT_ADD_DURATION_TASK_TEXT, testText);
            Assert.AreEqual(RESULT_ADD_DURATION_TASK_START_DATE, testStartDate);
            Assert.AreEqual(RESULT_ADD_DURATION_TASK_START_TIME, testStartTime);
            Assert.AreEqual(RESULT_ADD_DURATION_TASK_END_DATE, testEndDate);
            Assert.AreEqual(RESULT_ADD_DURATION_TASK_END_TIME, testEndTime);
        }

        [TestMethod]
        public void Remove()
        {
            CommandExtractors extractor = new CommandExtractors();

            extractor.Extract(COMMAND_REMOVE_FIRST_TASK,
                ref testType,
                ref testStartDate,
                ref testStartTime,
                ref testEndDate,
                ref testEndTime,
                ref testText);
            Assert.AreEqual(CommandExtractors.COMMAND_TYPE_REMOVE, testType);
            Assert.AreEqual(RESULT_REMOVE_FIRST_TASK_TEXT, testText);
        }

        [TestMethod]
        public void Change()
        {
            CommandExtractors extractor = new CommandExtractors();

            extractor.Extract(COMMAND_CHANGE_FIRST_TEXT,
                ref testType,
                ref testStartDate,
                ref testStartTime,
                ref testEndDate,
                ref testEndTime,
                ref testText);
            Assert.AreEqual(CommandExtractors.COMMAND_TYPE_CHANGE, testType);
            Assert.AreEqual(RESULT_CHANGE_FIRST_TEXT_TEXT, testText);
        }

        [TestMethod]
        public void Undo()
        {
            CommandExtractors extractor = new CommandExtractors();

            extractor.Extract(COMMAND_UNDO,
                ref testType,
                ref testStartDate,
                ref testStartTime,
                ref testEndDate,
                ref testEndTime,
                ref testText);
            Assert.AreEqual(CommandExtractors.COMMAND_TYPE_UNDO, testType);
        }

        [TestMethod]
        public void Redo()
        {
            CommandExtractors extractor = new CommandExtractors();

            extractor.Extract(COMMAND_REDO,
                ref testType,
                ref testStartDate,
                ref testStartTime,
                ref testEndDate,
                ref testEndTime,
                ref testText);
            Assert.AreEqual(CommandExtractors.COMMAND_TYPE_REDO, testType);
        }

        [TestMethod]
        public void Sync()
        {
            CommandExtractors extractor = new CommandExtractors();

            extractor.Extract(COMMAND_SYNC,
                ref testType,
                ref testStartDate,
                ref testStartTime,
                ref testEndDate,
                ref testEndTime,
                ref testText);
            Assert.AreEqual(CommandExtractors.COMMAND_TYPE_SYNC, testType);
        }

        [TestMethod]
        public void Import()
        {
            CommandExtractors extractor = new CommandExtractors();

            extractor.Extract(COMMAND_IMPORT,
                ref testType,
                ref testStartDate,
                ref testStartTime,
                ref testEndDate,
                ref testEndTime,
                ref testText);
            Assert.AreEqual(CommandExtractors.COMMAND_TYPE_IMPORT, testType);
        }
    }
}
