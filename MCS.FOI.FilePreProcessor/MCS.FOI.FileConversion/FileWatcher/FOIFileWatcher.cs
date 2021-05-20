using MCS.FOI.CalenderToPDF;
using MCS.FOI.ExcelToPDF;
using MCS.FOI.FileConversion.Logger;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MCS.FOI.FileConversion.FileWatcher
{
    public class FOIFileWatcher
    {
        ConcurrentDictionary<string, (DateTime, string, DateTime?, string, string)> watcherLogger;
        FileSystemWatcher watcher;
        private string PathToWatch { get; set; }

        private List<string> FileTypes { get; set; }

        public FOIFileWatcher(string pathtowatch, List<string> fileTypes)
        {
            this.PathToWatch = pathtowatch;
            this.FileTypes = fileTypes;
            this.watcherLogger = new ConcurrentDictionary<string, (DateTime, string, DateTime?, string, string)>();
            
        }

        public void StartWatching()
        {
            //string logFilePath = $"{this.PathToWatch}\\Log";
            //CSVLogger.CreateCSV(logFilePath);
            foreach (string fileType in FileTypes)
            {
                watcher = new FileSystemWatcher(this.PathToWatch);
                watcher.NotifyFilter = NotifyFilters.Attributes
                                     | NotifyFilters.CreationTime
                                     | NotifyFilters.DirectoryName
                                     | NotifyFilters.FileName
                                     | NotifyFilters.LastAccess
                                     | NotifyFilters.LastWrite
                                     | NotifyFilters.Security
                                     | NotifyFilters.Size;

                //watcher.Changed += OnChanged;
                watcher.Created += OnCreated;
                //watcher.Deleted += OnDeleted;
                //watcher.Renamed += OnRenamed;

                watcher.Filter = $"*.{fileType}";
                watcher.IncludeSubdirectories = true;
                watcher.EnableRaisingEvents = true;
            }

        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            Console.WriteLine($"Changed: {e.FullPath}");
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            string extension = string.Empty;            
            string value = $"Created: {e.FullPath}";
            Console.WriteLine(value);
            Console.WriteLine($"Path to watch is {this.PathToWatch}");
            string logFilePath = $"{e.FullPath.Replace(e.Name, "")}\\Log";
            FileInfo fileInfo = new FileInfo(e.FullPath);
            
            bool isProcessed = false;
            string message = string.Empty;
            string outputPath = string.Empty;
            Task.Run(() =>
            {
                if (fileInfo != null)
                {
                    watcherLogger.TryAdd(fileInfo.FullName,(fileInfo.CreationTimeUtc, "Created", null, message, outputPath));
                    extension = fileInfo.Extension;
                    switch (extension)
                    {
                        case FileExtensions.xls:
                        case FileExtensions.xlsx:
                            watcherLogger[fileInfo.FullName] = (fileInfo.CreationTimeUtc, "In Progress", null, message, outputPath);
                            (isProcessed, message, outputPath) = ProcessExcelFiles(fileInfo);
                            break;
                        case FileExtensions.ics:
                            watcherLogger[fileInfo.FullName] = (fileInfo.CreationTimeUtc, "In Progress", null, message, outputPath);
                            (isProcessed, message, outputPath) = ProcessCalendarFiles(fileInfo);
                            break;
                        default:
                            break;
                    }
                    if(isProcessed)
                        watcherLogger[fileInfo.FullName] = (fileInfo.CreationTimeUtc, "Completed", DateTime.UtcNow, message, outputPath);
                    else
                        watcherLogger[fileInfo.FullName] = (fileInfo.CreationTimeUtc, "Failed", DateTime.UtcNow, message, outputPath);

                    //CSVLogger.UpdateRecords(logFilePath, watcherLogger);
                    CSVLogger.LogtoCSV(watcherLogger, logFilePath);
                }
                
            });

            //CSVLogger.UpdateRecords(logFilePath, watcherLogger);
            //CSVLogger.LogtoCSV(watcherLogger, logFilePath);
        }

        private (bool, string, string) ProcessExcelFiles(FileInfo fileInfo)
        {
            string sourcePath = fileInfo.FullName.Replace(fileInfo.Name, "");
            ExcelFileProcessor excelFileProcessor = new ExcelFileProcessor();
            excelFileProcessor.ExcelFileName = fileInfo.Name;
            excelFileProcessor.IsSinglePDFOutput = false;
            excelFileProcessor.ExcelSourceFilePath = sourcePath;
            excelFileProcessor.PdfOutputFilePath = getPdfOutputPath(excelFileProcessor.ExcelSourceFilePath);
            return excelFileProcessor.ConvertToPDF();
        }

        private (bool, string, string) ProcessCalendarFiles(FileInfo fileInfo)
        {
            string sourcePath = fileInfo.FullName.Replace(fileInfo.Name, "");
            CalendarFileProcessor calendarFileProcessor = new CalendarFileProcessor();
            calendarFileProcessor.FileName = fileInfo.Name;
            calendarFileProcessor.SourcePath = sourcePath;
            calendarFileProcessor.DestinationPath = getPdfOutputPath(calendarFileProcessor.SourcePath);
            return calendarFileProcessor.ProcessCalendarFiles();
        }

        private void OnDeleted(object sender, FileSystemEventArgs e) =>
            Console.WriteLine($"Deleted: {e.FullPath}");

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"Renamed:");
            Console.WriteLine($"    Old: {e.OldFullPath}");
            Console.WriteLine($"    New: {e.FullPath}");
        }

        private void OnError(object sender, ErrorEventArgs e) =>
            PrintException(e.GetException());


        private void PrintException(Exception? ex)
        {
            if (ex != null)
            {
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine("Stacktrace:");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();
                PrintException(ex.InnerException);
            }
        }

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
