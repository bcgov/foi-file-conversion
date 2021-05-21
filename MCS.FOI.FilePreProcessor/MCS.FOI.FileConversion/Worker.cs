using MCS.FOI.FileConversion.FileWatcher;
using MCS.FOI.FileConversion.Utilities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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

                logger.LogInformation("FOI File Watcher running at: {time}", DateTimeOffset.Now);

                try
                {
                    
                    var foldersListed = DirectoryListing.GetRequestFoldersToWatch($@"{ConversionSettings.BaseWatchPath}");

                    foreach (string folderpath in foldersListed)
                    {
                        if (!folderWatchstatus.ContainsKey(folderpath))
                        {
                            if (ConversionSettings.DeploymentPlatform == Platform.Windows)
                            {
                                var filewatcher = new FOIFileWatcher(folderpath, new List<string>() { "xls", "xlsx", "ics" });
                                filewatcher.StartWatching();
                            }
                            else
                            {
                                var filewatcher = new FOIFileWatcherLinuxBased(folderpath, new List<string>() { "xls", "xlsx", "ics" });
                                filewatcher.StartWatching();
                            }

                            folderWatchstatus.Add(folderpath, "Watching");
                        }

                    }
                }
                catch(Exception ex)
                {
                    logger.LogError($" Error happened during FOI file catching {ex.Message} , stacktrace : {ex.StackTrace}");
                }

                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
