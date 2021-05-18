using MCS.FOI.ExcelToPDF;
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

            using var watcher = new FileSystemWatcher("/app/uploads");
            watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

            watcher.Changed += OnChanged;
            watcher.Created += OnCreated;
            watcher.Deleted += OnDeleted;
            watcher.Renamed += OnRenamed;

            watcher.Filter = "*.xlsx";
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;



            //using var watcher = new PhysicalFilesWatcher("/app/uploads",)

            //DoWork();

            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();

        }


        public static void DoWork()
        {
            _fileProvider = new PhysicalFileProvider(@"/app/uploads"); // e.g. C:\temp
            WatchForFileChanges();
        }

        private static void WatchForFileChanges()
        {
            IEnumerable<string> files = Directory.EnumerateFiles("/app/uploads", "*.xlsx", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                if (_files.TryGetValue(file, out DateTime existingTime))
                {
                    _files.TryUpdate(file, File.GetLastWriteTime(file), existingTime);
                }
                else
                {
                    if (File.Exists(file))
                    {
                        _files.TryAdd(file, File.GetLastWriteTime(file));
                    }
                }
            }
            _fileChangeToken = _fileProvider.Watch("**/*.xlsx");
            _fileChangeToken.RegisterChangeCallback(Notify, default);
        }

        private static void Notify(object state)
        {
            Console.WriteLine("File activity found");
            WatchForFileChanges();
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
                    //services.AddHostedService<Worker>();
                    services.AddSingleton(typeof(ExcelFileProcessor));
                });
    }
}
