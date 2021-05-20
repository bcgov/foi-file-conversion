﻿using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace MCS.FOI.FileConversion.Logger
{
    public static class CSVLogger
    {

        public static void CreateCSV(string logFilePath)
        {
            string fileName = $"{logFilePath}\\log.csv";
            if (!Directory.Exists(logFilePath))
                Directory.CreateDirectory(logFilePath);
            if (!File.Exists(fileName))
            {
                var file = File.Create(fileName);
                file.Close();
            }
            AddHeader(fileName);
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
                writer.Flush();
            }

        }

        public static void LogtoCSV(ConcurrentDictionary<string, (DateTime, string, DateTime?, string)> watcherLogger, string logFilePath)
        {
            string fileName = $"{logFilePath}\\log.csv";
            using (var writer = new StreamWriter(fileName))
            using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csvWriter.NextRecord();
                foreach (var logger in watcherLogger)
                {
                    csvWriter.WriteField(logger.Key);
                    csvWriter.WriteField(logger.Value.Item1);
                    csvWriter.WriteField(logger.Value.Item2);
                    csvWriter.WriteField(logger.Value.Item3);
                    csvWriter.WriteField(logger.Value.Item4);
                    csvWriter.NextRecord();
                }
                writer.Flush();                
            }
        }
    }
}
