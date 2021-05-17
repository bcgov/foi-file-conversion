using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.FOI.CalenderToPDF
{
    public interface ICalendarFileProcessor
    {
        public void ProcessCalendarFiles(string sourcePath);
    }
}
