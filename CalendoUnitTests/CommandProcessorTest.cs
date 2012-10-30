//@author Pallav
using Calendo.Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace CommandProcessorTest
{
    [TestClass]
    public class CommandProcessorTest
    {
        static private int EMPTY_YEAR_DEFAULT_INCREMENT_FROM_CURRENT = 1;
        static private int NEXT_YEAR = DateTime.Now.AddYears(EMPTY_YEAR_DEFAULT_INCREMENT_FROM_CURRENT).Year;
        static private int NUMBER_OF_ADD_TESTS = 3;
        static private string COMMAND_ADD_FLOATING_TASK = "/add Work on Calendo";
        static private string RESULT_ADD_FLOATING_TASK_DESCRIPTION = "Work on Calendo";
        static private string COMMAND_ADD_DEADLINE_TASK = "/add Finish Calendo /date 10/10/2012 /time 10:30";
        static private string RESULT_ADD_DEADLINE_TASK_DESCRIPTION = "Finish Calendo";
        static private DateTime RESULT_ADD_DEADLINE_TASK_DATETIME = new DateTime(2012, 10, 10, 10, 30, 0);
        static private string COMMAND_ADD_DURATION_TASK = "/add Add fancy stuff to Calendo /startdate 11/10/2012 /starttime 9:00 /enddate 9/11/2012 /endtime 21:00";
        static private string RESULT_ADD_DURATION_TASK_DESCRIPTION = "Add fancy stuff to Calendo";
        static private DateTime RESULT_ADD_DURATION_TASK_START_DATETIME = new DateTime(2012, 10, 11, 9, 0, 0);
        static private DateTime RESULT_ADD_DURATION_TASK_END_DATETIME = new DateTime(2012, 11, 9, 21, 0, 0);

        static private string COMMAND_DELETE_FIRST_TASK = "/delete 1";

        static private int EXPECTED_NUMBER_OF_TASKS_CHANGE = 1;
        static private string COMMAND_CHANGE_FIRST_TASK = "/change 1 Code Calendo";
        static private string RESULT_CHANGE_FIRST_TASK_DESCRIPTION = "Code Calendo";

        [TestMethod]
        public void CPAdd()
        {
            CommandProcessor commandProcessor = new CommandProcessor();
            commandProcessor.IndexMap = GenerateMapping(commandProcessor.TaskList.Count);

            // NOTE TO DEVELOPER: Do not assume Command Processor begin with zero tasks. (Especially if this unit test is run more than once.)
            int initial = commandProcessor.TaskList.Count;

            // Add a few tasks
            commandProcessor.ExecuteCommand(COMMAND_ADD_FLOATING_TASK);
            commandProcessor.ExecuteCommand(COMMAND_ADD_DEADLINE_TASK);
            commandProcessor.ExecuteCommand(COMMAND_ADD_DURATION_TASK);

            List<Entry> taskList = commandProcessor.TaskList;

            

            // Length of taskList should be equal to the number of tasks added
            Assert.AreEqual(taskList.Count, NUMBER_OF_ADD_TESTS + initial);

            // All the tasks in taskList should have the expected results
            Assert.AreEqual(RESULT_ADD_FLOATING_TASK_DESCRIPTION, taskList[0 + initial].Description);
            Assert.AreEqual(RESULT_ADD_DEADLINE_TASK_DESCRIPTION, taskList[1 + initial].Description);
            Assert.AreEqual(RESULT_ADD_DEADLINE_TASK_DATETIME, taskList[1 + initial].StartTime);
            Assert.AreEqual(RESULT_ADD_DURATION_TASK_DESCRIPTION, taskList[2 + initial].Description);
            Assert.AreEqual(RESULT_ADD_DURATION_TASK_START_DATETIME, taskList[2 + initial].StartTime);
            Assert.AreEqual(RESULT_ADD_DURATION_TASK_END_DATETIME, taskList[2 + initial].EndTime);
        }

        [TestMethod]
        public void CPDelete()
        {
            CommandProcessor commandProcessor = new CommandProcessor();
            commandProcessor.IndexMap = GenerateMapping(commandProcessor.TaskList.Count);

            // NOTE TO DEVELOPER: Do not assume Command Processor begin with zero tasks.
            int initial = commandProcessor.TaskList.Count;

            // Add a task
            commandProcessor.ExecuteCommand(COMMAND_ADD_FLOATING_TASK);
            // Delete the task
            commandProcessor.ExecuteCommand(COMMAND_DELETE_FIRST_TASK);

            List<Entry> taskList = commandProcessor.TaskList;

            // taskList should be empty
            Assert.AreEqual(0, taskList.Count - initial);
        }

        [TestMethod]
        public void CPChange()
        {
            CommandProcessor commandProcessor = new CommandProcessor();
            commandProcessor.IndexMap = GenerateMapping(commandProcessor.TaskList.Count);

            // NOTE TO DEVELOPER: Do not assume Command Processor begin with zero tasks.
            int initial = commandProcessor.TaskList.Count;

            // Add a task
            commandProcessor.ExecuteCommand(COMMAND_ADD_FLOATING_TASK);
            // Change the task
            commandProcessor.ExecuteCommand(COMMAND_CHANGE_FIRST_TASK);

            List<Entry> taskList = commandProcessor.TaskList;

            // taskList should have the number of tasks added
            Assert.AreEqual(EXPECTED_NUMBER_OF_TASKS_CHANGE, taskList.Count - initial);

            Assert.AreEqual(RESULT_CHANGE_FIRST_TASK_DESCRIPTION, taskList[0].Description);
        }

        //@author Nicholas
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
