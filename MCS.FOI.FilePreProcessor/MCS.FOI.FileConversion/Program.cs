using MCS.FOI.FileConversion.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace MCS.FOI.FileConversion
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var environmentName = Environment.GetEnvironmentVariable("EXEC_ENV");

            if (!string.IsNullOrEmpty(environmentName))
            {
                var configurationbuilder = new ConfigurationBuilder()
                        .AddJsonFile($"appsettings.json", true, true)
                        .AddJsonFile($"appsettings.{environmentName}.json", true, true)
                        .AddEnvironmentVariables().Build();

                ConversionSettings.DeploymentPlatform = environmentName.ToLower().StartsWith("linux") ? Platform.Linux : Platform.Windows;
                ConversionSettings.BaseWatchPath = configurationbuilder.GetSection("ConversionSettings:BaseWatchPath").Value;
                ConversionSettings.FolderSearchPattern = configurationbuilder.GetSection("ConversionSettings:FolderSearchPath").Value;
                ConversionSettings.FailureAttemptCount = configurationbuilder.GetSection("ConversionSettings:FailureAttemptCount").Value;
                ConversionSettings.WaitTimeInMilliSeconds = configurationbuilder.GetSection("ConversionSettings:WaitTimeInMilliSeconds").Value;
                int.TryParse(configurationbuilder.GetSection("ConversionSettings:DayCountBehindToStart").Value, out int count);
                ConversionSettings.DayCountBehindToStart = count;

                CreateHostBuilder(args).Build().Run();


            }
            else
            {
                Console.WriteLine($"Missing Environment Variable 'EXEC_ENV', application requires this ENV VAR to starts with ");
            }


            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();

        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();


                });
    }
}
