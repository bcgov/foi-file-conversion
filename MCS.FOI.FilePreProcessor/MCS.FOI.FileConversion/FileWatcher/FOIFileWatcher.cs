using MCS.FOI.CalenderToPDF;
using MCS.FOI.ExcelToPDF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MCS.FOI.FileConversion.FileWatcher
{
    public class FOIFileWatcher
    {

        FileSystemWatcher watcher;
        private string PathToWatch { get; set; }

        private List<string> FileTypes { get; set; }

        public FOIFileWatcher(string pathtowatch, List<string> fileTypes)
        {
            this.PathToWatch = pathtowatch;
            this.FileTypes = fileTypes;
        }

        public void StartWatching()
        {

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
            string sourcePath = string.Empty;
            string fileName = string.Empty;
            string value = $"Created: {e.FullPath}";
            Console.WriteLine(value);
            Console.WriteLine($"Path to watch is {this.PathToWatch}");

            FileInfo fileInfo = new FileInfo(e.FullPath);
            if (fileInfo != null)
            {
                extension = fileInfo.Extension;
                fileName = fileInfo.Name;
                sourcePath = e.FullPath.Replace(fileInfo.Name, "");
            }
            Task.Run(() =>
            {
                switch (extension)
                {
                    case FileExtensions.xls:
                    case FileExtensions.xlsx:
                        ProcessExcelFiles(fileName, sourcePath);
                        break;
                    case FileExtensions.ics:
                        ProcessCalendarFiles(fileName, sourcePath);
                        break;
                    default:
                        break;
                }

            });

        }

        private void ProcessExcelFiles(string fileName, string sourcePath)
        {
            ExcelFileProcessor excelFileProcessor = new ExcelFileProcessor();
            excelFileProcessor.ExcelFileName = fileName;
            excelFileProcessor.IsSinglePDFOutput = false;
            excelFileProcessor.ExcelSourceFilePath = sourcePath;
            excelFileProcessor.PdfOutputFilePath = getPdfOutputPath(excelFileProcessor.ExcelSourceFilePath);
            excelFileProcessor.ConvertToPDF();
        }

        private void ProcessCalendarFiles(string fileName, string sourcePath)
        {
            CalendarFileProcessor calendarFileProcessor = new CalendarFileProcessor();
            calendarFileProcessor.FileName = fileName;
            calendarFileProcessor.SourcePath = sourcePath;
            calendarFileProcessor.DestinationPath = getPdfOutputPath(calendarFileProcessor.SourcePath);
            calendarFileProcessor.ProcessCalendarFiles();
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
