using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace MCS.FOI.FileConversion.FileWatcher
{
    public class DirectoryListing
    {
        public DirectoryListing(string _basepath)
        {
            this.BaseSharedPath = _basepath;
        }
        private string BaseSharedPath { get; set; }

        public List<string> GetRequestFoldersToWatch()
        {
            List<string> watchFolders = Directory.GetDirectories(this.BaseSharedPath, "S*", SearchOption.TopDirectoryOnly).Where(f => new DirectoryInfo(f).CreationTimeUtc > DateTime.Now.AddDays(-6)).ToList<string>();
            return watchFolders;
        }

    }
}
