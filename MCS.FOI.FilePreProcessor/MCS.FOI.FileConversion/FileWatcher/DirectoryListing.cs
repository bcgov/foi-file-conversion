using MCS.FOI.FileConversion.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Serilog;


namespace MCS.FOI.FileConversion.FileWatcher
{
    /// <summary>
    /// Directory listing class, with methods to populate the FOI request Folders to watch for FileWatcher
    /// TODO : This method will get updated as project project with FOI Request Data capture into database. As of now, we have
    /// this below static logic to fetch folder under the main network drive(shared LAN drive) and its by configurable "folder search pattern e.g. "S*" and days from which folders need to 
    /// be monitored.  As project progress, below functionality will get updated with a logic mapped with Database tables where FOI requests are captured.
    /// </summary>
    public static class DirectoryListing
    {
        public static List<string> GetRequestFoldersToWatch(string baseSharedPath)
        {
            DateTime startDatetime = DateTime.MinValue; //default DateTime to starts with.

            DateTime.TryParse(ConversionSettings.FileWatcherStartDate, out startDatetime);

            List<string> watchFolders = new List<string>();

            foreach (string subministrypath in ConversionSettings.MinistryIncomingPaths)
            {
                if (Directory.Exists(String.Concat(baseSharedPath, subministrypath)))
                {
                    List<string> _subwatchFolder = Directory.GetDirectories(String.Concat(baseSharedPath, subministrypath), ConversionSettings.FolderSearchPattern, SearchOption.TopDirectoryOnly)
                    .Where(f => new DirectoryInfo(f).CreationTimeUtc > startDatetime && new DirectoryInfo(f).Name != ConversionSettings.CFRArchiveFoldertoSkip).ToList<string>();
                    foreach (var folder in _subwatchFolder)
                    {
                        if (!folder.ToLower().Contains("new folder"))
                        {
                            watchFolders.Add(folder);
                            Log.Information($"Found folder to watch, but might be already watching! : {folder}");
                        }

                    }
                }
                else
                {
                    Log.Information($"Folder not found to watch!!!!!! : {subministrypath}");
                }

            }

            return watchFolders;
        }
    }
}
