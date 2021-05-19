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
            List<string> watchFolders = Directory.GetDirectories(baseSharedPath, "S*", SearchOption.TopDirectoryOnly).Where(f => new DirectoryInfo(f).CreationTimeUtc > DateTime.Now.AddDays(-6)).ToList<string>();
            return watchFolders;
        }

    }
}
