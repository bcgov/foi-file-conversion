using MCS.FOI.FileConversion.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace MCS.FOI.FileConversion.FileWatcher
{
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
