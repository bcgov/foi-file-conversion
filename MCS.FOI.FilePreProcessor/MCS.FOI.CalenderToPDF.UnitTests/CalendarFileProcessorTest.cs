using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace MCS.FOI.CalenderToPDF.UnitTests
{
    [TestClass]
    public class CalendarFileProcessorTest
    {
        [TestMethod]
        public void ProcessCalendarFilesTest()
        {
            string basePath = @"../../../SharedLAN/Req1";
            string destinationPath = basePath + "/Output/folder1/folder1/";
            string sourcePath = $@"{basePath}/folder1/folder1/FOI-FileConversion Test iCalendar Request.ics";
            CalendarFileProcessor calendarFileProcessor = new CalendarFileProcessor(basePath, sourcePath, destinationPath);
            calendarFileProcessor.ProcessCalendarFiles();     
            bool isFileExists = File.Exists($@"{destinationPath}/FOI-FileConversion Test iCalendar Request.pdf");
            Assert.IsTrue(isFileExists);
        }
        [TestMethod]
        public void ReadFileTest()
        {
            string basePath = @"../../../SharedLAN/Req1";
            string destinationPath = basePath + "/Output/folder1/folder1/";
            string sourcePath = $@"{basePath}/folder1/folder1/FOI-FileConversion Test iCalendar Request.ics";
            CalendarFileProcessor calendarFileProcessor = new CalendarFileProcessor(basePath, sourcePath, destinationPath);
            string htmlString = calendarFileProcessor.ReadFIle();
            Assert.IsTrue(!string.IsNullOrEmpty(htmlString));
        }
    }
}

