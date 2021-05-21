using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.FOI.CalenderToPDF
{
    public interface ICalendarFileProcessor
    {
        public bool ProcessCalendarFiles();       

        public string SourcePath { get; set; }

        public string DestinationPath { get; set; }

        public string FileName { get; set; }

        public int FailureAttemptCount { get; set; }

        public int WaitTimeinMilliSeconds { get; set; }
    }
}
