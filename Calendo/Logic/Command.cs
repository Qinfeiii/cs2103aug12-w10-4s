//@author A0091571E

namespace Calendo.Logic
{
    class Command
    {
        private string startDate;
        private string startTime;
        private string endDate;
        private string endTime;
        private string type;
        private string text;

        public string StartDate { get { return startDate; } }
        public string StartTime { get { return startTime; } }
        public string EndDate { get { return endDate; } }
        public string EndTime { get { return endTime; } }
        public string Type { get { return type; } }
        public string Text { get { return text; } }

        public Command(string userInput, ref CommandExtractors extractors)
        {
            Initialise();
            extractors.Extract(userInput,
                ref type,
                ref startDate,
                ref startTime,
                ref endDate,
                ref endTime,
                ref text);
        }

        private void Initialise()
        {
            startDate = null;
            startTime = null;
            endDate = null;
            endTime = null;
            type = null;
            text = null;
        }
    }
}
