using CsvHelper;
using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;

namespace MCS.FOI.FileConversion.Logger
{
    /// <summary>
    /// This class manages the logging of File convesion event to /LOGS folder based on the current time and status of conversion.
    /// </summary>
    public static class CSVLogger
    {

        public static string CreateCSV(string logFilePath)
        {
            //string fileName = $"{logFilePath}//log_{DateTime.Now:yyyyMMddHHmmssfff}.csv";
            string fileName = $"{logFilePath}//log.csv";
            if (!Directory.Exists(logFilePath))
                Directory.CreateDirectory(logFilePath);
            if (!File.Exists(fileName))
            {
                using var file = File.Create(fileName);
                file.Close();
                
            }           
            return fileName;
        }
        public static async void LogtoCSV(ConcurrentDictionary<string, (DateTime, string, DateTime?, string, string)> watcherLogger, string logFilePath)
        {
            try
            {
                string fileName = CreateCSV(logFilePath);
                using (var writer = new StreamWriter(fileName))
                using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {

                    csvWriter.WriteField("File Name");
                    csvWriter.WriteField("Created UTC");
                    csvWriter.WriteField("Status");
                    csvWriter.WriteField("Processed UTC");
                    csvWriter.WriteField("Comments");
                    csvWriter.WriteField("Ouput File Path");
                    await csvWriter.NextRecordAsync();
                    foreach (var logger in watcherLogger)
                    {
                        csvWriter.WriteField(logger.Key);
                        csvWriter.WriteField(logger.Value.Item1);
                        csvWriter.WriteField(logger.Value.Item2);
                        csvWriter.WriteField(logger.Value.Item3);
                        csvWriter.WriteField(logger.Value.Item4);
                        csvWriter.WriteField(logger.Value.Item5);
                        await csvWriter.NextRecordAsync();
                    }
                    writer.Flush();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured while writing to csv. {ex.Message}");
            }
        }
    }
}
