using Syncfusion.Pdf;
using Syncfusion.XlsIO;
using Syncfusion.XlsIORenderer;
using System;
using System.IO;
using System.Threading;

namespace MCS.FOI.ExcelToPDF
{
    public class ExcelFileProcessor : IExcelFileProcessor
    {

        public ExcelFileProcessor() { }

        public ExcelFileProcessor(string sourceExcelFilePath, string outputPdfFilePath, string excelFileName)
        {
            this.ExcelSourceFilePath = sourceExcelFilePath;
            this.PdfOutputFilePath = outputPdfFilePath;
            this.ExcelFileName = excelFileName;
        }

        public string ExcelSourceFilePath { get; set; }

        public string PdfOutputFilePath { get; set; }

        public string ExcelFileName { get; set; }

        public bool IsSinglePDFOutput { get; set; }

        private static object lockObject = new object();
        public (bool, string, string) ConvertToPDF()
        {
            bool converted = false;
            string message = string.Empty;
            try
            {                
                string sourceFile = Path.Combine(ExcelSourceFilePath, ExcelFileName);
                if (File.Exists(sourceFile))
                {
                    using (ExcelEngine excelEngine = new ExcelEngine())
                    {
                        IApplication application = excelEngine.Excel;
                       
                        for (int attempt = 1; attempt < 5; attempt++)
                        {
                            FileStream excelStream;
                            Thread.Sleep(5000);
                            try
                            {
                                using (excelStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read))
                                {
                                    IWorkbook workbook = application.Workbooks.Open(excelStream, ExcelParseOptions.DoNotParsePivotTable);

                                    if (workbook.Worksheets.Count > 0)
                                    {
                                        if (!IsSinglePDFOutput)
                                        {
                                            foreach (IWorksheet worksheet in workbook.Worksheets)
                                            {
                                                if (worksheet.Visibility == WorksheetVisibility.Visible)
                                                    saveToPdf(worksheet);
                                            }
                                        }
                                        else
                                        {
                                            saveToPdf(workbook);

                                        }
                                    }

                                    converted = true;
                                    message = $"{sourceFile} processed successfully!";
                                    break;
                                }
                            }
                            catch(Exception e)
                            {
                                message = $"Exception happened while accessing File {sourceFile}, re-attempting count : {attempt}";
                                Console.WriteLine(message);
                                excelStream = null;

                            }
                        }

                    }
                }
                else
                {
                    message = $"{sourceFile} does not exist!";
                    //return converted;
                }
            }
            catch (Exception ex)
            {
                converted = false;
                string error = $"Exception occured while coverting file at {ExcelSourceFilePath} , exception :  {ex.Message} , stacktrace : {ex.StackTrace}";
                Console.WriteLine(error);
            }

            return (converted, message, PdfOutputFilePath);
        }

        private void saveToPdf(IWorksheet worksheet)
        {
            XlsIORenderer renderer = new XlsIORenderer();
            if (!Directory.Exists(PdfOutputFilePath))
                Directory.CreateDirectory(PdfOutputFilePath);
            string outputFileName = Path.Combine(PdfOutputFilePath, $"{Path.GetFileNameWithoutExtension(ExcelFileName)}_{worksheet.Name}");
            using var pdfDocument = renderer.ConvertToPDF(worksheet, new XlsIORendererSettings() { LayoutOptions = LayoutOptions.FitAllColumnsOnOnePage });
            using var stream = new FileStream($"{outputFileName}.pdf", FileMode.Create, FileAccess.ReadWrite);
            pdfDocument.Compression = PdfCompressionLevel.BestSpeed;
            pdfDocument.Save(stream);
            stream.Dispose();

        }

        private void saveToPdf(IWorkbook workbook)
        {
            XlsIORenderer renderer = new XlsIORenderer();
            if (!Directory.Exists(PdfOutputFilePath))
                Directory.CreateDirectory(PdfOutputFilePath);
            string outputFileName = Path.Combine(PdfOutputFilePath, $"{Path.GetFileNameWithoutExtension(ExcelFileName)}.pdf");
            PdfDocument pdfDocument = renderer.ConvertToPDF(workbook, new XlsIORendererSettings() { LayoutOptions = LayoutOptions.FitAllColumnsOnOnePage });
            Stream stream = new FileStream(outputFileName, FileMode.Create, FileAccess.ReadWrite);
            pdfDocument.Save(stream);
            stream.Dispose();

        }



    }
}
