using MCS.FOI.ExcelToPDF;
using MCS.FOI.FileConversion.FileWatcher;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MCS.FOI.FileConversion
{
    public class Program
    {
        private static IChangeToken _fileChangeToken;
        private static PhysicalFileProvider _fileProvider;
        private static readonly ConcurrentDictionary<string, DateTime> _files = new ConcurrentDictionary<string, DateTime>();


        public static void Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run();

            DirectoryListing directoryListing = new DirectoryListing(@"\\sfp.idir.bcgov\S177\S77104\Agile Test");
            var foldersListed = directoryListing.GetRequestFoldersToWatch();

            foreach (string folderpath in foldersListed)
            {
               var xlsfilewatcher = new FOIFileWatcher(folderpath, new List<string>() { "xls","xlsx","ics" });
               xlsfilewatcher.StartWatching();
               
            }

            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();

        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            Console.WriteLine($"Changed: {e.FullPath}");
        }

        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            string value = $"Created: {e.FullPath}";
            Console.WriteLine(value);
        }

        private static void OnDeleted(object sender, FileSystemEventArgs e) =>
            Console.WriteLine($"Deleted: {e.FullPath}");

        private static void OnRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"Renamed:");
            Console.WriteLine($"    Old: {e.OldFullPath}");
            Console.WriteLine($"    New: {e.FullPath}");
        }

        private static void OnError(object sender, ErrorEventArgs e) =>
            PrintException(e.GetException());


        private static void PrintException(Exception? ex)
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



        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddSingleton(typeof(ExcelFileProcessor));
                    services.AddSingleton(typeof(FileSystemWatcher));
                });
    }
}
