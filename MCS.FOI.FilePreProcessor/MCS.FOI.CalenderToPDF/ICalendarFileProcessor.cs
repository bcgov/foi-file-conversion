using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.FOI.CalenderToPDF
{
    public interface ICalendarFileProcessor
    {
        public (bool, string, string) ProcessCalendarFiles();       

        public string SourcePath { get; set; }

        public string DestinationPath { get; set; }

        public string FileName { get; set; }

        public string Message { get; set; }
    }
}
