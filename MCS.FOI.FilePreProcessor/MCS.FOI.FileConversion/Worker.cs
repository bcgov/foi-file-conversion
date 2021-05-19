using MCS.FOI.ExcelToPDF;
using MCS.FOI.FileConversion.FileWatcher;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MCS.FOI.FileConversion
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;

        Dictionary<string, string> folderWatchstatus = new Dictionary<string, string>();

        public Worker(ILogger<Worker> _logger)
        {
            logger = _logger;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                var foldersListed = DirectoryListing.GetRequestFoldersToWatch(@"\\sfp.idir.bcgov\S177\S77104\Agile Test");

                foreach (string folderpath in foldersListed)
                {
                    if (!folderWatchstatus.ContainsKey(folderpath))
                    {
                        var xlsfilewatcher = new FOIFileWatcher(folderpath, new List<string>() { "xls", "xlsx", "ics" });
                        xlsfilewatcher.StartWatching();
                        folderWatchstatus.Add(folderpath, "Watching");
                    }

                }

                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
