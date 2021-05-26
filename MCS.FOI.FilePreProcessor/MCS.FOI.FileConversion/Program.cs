using MCS.FOI.FileConversion.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace MCS.FOI.FileConversion
{
    /// <summary>
    /// Application entry point method, which invokes the Background Service / Worker for FileWatcher, then in turn the FileConversion
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            // Getting the environment variable to understand execution enviroment with OS platform. For e.g. : Windows Development, Linux Test, Linux Production etc.
            var environmentName = Environment.GetEnvironmentVariable("EXEC_ENV");
            var htmltoPdfWebkitPath = Environment.GetEnvironmentVariable("HTMLtoPdfWebkitPath");

            if (!string.IsNullOrEmpty(environmentName) && !string.IsNullOrEmpty(htmltoPdfWebkitPath))
            {
                
                // based on the environment + OS platform , settings file is loaded into the memory to parse the settings/configurations to a strongly typed object.
                var configurationbuilder = new ConfigurationBuilder()
                        .AddJsonFile($"appsettings.json", true, true)
                        .AddJsonFile($"appsettings.{environmentName}.json", true, true)
                        .AddEnvironmentVariables().Build();

                // Fetching Configuration values from setting file {appsetting.{environment_platform}.json}
                ConversionSettings.DeploymentPlatform = environmentName.ToLower().StartsWith("linux") ? Platform.Linux : Platform.Windows;
                ConversionSettings.BaseWatchPath = configurationbuilder.GetSection("ConversionSettings:BaseWatchPath").Value;
                ConversionSettings.FolderSearchPattern = configurationbuilder.GetSection("ConversionSettings:FolderSearchPattern").Value;
                ConversionSettings.SyncfusionLicense = configurationbuilder.GetSection("ConversionSettings:SyncfusionLicense").Value;
                ConversionSettings.HTMLtoPdfWebkitPath = htmltoPdfWebkitPath;

                int.TryParse(configurationbuilder.GetSection("ConversionSettings:FailureAttemptCount").Value, out int faitureattempt);

                ConversionSettings.FailureAttemptCount = faitureattempt;// Max. recovery attempts after a failure.

                int.TryParse(configurationbuilder.GetSection("ConversionSettings:WaitTimeInMilliSeconds").Value, out int waittimemilliseconds);
                ConversionSettings.WaitTimeInMilliSeconds = waittimemilliseconds; // Wait time between recovery attempts after a failure

                int.TryParse(configurationbuilder.GetSection("ConversionSettings:FileWatcherMonitoringDelayInMilliSeconds").Value, out int fileWatcherMonitoringDelayInMilliSeconds);
                ConversionSettings.FileWatcherMonitoringDelayInMilliSeconds = fileWatcherMonitoringDelayInMilliSeconds; // Delay between file directory fetch.

                int.TryParse(configurationbuilder.GetSection("ConversionSettings:DayCountBehindToStart").Value, out int count);
                ConversionSettings.DayCountBehindToStart = count; // days behind to start from for File watching, this is for DirectoryList Object to set up static algo for FileWacthing, till DB integration.

                // Fetching Syncfusion License from settings
                Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(ConversionSettings.SyncfusionLicense);

                CreateHostBuilder(args).Build().Run();


            }
            else
            {
                Console.WriteLine($"Missing Environment Variable(s) 'EXEC_ENV' or/both 'HTMLtoPdfWebkitPath', application requires these ENV VAR to starts with ");
            }


            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();

        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>(); // Dependency Injection of  Worker / BG Service


                });
    }
}
