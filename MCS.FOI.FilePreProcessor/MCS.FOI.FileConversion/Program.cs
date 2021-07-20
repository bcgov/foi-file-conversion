using MCS.FOI.FileConversion.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using ILogger = Serilog.ILogger;

namespace MCS.FOI.FileConversion
{
    /// <summary>
    /// Application entry point method, which invokes the Background Service / Worker for FileWatcher, then in turn the FileConversion
    /// </summary>
    public class Program
    {
        
        public static void Main(string[] args)
        {

            try
            {

                Log.Information("MCS FOI FileConversion Service is up");

                // based on the environment + OS platform , settings file is loaded into the memory to parse the settings/configurations to a strongly typed object.
                var configurationbuilder = new ConfigurationBuilder()
                        .AddJsonFile($"appsettings.json", true, true)
                        //.AddJsonFile($"appsettings.{environmentName}.json", true, true) // TEMP: Commented out, will track back when we migrate to Cloud/OS in future
                        .AddEnvironmentVariables().Build();

                Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configurationbuilder)                
                .CreateLogger();

                // Fetching Configuration values from setting file { appsetting.{ environment_platform}.json}
                //ConversionSettings.DeploymentPlatform = environmentName.ToLower().StartsWith("linux") ? Platform.Linux : Platform.Windows; //KEEPING MULTI PLATFORM CODE BASE LOGIC FOR FUTURE REFERENCE
                ConversionSettings.DeploymentPlatform = Platform.Windows; //Fixing to Windows platform for Win Server VM deployment, once with Linux/OS , will take environment
                ConversionSettings.BaseWatchPath = configurationbuilder.GetSection("ConversionSettings:BaseWatchPath").Value;
                ConversionSettings.FileWatcherStartDate = configurationbuilder.GetSection("ConversionSettings:FileWatcherStartDate").Value;
                ConversionSettings.FolderSearchPattern = configurationbuilder.GetSection("ConversionSettings:FolderSearchPattern").Value;
                ConversionSettings.SyncfusionLicense = configurationbuilder.GetSection("ConversionSettings:SyncfusionLicense").Value;
                ConversionSettings.HTMLtoPdfWebkitPath = configurationbuilder.GetSection("ConversionSettings:HTMLtoPdfWebkitPath").Value;
                IConfigurationSection ministryConfigSection = configurationbuilder.GetSection("ConversionSettings:MinistryIncomingPaths");

                ConversionSettings.MinistryIncomingPaths = ministryConfigSection.Get<string[]>();                              

                int.TryParse(configurationbuilder.GetSection("ConversionSettings:FailureAttemptCount").Value, out int failureattempt);

                ConversionSettings.FailureAttemptCount = failureattempt;// Max. recovery attempts after a failure.

                int.TryParse(configurationbuilder.GetSection("ConversionSettings:WaitTimeInMilliSeconds").Value, out int waittimemilliseconds);
                ConversionSettings.WaitTimeInMilliSeconds = waittimemilliseconds; // Wait time between recovery attempts after a failure

                int.TryParse(configurationbuilder.GetSection("ConversionSettings:FileWatcherMonitoringDelayInMilliSeconds").Value, out int fileWatcherMonitoringDelayInMilliSeconds);
                ConversionSettings.FileWatcherMonitoringDelayInMilliSeconds = fileWatcherMonitoringDelayInMilliSeconds; // Delay between file directory fetch.

                int.TryParse(configurationbuilder.GetSection("ConversionSettings:DayCountBehindToStart").Value, out int count);
                ConversionSettings.DayCountBehindToStart = count; // days behind to start from for File watching, this is for DirectoryList Object to set up static algo for FileWacthing, till DB integration.

                //Fetching Syncfusion License from settings
                Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(ConversionSettings.SyncfusionLicense);

                CreateHostBuilder(args).Build().Run();


            }
            catch (Exception ex)
            {
                Log.Information($" Error happpened while running the FOI File Conversion service. Exception message : {ex.Message} , StackTrace :{ex.StackTrace}");
            }
            finally
            {
                Console.WriteLine("Press enter to exit.");
                Console.ReadLine();
            }

        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
             
                .ConfigureServices((hostContext, services) =>
                {                    
                    services.AddHostedService<Worker>(); // Dependency Injection of  Worker / BG Service               
                })
            .UseWindowsService(); // Marking as Windows Service to silently execute the process.

    }
}
