using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;

namespace MCS.FOI.CalenderToPDF.UnitTests
{
    [TestClass]
    public class CalendarFileProcessorTest
    {
        [TestMethod]
        public void ProcessCalendarFilesTest()
        {
            string sharedpath = @"../../../SharedLAN/Req1";            
            CalendarFileProcessor calendarFileProcessor = new CalendarFileProcessor();
            calendarFileProcessor.ProcessCalendarFiles(sharedpath);
        }
    }
}
