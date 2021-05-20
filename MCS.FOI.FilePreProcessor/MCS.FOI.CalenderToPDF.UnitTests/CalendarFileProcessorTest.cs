using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace MCS.FOI.CalenderToPDF.UnitTests
{
    [TestClass]
    public class CalendarFileProcessorTest
    {
        [TestMethod]
        public void ProcessSimpleCalendarFilesTest()
        {
            bool isProcessed;
            string message = string.Empty;
            string rootFolder = getSourceFolder();
            CalendarFileProcessor calendarFileProcessor = new CalendarFileProcessor();
            calendarFileProcessor.SourcePath = string.Concat(rootFolder, @"\folder2\");
            calendarFileProcessor.DestinationPath = string.Concat(rootFolder, @"\output\", calendarFileProcessor.SourcePath.Replace(rootFolder, "")); 
            calendarFileProcessor.FileName = "iCalendar.ics";
                     
            (isProcessed, message) = calendarFileProcessor.ProcessCalendarFiles();
            Assert.IsTrue(isProcessed == true, $"Calendar to PDF Conversion failed for {calendarFileProcessor.FileName}");

            string outputFilePath = Path.Combine(calendarFileProcessor.DestinationPath, $"{Path.GetFileNameWithoutExtension(calendarFileProcessor.FileName)}.pdf");
            bool isFileExists = File.Exists(outputFilePath);
            Assert.IsTrue(isFileExists, $"Converted PDF file does not exists {calendarFileProcessor.FileName}");
        }
        [TestMethod]
        public void ProcessCalendarFileWithAttachmentsTest()
        {
            bool isProcessed;
            string message = string.Empty;
            string rootFolder = getSourceFolder();
            CalendarFileProcessor calendarFileProcessor = new CalendarFileProcessor();
            calendarFileProcessor.SourcePath = string.Concat(rootFolder, @"\folder3\");
            calendarFileProcessor.DestinationPath = string.Concat(rootFolder, @"\output\", calendarFileProcessor.SourcePath.Replace(rootFolder, ""));
            calendarFileProcessor.FileName = "FOI-FileConversion Test iCalendar Request.ics";

            (isProcessed, message) = calendarFileProcessor.ProcessCalendarFiles();
            Assert.IsTrue(isProcessed == true, $"Calendar to PDF Conversion failed for {calendarFileProcessor.FileName}");

            string outputFilePath = Path.Combine(calendarFileProcessor.DestinationPath, $"{Path.GetFileNameWithoutExtension(calendarFileProcessor.FileName)}.pdf");
            bool isFileExists = File.Exists(outputFilePath);
            Assert.IsTrue(isFileExists, $"Converted PDF file does not exists {calendarFileProcessor.FileName}");
        }
        [TestMethod]
        public void ProcessFolderLevelCalendarFileTest()
        {
            bool isProcessed;
            string message = string.Empty;
            string rootFolder = getSourceFolder();
            CalendarFileProcessor calendarFileProcessor = new CalendarFileProcessor();
            calendarFileProcessor.SourcePath = string.Concat(rootFolder, @"\folder1\folder1\");
            calendarFileProcessor.DestinationPath = string.Concat(rootFolder, @"\output\", calendarFileProcessor.SourcePath.Replace(rootFolder, ""));
            calendarFileProcessor.FileName = "FOI-FileConversion Test iCalendar Request.ics";

            (isProcessed, message) = calendarFileProcessor.ProcessCalendarFiles();
            Assert.IsTrue(isProcessed == true, $"Calendar to PDF Conversion failed for {calendarFileProcessor.FileName}");

            string outputFilePath = Path.Combine(calendarFileProcessor.DestinationPath, $"{Path.GetFileNameWithoutExtension(calendarFileProcessor.FileName)}.pdf");
            bool isFileExists = File.Exists(outputFilePath);
            Assert.IsTrue(isFileExists, $"Converted PDF file does not exists {calendarFileProcessor.FileName}");
        }
        [TestMethod]
        public void ProcessComplexCalendarFilesTest()
        {
            bool isProcessed;
            string message = string.Empty;
            string rootFolder = getSourceFolder();
            CalendarFileProcessor calendarFileProcessor = new CalendarFileProcessor();
            calendarFileProcessor.SourcePath = string.Concat(rootFolder, @"\folder2\");
            calendarFileProcessor.DestinationPath = string.Concat(rootFolder, @"\output\", calendarFileProcessor.SourcePath.Replace(rootFolder, ""));
            calendarFileProcessor.FileName = "divya.v@aot-technologies.com.ics";

            (isProcessed, message) = calendarFileProcessor.ProcessCalendarFiles();
            Assert.IsTrue(isProcessed == true, $"Calendar to PDF Conversion failed for {calendarFileProcessor.FileName}");

            string outputFilePath = Path.Combine(calendarFileProcessor.DestinationPath, $"{Path.GetFileNameWithoutExtension(calendarFileProcessor.FileName)}.pdf");
            bool isFileExists = File.Exists(outputFilePath);
            Assert.IsTrue(isFileExists, $"Converted PDF file does not exists {calendarFileProcessor.FileName}");
        }
        private string getSourceFolder()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string approot = currentDirectory.Replace(@"\bin\Debug\netcoreapp3.1", "");
            return Path.Combine(approot, @"SharedLAN\Req1");

        }
    }
}

