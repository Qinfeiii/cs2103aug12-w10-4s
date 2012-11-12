//@author A0091571E
using Calendo.Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace CommandProcessorTest
{
    [TestClass]
    public class CommandProcessorTest
    {
        private readonly int NEXT_YEAR = DateTime.Now.Year + 1;
        private readonly int NUMBER_OF_ADD_TESTS = 3;
        private readonly string COMMAND_ADD_FLOATING_TASK = "/add Work on Calendo";
        private readonly string RESULT_ADD_FLOATING_TASK_DESCRIPTION = "Work on Calendo";
        private readonly string COMMAND_ADD_DEADLINE_TASK = "/add Finish Calendo /date 10/10/2012 /time 10:30";
        private readonly string RESULT_ADD_DEADLINE_TASK_DESCRIPTION = "Finish Calendo";
        private readonly DateTime RESULT_ADD_DEADLINE_TASK_DATETIME = new DateTime(2012, 10, 10, 10, 30, 0);
        private readonly string COMMAND_ADD_DURATION_TASK = "/add Add fancy stuff to Calendo /startdate 11/10/2012 /starttime 9:00 /enddate 9/11/2012 /endtime 21:00";
        private readonly string RESULT_ADD_DURATION_TASK_DESCRIPTION = "Add fancy stuff to Calendo";
        private readonly DateTime RESULT_ADD_DURATION_TASK_START_DATETIME = new DateTime(2012, 10, 11, 9, 0, 0);
        private readonly DateTime RESULT_ADD_DURATION_TASK_END_DATETIME = new DateTime(2012, 11, 9, 21, 0, 0);
        private readonly string COMMAND_DELETE_FIRST_TASK = "/delete 1";
        private readonly int EXPECTED_NUMBER_OF_TASKS_CHANGE = 1;
        private readonly string COMMAND_CHANGE_FIRST_TASK = "/change 1 Code Calendo";
        private readonly string RESULT_CHANGE_FIRST_TASK_DESCRIPTION = "Code Calendo";
        private const int MAX_CASES = 100;

        /// <summary>
        /// Tests the add command
        /// </summary>
        [TestMethod]
        public void CommandAdd()
        {
            CommandProcessor commandProcessor = new CommandProcessor();
            InitializeTest(ref commandProcessor);

            // Add a few tasks
            commandProcessor.ExecuteCommand(COMMAND_ADD_FLOATING_TASK);
            commandProcessor.ExecuteCommand(COMMAND_ADD_DEADLINE_TASK);
            commandProcessor.ExecuteCommand(COMMAND_ADD_DURATION_TASK);

            List<Entry> taskList = commandProcessor.TaskList;

            // Length of taskList should be equal to the number of tasks added
            Assert.AreEqual(taskList.Count, NUMBER_OF_ADD_TESTS);

            // All the tasks in taskList should have the expected results
            Assert.AreEqual(RESULT_ADD_FLOATING_TASK_DESCRIPTION, taskList[0].Description);
            Assert.AreEqual(RESULT_ADD_DEADLINE_TASK_DESCRIPTION, taskList[1].Description);
            Assert.AreEqual(RESULT_ADD_DEADLINE_TASK_DATETIME, taskList[1].StartTime);
            Assert.AreEqual(RESULT_ADD_DURATION_TASK_DESCRIPTION, taskList[2].Description);
            Assert.AreEqual(RESULT_ADD_DURATION_TASK_START_DATETIME, taskList[2].StartTime);
            Assert.AreEqual(RESULT_ADD_DURATION_TASK_END_DATETIME, taskList[2].EndTime);
        }

        /// <summary>
        /// Tests the remove command
        /// </summary>
        [TestMethod]
        public void CommandRemove()
        {
            CommandProcessor commandProcessor = new CommandProcessor();
            InitializeTest(ref commandProcessor);

            // Add a task
            commandProcessor.ExecuteCommand(COMMAND_ADD_FLOATING_TASK);
            // Delete the task
            commandProcessor.ExecuteCommand(COMMAND_DELETE_FIRST_TASK);

            List<Entry> taskList = commandProcessor.TaskList;

            // taskList should be empty
            Assert.AreEqual(0, taskList.Count);
        }

        /// <summary>
        /// Tests the change command
        /// </summary>
        [TestMethod]
        public void CommandChange()
        {
            CommandProcessor commandProcessor = new CommandProcessor();
            InitializeTest(ref commandProcessor);

            // Add a task
            commandProcessor.ExecuteCommand(COMMAND_ADD_FLOATING_TASK);
            // Change the task
            commandProcessor.ExecuteCommand(COMMAND_CHANGE_FIRST_TASK);

            List<Entry> taskList = commandProcessor.TaskList;

            // taskList should have the number of tasks added
            Assert.AreEqual(EXPECTED_NUMBER_OF_TASKS_CHANGE, taskList.Count);

            Assert.AreEqual(RESULT_CHANGE_FIRST_TASK_DESCRIPTION, taskList[0].Description);
        }

        //@author A0080933E
        /// <summary>
        /// Test for invalid commands
        /// </summary>
        [TestMethod]
        public void CommandInvalid()
        {
            CommandProcessor commandProcessor = new CommandProcessor();
            InitializeTest(ref commandProcessor);

            // Invalid command
            commandProcessor.ExecuteCommand("//");
            Assert.IsTrue(commandProcessor.TaskList.Count == 0);

            // Empty command
            commandProcessor.ExecuteCommand("/");
            Assert.IsTrue(commandProcessor.TaskList.Count == 0);

            // Non-existant command
            commandProcessor.ExecuteCommand("/nonexistant aa");
            Assert.IsTrue(commandProcessor.TaskList.Count == 0);

            // Non-existant parameter (convert to regular text)
            commandProcessor.ExecuteCommand("/add /para");
            Assert.IsTrue(commandProcessor.TaskList.Count == 1);
            Assert.IsTrue(commandProcessor.TaskList[0].Description == "/para");
            commandProcessor.TaskList.Clear();

            // Missing parameter
            commandProcessor.ExecuteCommand("/remove");
            Assert.IsTrue(commandProcessor.TaskList.Count == 0);

            // Invalid parameter
            commandProcessor.ExecuteCommand("/change -1");
            Assert.IsTrue(commandProcessor.TaskList.Count == 0);
            commandProcessor.ExecuteCommand("/change alphabet");
            Assert.IsTrue(commandProcessor.TaskList.Count == 0);
        }

        /// <summary>
        /// Prepares conditions for unit test
        /// </summary>
        /// <param name="commandProcessor">Command Processor</param>
        private void InitializeTest(ref CommandProcessor commandProcessor)
        {
            commandProcessor.IndexMap = GenerateMapping(MAX_CASES);
            TaskManager taskManager = TaskManager.Instance;
            taskManager.Load();
            taskManager.Entries.Clear();
            taskManager.Save();
            taskManager.Load();
        }

        /// <summary>
        /// Generates a one-to-one mapping
        /// </summary>
        /// <param name="max">Maximum number of mappings</param>
        /// <returns>Returns dictionary mapping</returns>
        private Dictionary<int, int> GenerateMapping(int max)
        {
            // Generates a one-to-one mapping
            Dictionary<int, int> directMap = new Dictionary<int, int>();
            for (int i = 1; i <= max; i++)
            {
                directMap.Add(i, i);
            }
            return directMap;
        }
    }
}
