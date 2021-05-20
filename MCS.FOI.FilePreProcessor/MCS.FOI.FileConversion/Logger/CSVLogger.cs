using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace MCS.FOI.FileConversion.Logger
{
    public static class CSVLogger
    {

        public static string CreateCSV(string logFilePath)
        {
            string fileName = $"{logFilePath}\\log_{DateTime.Now:yyyyMMddHHmmssfff}.csv";
            if (!Directory.Exists(logFilePath))
                Directory.CreateDirectory(logFilePath);
            if (!File.Exists(fileName))
            {
                var file = File.Create(fileName);
                file.Close();
                //string csvHeader = $"\"File Name\",\"Created UTC\",\"Status\",\"Processed UTC\",\"Comments\"{Environment.NewLine}";
                //File.WriteAllText(fileName, csvHeader);
            }
            //AddHeader(fileName);
            return fileName;
        }

        public static void AddHeader(string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csvWriter.WriteField("File Name");
                csvWriter.WriteField("Created UTC");
                csvWriter.WriteField("Status");
                csvWriter.WriteField("Processed UTC");
                csvWriter.WriteField("Comments");
                csvWriter.WriteField("Ouput File Path");
                writer.Flush();
            }

        }

        public static async void LogtoCSV(ConcurrentDictionary<string, (DateTime, string, DateTime?, string, string)> watcherLogger, string logFilePath)
        {
            try
            {
                string fileName = CreateCSV(logFilePath);
                //string fileName = $"{logFilePath}\\log.csv";

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


        public static async void UpdateRecords(string logFilePath, ConcurrentDictionary<string, (DateTime, string, DateTime?, string)> watcherLogger)
        {
            string fileName = $"{logFilePath}\\log.csv";
            try
            {
                string[] lines = await File.ReadAllLinesAsync(fileName);

                List<string> updatedLines = new List<string>(lines);
                foreach (var log in watcherLogger)
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (lines[i].Contains(log.Key))
                        {
                            updatedLines.RemoveAt(i);
                        }
                    }
                    updatedLines.Add($"{log.Key},{log.Value.Item1},{log.Value.Item2},{log.Value.Item3},{log.Value.Item4}");
                }

                for (int attempt = 1; attempt < 5; attempt++)
                {
                    Thread.Sleep(5000);
                    try
                    {
                        using (var fileWriter = new StreamWriter(fileName))
                        {
                            foreach (var item in updatedLines)
                            {
                                if (!string.IsNullOrEmpty(item))
                                {
                                    await fileWriter.WriteAsync(item);
                                    await fileWriter.WriteAsync(Environment.NewLine);
                                }
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error occured while writing to CSV, re-attempting count : {attempt}");

                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured while writing to CSV: {ex.Message}");

            }
        }
    }
}
