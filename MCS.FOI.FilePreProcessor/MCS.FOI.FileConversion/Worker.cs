using MCS.FOI.ExcelToPDF;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MCS.FOI.FileConversion
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private IExcelFileProcessor ExcelFileProcessor;

        public Worker(ILogger<Worker> _logger, IExcelFileProcessor _excelFileProcessor)
        {
            logger = _logger;
            ExcelFileProcessor = _excelFileProcessor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
