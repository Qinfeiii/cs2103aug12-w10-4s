//@author A0091571E

namespace Calendo.Logic
{
    class Command
    {
        public string StartDate { get; set; }
        public string StartTime { get; set; }
        public string EndDate { get; set; }
        public string EndTime { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }

        public Command()
        {
            Initialise();
        }

        private void Initialise()
        {
            StartDate = null;
            StartTime = null;
            EndDate = null;
            EndTime = null;
            Type = null;
            Text = null;
        }
    }
}
