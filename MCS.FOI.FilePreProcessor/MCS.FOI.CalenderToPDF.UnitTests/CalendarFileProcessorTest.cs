using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace MCS.FOI.CalenderToPDF.UnitTests
{
    [TestClass]
    public class CalendarFileProcessorTest
    {
        public CalendarFileProcessorTest()
        {            
            checkWebkitENVVAR();
        }

        private void checkWebkitENVVAR()
        {

            #if DEBUG
                Environment.SetEnvironmentVariable("HTMLtoPdfWebkitPath","");//Enter local path, if required on debug execution.
            #endif

            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("HTMLtoPdfWebkitPath")))
            {
                var errorENV = "HTMLtoPdfWebkitPath ENV VAR missing!";
                Console.WriteLine(errorENV);
                Assert.Fail(errorENV);
            }
        }

        [TestMethod]
        public void ProcessSimpleCalendarFilesTest()
        {

            checkWebkitENVVAR();

            bool isProcessed;
            string message = string.Empty;
            string rootFolder = getSourceFolder();
            CalendarFileProcessor calendarFileProcessor = new CalendarFileProcessor();
            calendarFileProcessor.SourcePath = string.Concat(rootFolder, @"\folder2\");
            calendarFileProcessor.DestinationPath = string.Concat(rootFolder, @"\output\", calendarFileProcessor.SourcePath.Replace(rootFolder, "")); 
            calendarFileProcessor.FileName = "iCalendar.ics";
            calendarFileProcessor.FailureAttemptCount = 5;
            calendarFileProcessor.WaitTimeinMilliSeconds = 4000;
            calendarFileProcessor.HTMLtoPdfWebkitPath = Environment.GetEnvironmentVariable("HTMLtoPdfWebkitPath");

            (isProcessed, message, calendarFileProcessor.DestinationPath) = calendarFileProcessor.ProcessCalendarFiles();
            Assert.IsTrue(isProcessed == true, $"Calendar to PDF Conversion failed for {calendarFileProcessor.FileName}");

            string outputFilePath = Path.Combine(calendarFileProcessor.DestinationPath, $"{Path.GetFileNameWithoutExtension(calendarFileProcessor.FileName)}.pdf");
            bool isFileExists = File.Exists(outputFilePath);
            Assert.IsTrue(isFileExists, $"Converted PDF file does not exists {calendarFileProcessor.FileName}");
        }
        [TestMethod]
        public void ProcessCalendarFileWithAttachmentsTest()
        {
            checkWebkitENVVAR();

            bool isProcessed;
            string message = string.Empty;
            string rootFolder = getSourceFolder();
            CalendarFileProcessor calendarFileProcessor = new CalendarFileProcessor();
            calendarFileProcessor.SourcePath = string.Concat(rootFolder, @"\folder3\");
            calendarFileProcessor.DestinationPath = string.Concat(rootFolder, @"\output\", calendarFileProcessor.SourcePath.Replace(rootFolder, ""));
            calendarFileProcessor.FileName = "FOI-FileConversion Test iCalendar Request.ics";
            calendarFileProcessor.FailureAttemptCount = 5;
            calendarFileProcessor.WaitTimeinMilliSeconds = 4000;
            calendarFileProcessor.DeploymentPlatform = Platform.Windows;
            calendarFileProcessor.HTMLtoPdfWebkitPath = Environment.GetEnvironmentVariable("HTMLtoPdfWebkitPath");
            (isProcessed, message, calendarFileProcessor.DestinationPath) = calendarFileProcessor.ProcessCalendarFiles();
            Assert.IsTrue(isProcessed == true, $"Calendar to PDF Conversion failed for {calendarFileProcessor.FileName}");

            string outputFilePath = Path.Combine(calendarFileProcessor.DestinationPath, $"{Path.GetFileNameWithoutExtension(calendarFileProcessor.FileName)}.pdf");
            bool isFileExists = File.Exists(outputFilePath);
            Assert.IsTrue(isFileExists, $"Converted PDF file does not exists {calendarFileProcessor.FileName}");
        }
        [TestMethod]
        public void ProcessFolderLevelCalendarFileTest()
        {
            checkWebkitENVVAR();

            bool isProcessed;
            string message = string.Empty;
            string rootFolder = getSourceFolder();
            CalendarFileProcessor calendarFileProcessor = new CalendarFileProcessor();
            calendarFileProcessor.SourcePath = string.Concat(rootFolder, @"\folder1\folder1\");
            calendarFileProcessor.DestinationPath = string.Concat(rootFolder, @"\output\", calendarFileProcessor.SourcePath.Replace(rootFolder, ""));
            calendarFileProcessor.FileName = "FOI-FileConversion Test iCalendar Request.ics";
            calendarFileProcessor.FailureAttemptCount = 5;
            calendarFileProcessor.WaitTimeinMilliSeconds = 4000;
            calendarFileProcessor.DeploymentPlatform = Platform.Windows;
            calendarFileProcessor.HTMLtoPdfWebkitPath = Environment.GetEnvironmentVariable("HTMLtoPdfWebkitPath");
            (isProcessed, message, calendarFileProcessor.DestinationPath) = calendarFileProcessor.ProcessCalendarFiles();
            Assert.IsTrue(isProcessed == true, $"Calendar to PDF Conversion failed for {calendarFileProcessor.FileName}");

            string outputFilePath = Path.Combine(calendarFileProcessor.DestinationPath, $"{Path.GetFileNameWithoutExtension(calendarFileProcessor.FileName)}.pdf");
            bool isFileExists = File.Exists(outputFilePath);
            Assert.IsTrue(isFileExists, $"Converted PDF file does not exists {calendarFileProcessor.FileName}");
        }
        [TestMethod]
        public void ProcessComplexCalendarFilesTest()
        {
            checkWebkitENVVAR();
            bool isProcessed;
            string message = string.Empty;
            string rootFolder = getSourceFolder();
            CalendarFileProcessor calendarFileProcessor = new CalendarFileProcessor();
            calendarFileProcessor.SourcePath = string.Concat(rootFolder, @"\folder2\");
            calendarFileProcessor.DestinationPath = string.Concat(rootFolder, @"\output\", calendarFileProcessor.SourcePath.Replace(rootFolder, ""));
            calendarFileProcessor.FileName = "divya.v@aot-technologies.com.ics";
            calendarFileProcessor.FailureAttemptCount = 5;
            calendarFileProcessor.WaitTimeinMilliSeconds = 4000;
            calendarFileProcessor.DeploymentPlatform = Platform.Windows;
            calendarFileProcessor.HTMLtoPdfWebkitPath = Environment.GetEnvironmentVariable("HTMLtoPdfWebkitPath");
            (isProcessed, message, calendarFileProcessor.DestinationPath) = calendarFileProcessor.ProcessCalendarFiles();
            Assert.IsTrue(isProcessed == true, $"Calendar to PDF Conversion failed for {calendarFileProcessor.FileName}");

            string outputFilePath = Path.Combine(calendarFileProcessor.DestinationPath, $"{Path.GetFileNameWithoutExtension(calendarFileProcessor.FileName)}.pdf");
            bool isFileExists = File.Exists(outputFilePath);
            Assert.IsTrue(isFileExists, $"Converted PDF file does not exists {calendarFileProcessor.FileName}");
        }
        private string getSourceFolder()
        {
            #if DEBUG
                string currentDirectory = Directory.GetCurrentDirectory();
                string approot = currentDirectory.Replace(@"\bin\Debug\netcoreapp3.1", "");
            #endif

            return Path.Combine(approot, @"SharedLAN\Req1");

        }
    }
}

