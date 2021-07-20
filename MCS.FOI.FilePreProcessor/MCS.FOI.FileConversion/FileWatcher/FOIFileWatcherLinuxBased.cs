using MCS.FOI.CalenderToPDF;
using MCS.FOI.ExcelToPDF;
using MCS.FOI.FileConversion.Logger;
using MCS.FOI.FileConversion.Utilities;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace MCS.FOI.FileConversion.FileWatcher
{
    /// <summary>
    /// This class is for Filewatching a Shared Network Drive from Linux Client machine.This component utilize's .NET Core's PhysicalFileProvider Object to monitor the
    /// changes as per the provided path and fileextension and dependent on the DirectoryListing Class under FileWatcher.
    /// </summary>
    public class FOIFileWatcherLinuxBased
    {
        private IChangeToken _fileChangeToken;
        private PhysicalFileProvider _fileProvider;
        private readonly ConcurrentDictionary<string, (DateTime, DateTime)> _files = new ConcurrentDictionary<string, (DateTime, DateTime)>();
        ConcurrentDictionary<string, (DateTime, string, DateTime?, string, string)> watcherLogger;

        private string PathToWatch { get; set; }
        private List<string> FileTypes { get; set; }

        public FOIFileWatcherLinuxBased(string pathtowatch, List<string> fileTypes)
        {
            this.PathToWatch = pathtowatch;
            this.FileTypes = fileTypes;
            this.watcherLogger = new ConcurrentDictionary<string, (DateTime, string, DateTime?, string, string)>();
        }

        /// <summary>
        /// Main Watcher method which is called from the entry point , based on the client deployment(Linux vs Windows) of the FileWatcher
        /// </summary>
        public void StartWatching()
        {
            foreach (string fileType in this.FileTypes)
            {
                StartWatchingByFileType(fileType);
            }
        }

        private void StartWatchingByFileType(string fileType)
        {
            string check = "";           

            IEnumerable<string> filesbyfiletypes = Directory.EnumerateFiles(this.PathToWatch, $"*.{fileType}", SearchOption.AllDirectories);

            foreach (string file in filesbyfiletypes)
            {
                FileInfo fileInfo = new FileInfo(file);
                if (_files.TryGetValue(file, out (DateTime, DateTime) timestamp))
                {
                    if (fileInfo.LastAccessTimeUtc != timestamp.Item2)
                    {
                        _files[file] = (fileInfo.LastAccessTimeUtc, timestamp.Item2);
                        ProcessFile(fileInfo);
                    }

                }
                else
                {
                    DateTime addedTime = DateTime.Now.ToUniversalTime();
                    if (File.Exists(file))
                    {
                        _files.TryAdd(file, (addedTime, addedTime));
                        ProcessFile(fileInfo);
                    }
                }

            }

            _fileProvider = new PhysicalFileProvider(this.PathToWatch);
            _fileChangeToken = _fileProvider.Watch($"**/*.{fileType}");

            _fileProvider.UsePollingFileWatcher = true;
            _fileProvider.UseActivePolling = true;

            switch ($".{fileType}")
            {
                case FileExtensions.xls:
                    _fileChangeToken.RegisterChangeCallback(XLSEvent, fileType);
                    break;
                case FileExtensions.xlsx:
                    _fileChangeToken.RegisterChangeCallback(XLSXEvent, fileType);
                    break;
                case FileExtensions.ics:
                    _fileChangeToken.RegisterChangeCallback(ICSEvent, fileType);
                    break;
                default:
                    break;
            }

        }


        private (bool, string, string) ProcessExcelFiles(FileInfo fileInfo)
        {

            var sourcePath = fileInfo.FullName.Replace(fileInfo.Name, "");
            var fileName = fileInfo.Name;
            ExcelFileProcessor excelFileProcessor = new ExcelFileProcessor();
            excelFileProcessor.ExcelFileName = fileName;
            excelFileProcessor.IsSinglePDFOutput = false;
            excelFileProcessor.ExcelSourceFilePath = sourcePath;
            excelFileProcessor.PdfOutputFilePath = getPdfOutputPath(excelFileProcessor.ExcelSourceFilePath);
            excelFileProcessor.WaitTimeinMilliSeconds = ConversionSettings.WaitTimeInMilliSeconds;
            excelFileProcessor.FailureAttemptCount = ConversionSettings.FailureAttemptCount;
            return excelFileProcessor.ConvertToPDF();
        }

        private (bool, string, string) ProcessCalendarFiles(FileInfo fileInfo)
        {
            var sourcePath = fileInfo.FullName.Replace(fileInfo.Name, "");
            var fileName = fileInfo.Name;

            CalendarFileProcessor calendarFileProcessor = new CalendarFileProcessor();
            calendarFileProcessor.FileName = fileName;
            calendarFileProcessor.SourcePath = sourcePath;
            calendarFileProcessor.DestinationPath = getPdfOutputPath(calendarFileProcessor.SourcePath);
            calendarFileProcessor.WaitTimeinMilliSeconds = ConversionSettings.WaitTimeInMilliSeconds;
            calendarFileProcessor.FailureAttemptCount = ConversionSettings.FailureAttemptCount;
            calendarFileProcessor.DeploymentPlatform = CalenderToPDF.Platform.Linux;
            calendarFileProcessor.HTMLtoPdfWebkitPath = ConversionSettings.HTMLtoPdfWebkitPath;
            return calendarFileProcessor.ProcessCalendarFiles();
        }

        /// <summary>
        /// PhysicalFile Provider's File ChangeToken event handler to recognize XLS file type updates
        /// </summary>
        /// <param name="state"></param>
        private void XLSEvent(Object state)
        {
            Console.WriteLine("An XLS detected");
            StartWatchingByFileType(state.ToString());
        }

        /// <summary>
        /// PhysicalFile Provider's File ChangeToken event handler to recognize XLSX file type updates
        /// </summary>
        /// <param name="state"></param>
        private void XLSXEvent(Object state)
        {
            Console.WriteLine("An XLSX detected");
            StartWatchingByFileType(state.ToString());
        }

        /// <summary>
        /// PhysicalFile Provider's File ChangeToken event handler to recognize ICS(Calender) file type updates
        /// </summary>
        /// <param name="state"></param>
        private void ICSEvent(Object state)
        {
            Console.WriteLine("An ICS detected");
            StartWatchingByFileType(state.ToString());
        }



        private string getPdfOutputPath(string excelsourcePath)
        {

            if (!excelsourcePath.ToLower().Contains(@"/output/"))
            {
                return string.Concat(this.PathToWatch, @"/output/", excelsourcePath.Replace(this.PathToWatch, ""));
            }
            else
            {
                return string.Concat(excelsourcePath, @"/calenderattachments/");
            }
        }

        /// <summary>
        /// File Processing generic main private method to invoke File Type specfic Conversion logic , based on File Extension
        /// </summary>
        /// <param name="fileInfo"></param>
        private void ProcessFile(FileInfo fileInfo)
        {
            bool isProcessed = false;
            string message = string.Empty;
            string outputPath = string.Empty;
            string logFilePath = $"{this.PathToWatch}/Log";

            watcherLogger.TryAdd(fileInfo.FullName, (fileInfo.CreationTimeUtc, "Created", null, message, outputPath));

            //Condition check for File Extension for triggering File Conversion logic
            switch (fileInfo.Extension)
            {
                case FileExtensions.xls:
                case FileExtensions.xlsx:
                    watcherLogger[fileInfo.FullName] = (fileInfo.CreationTimeUtc, "In Progress", null, message, outputPath);
                    (isProcessed, message, outputPath) = ProcessExcelFiles(fileInfo);
                    break;
                case FileExtensions.ics:
                    (isProcessed, message, outputPath) =  ProcessCalendarFiles(fileInfo);
                    break;

                default:
                    break;
            }
            if (isProcessed)
                watcherLogger[fileInfo.FullName] = (fileInfo.CreationTimeUtc, "Completed", DateTime.UtcNow, message, outputPath);
            else
                watcherLogger[fileInfo.FullName] = (fileInfo.CreationTimeUtc, "Failed", DateTime.UtcNow, message, outputPath);
          
            CSVLogger.LogtoCSV(watcherLogger, logFilePath);
        }
    }
}
