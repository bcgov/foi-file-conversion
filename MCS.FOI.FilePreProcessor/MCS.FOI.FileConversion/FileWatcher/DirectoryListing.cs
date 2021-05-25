using MCS.FOI.FileConversion.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


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
            List<string> watchFolders = Directory.GetDirectories(baseSharedPath, ConversionSettings.FolderSearchPattern, SearchOption.TopDirectoryOnly)
                .Where(f => new DirectoryInfo(f).CreationTimeUtc > DateTime.Now.AddDays(ConversionSettings.DayCountBehindToStart > 0 ? ConversionSettings.DayCountBehindToStart * -1 : -1)).ToList<string>();
            return watchFolders;
        }
    }
}
