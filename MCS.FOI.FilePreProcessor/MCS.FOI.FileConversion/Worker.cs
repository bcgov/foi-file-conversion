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
    /// <summary>
    /// This class is the Background Service / Worker running, which invokes the required type of FileWatcher based on the client/running platform (Linux vs Windows)
    /// 
    /// </summary>
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;

        Dictionary<string, string> folderWatchstatus = new Dictionary<string, string>();

        public Worker(ILogger<Worker> _logger)
        {
            logger = _logger; // Console Event Logger

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                logger.LogInformation("FOI File Watcher running at: {time}", DateTimeOffset.Now);

                try
                {
                    
                    //Fetching the list of FOI Request Folder path to monitor/ FileWatched
                    // DirectoryListing logic is partially acheived based on a static, configurable logic. Once FOI requests are captured into DB, the directory listing
                    //will connected to DB to get the list Folders to be watched.
                    var foldersListed = DirectoryListing.GetRequestFoldersToWatch($@"{ConversionSettings.BaseWatchPath}"); 

                    foreach (string folderpath in foldersListed)
                    {
                        if (!folderWatchstatus.ContainsKey(folderpath)) // Logic to understand whether its already under Filewatching.
                        {
                            if (ConversionSettings.DeploymentPlatform == Platform.Windows) // Check for deployment platform for invoking specific type of FileWatcher logic.
                            {
                                //Invoking Windows based File Watcher
                                var filewatcher = new FOIFileWatcherWindowsBased(folderpath, new List<string>() { "xls", "xlsx", "ics" });
                                filewatcher.StartWatching();
                            }
                            else
                            {
                                //Invoking Linux based File Watcher
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

                //Delaying for next watch, this is as per config from ENV VAR
                await Task.Delay(ConversionSettings.FileWatcherMonitoringDelayInMilliSeconds, stoppingToken);
            }
        }
    }
}
