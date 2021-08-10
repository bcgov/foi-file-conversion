using MCS.FOI.CalenderToPDF;
using MCS.FOI.ExcelToPDF;
using MCS.FOI.FileConversion.Logger;
using MCS.FOI.FileConversion.Utilities;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;



namespace MCS.FOI.FileConversion.FileWatcher
{
    /// <summary>
    /// This class is for Filewatching a Shared Network Drive from Windows Client machine. This component utilize's .NET Core's FileSystemWatcher Object to monitor the
    /// changes as per the provided path and fileextension and dependent on the DirectoryListing Class under FileWatcher.
    /// </summary>
    public class FOIFileWatcherWindowsBased
    {
        ConcurrentDictionary<string, (DateTime, string, DateTime?, string, string)> watcherLogger;  // Thread Safe concurrent dictionary to monitor/watch event for logging
                                                                                                    // FileSystemWatcher watcher;
        private string PathToWatch { get; set; } // Path to the FOI request Folder

        private List<string> FileTypes { get; set; } //FileTypes (.xls,.xlsx , .ics) to be watched

        public FOIFileWatcherWindowsBased(string pathtowatch, List<string> fileTypes)
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

            Log.Information($"Started Watching new Folder Path! {this.PathToWatch}");


            FileSystemWatcher watcher = new FileSystemWatcher(this.PathToWatch);
            
                watcher.NotifyFilter = NotifyFilters.Attributes
                                     | NotifyFilters.CreationTime
                                     | NotifyFilters.DirectoryName
                                     | NotifyFilters.FileName
                                     | NotifyFilters.LastAccess
                                     | NotifyFilters.LastWrite
                                     | NotifyFilters.Security
                                     | NotifyFilters.Size;


                watcher.Created += OnCreated;
                watcher.Error += OnError;


                foreach (string fileType in FileTypes)
                {
                    watcher.Filters.Add($"*.{fileType}");
                }
                watcher.IncludeSubdirectories = true;
                watcher.EnableRaisingEvents = true;
            
        }


        /// <summary>
        /// File Created event handled from FileSystemWatcher to invoke FileConversion Logic inside corresponding FileConversion Libraries
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            string extension = string.Empty;
            string value = $"Created: {e.FullPath}";
            Console.WriteLine(value);
            Console.WriteLine($"Path to watch is {this.PathToWatch}");
            Log.Information($"Created File Event for file Path! {e.FullPath}");
            string logFilePath = $"{e.FullPath.Replace(e.Name, "")}\\Log";
            FileInfo fileInfo = new FileInfo(e.FullPath);

            bool isProcessed = false;
            string message = string.Empty;
            string outputPath = string.Empty;


            if (fileInfo.Exists && !e.FullPath.Contains("\\~$"))
            {
                // Async File Conversion component based on extension.
                Task.Run(() =>
                {
                    if (fileInfo != null)
                    {
                        watcherLogger.TryAdd(fileInfo.FullName, (fileInfo.CreationTimeUtc, "Created", null, message, outputPath));
                        extension = fileInfo.Extension;

                        //Condition check for File Extension for triggering File Conversion logic
                        switch (extension)
                        {
                            case FileExtensions.xls:
                            case FileExtensions.xlsx:
                                watcherLogger[fileInfo.FullName] = (fileInfo.CreationTimeUtc, "In Progress", null, message, outputPath);
                                (isProcessed, message, outputPath) = ProcessExcelFiles(fileInfo); // Calling Excel Conversion Logic
                                break;
                            case FileExtensions.ics:
                                watcherLogger[fileInfo.FullName] = (fileInfo.CreationTimeUtc, "In Progress", null, message, outputPath);
                                (isProcessed, message, outputPath) = ProcessCalendarFiles(fileInfo); // Calling ICalender Conversion Logic
                                break;
                            default:
                                break;
                        }
                        if (isProcessed)
                            watcherLogger[fileInfo.FullName] = (fileInfo.CreationTimeUtc, "Completed", DateTime.UtcNow, message, outputPath);
                        else
                            watcherLogger[fileInfo.FullName] = (fileInfo.CreationTimeUtc, "Failed", DateTime.UtcNow, message, outputPath);

                        CSVLogger.LogtoCSV(watcherLogger, logFilePath); //Logging the events into FileLogger , under /logs folder on the FOI Request Folder
                    }

                }).ConfigureAwait(false);

            }
        }

        /// <summary>
        /// private method to pass the details(excel source path, desired output location, output format - single/multiple, attempts to overcome failures etc.) 
        /// into ExcelFileProcessor Library types
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        private (bool, string, string) ProcessExcelFiles(FileInfo fileInfo)
        {
            string sourcePath = fileInfo.FullName.Replace(fileInfo.Name, "");
            ExcelFileProcessor excelFileProcessor = new ExcelFileProcessor();
            excelFileProcessor.ExcelFileName = fileInfo.Name;
            excelFileProcessor.IsSinglePDFOutput = false;
            excelFileProcessor.ExcelSourceFilePath = sourcePath;
            excelFileProcessor.PdfOutputFilePath = getPdfOutputPath(excelFileProcessor.ExcelSourceFilePath);
            excelFileProcessor.WaitTimeinMilliSeconds = ConversionSettings.WaitTimeInMilliSeconds;
            excelFileProcessor.FailureAttemptCount = ConversionSettings.FailureAttemptCount;
            return excelFileProcessor.ConvertToPDF();
        }

        /// <summary>
        /// private method to pass the details(calender source path, desired output location, output format - single/multiple, attempts to overcome failures etc.) 
        /// into CalenderFileProcessor Library types 
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        private (bool, string, string) ProcessCalendarFiles(FileInfo fileInfo)
        {
            string sourcePath = fileInfo.FullName.Replace(fileInfo.Name, "");
            CalendarFileProcessor calendarFileProcessor = new CalendarFileProcessor();
            calendarFileProcessor.FileName = fileInfo.Name;
            calendarFileProcessor.SourcePath = sourcePath;
            calendarFileProcessor.DestinationPath = getPdfOutputPath(calendarFileProcessor.SourcePath);
            calendarFileProcessor.WaitTimeinMilliSeconds = ConversionSettings.WaitTimeInMilliSeconds;
            calendarFileProcessor.FailureAttemptCount = ConversionSettings.FailureAttemptCount;
            calendarFileProcessor.DeploymentPlatform = CalenderToPDF.Platform.Windows;
            calendarFileProcessor.HTMLtoPdfWebkitPath = ConversionSettings.HTMLtoPdfWebkitPath;
            return calendarFileProcessor.ProcessCalendarFiles();
        }

        private void OnError(object sender, ErrorEventArgs e) =>
            PrintException(e.GetException());


        private void PrintException(Exception? ex)
        {
            if (ex != null)
            {
                Log.Error($"Error happened during file watching evening {ex.Message}, {ex.StackTrace}");
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine("Stacktrace:");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();
                PrintException(ex.InnerException);
            }
        }

        /// <summary>
        /// PDF output path creation method, based source location
        /// </summary>
        /// <param name="excelsourcePath"></param>
        /// <returns></returns>
        private string getPdfOutputPath(string excelsourcePath)
        {
            if (!excelsourcePath.ToLower().Contains(@"\output\"))
            {
                return string.Concat(this.PathToWatch, @"\output\", excelsourcePath.Replace(this.PathToWatch, ""));
            }
            else
            {
                return string.Concat(excelsourcePath, @"\calenderattachments\");
            }
        }
    }
}
