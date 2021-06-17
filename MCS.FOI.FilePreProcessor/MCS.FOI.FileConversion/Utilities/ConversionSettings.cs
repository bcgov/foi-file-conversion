﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.FOI.FileConversion.Utilities
{
    /// <summary>
    /// Mapping POCO class to map conversion environment variables to strongly typed objects
    /// </summary>
    public static class ConversionSettings
    {
        public static Platform DeploymentPlatform { get; set; }
       
        public static string BaseWatchPath { get; set; }

        public static string[] MinistryIncomingPaths { get; set; }

        public static string FolderSearchPattern { get; set; }

        public static int FailureAttemptCount { get; set; }

        public static int WaitTimeInMilliSeconds { get; set; }

        public static int FileWatcherMonitoringDelayInMilliSeconds { get; set; }
        
        public static int DayCountBehindToStart { get; set; }

        public static string SyncfusionLicense { get; set; }

        public static string HTMLtoPdfWebkitPath { get; set; }

        public static string FileWatcherStartDate { get; set; }

    }

    public enum Platform
    {
        Linux=0,
        Windows=1
    }
}
