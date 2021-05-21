using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.FOI.FileConversion.Utilities
{
    public static class ConversionSettings
    {

        public static Platform DeploymentPlatform { get; set; }
       
        public static string BaseWatchPath { get; set; }

        public static string FolderSearchPattern { get; set; }

        public static string FailureAttemptCount { get; set; }

        public static string WaitTimeInMilliSeconds { get; set; }

        public static int DayCountBehindToStart { get; set; }

    }

    public enum Platform
    {
        Linux=0,
        Windows=1
    }
}
