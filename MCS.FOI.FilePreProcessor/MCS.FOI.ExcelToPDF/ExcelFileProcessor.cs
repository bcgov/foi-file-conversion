using Syncfusion.Pdf;
using Syncfusion.XlsIO;
using Syncfusion.XlsIORenderer;
using System;
using System.IO;

namespace MCS.FOI.ExcelToPDF
{
    public class ExcelFileProcessor : IExcelFileProcessor
    {

        public ExcelFileProcessor()
        {

        }

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

        public bool ConvertToPDF()
        {
            bool converted = false;
            try
            {
                string sourceFile = Path.Combine(ExcelSourceFilePath, ExcelFileName);
                if (File.Exists(sourceFile))
                {
                    using (ExcelEngine excelEngine = new ExcelEngine())
                    {
                        IApplication application = excelEngine.Excel;
                        FileStream excelStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read);

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

                        excelStream.Dispose();
                        converted = true;
                    }
                }
                else
                {
                    return converted;
                }
            }
            catch (Exception ex)
            {
                converted = false;
                string error = $"Exception Occured while coverting file at {ExcelSourceFilePath} , exception :  {ex.Message} , stacktrace : {ex.StackTrace}";
                Console.WriteLine(error);
            }

            return converted;
        }

        private void saveToPdf(IWorksheet worksheet)
        {
            XlsIORenderer renderer = new XlsIORenderer();
            if (!Directory.Exists(PdfOutputFilePath))
                Directory.CreateDirectory(PdfOutputFilePath);
            string outputFileName = Path.Combine(PdfOutputFilePath, $"{Path.GetFileNameWithoutExtension(ExcelFileName)}_{worksheet.Name}");
            PdfDocument pdfDocument = renderer.ConvertToPDF(worksheet, new XlsIORendererSettings() { LayoutOptions = LayoutOptions.FitAllColumnsOnOnePage });
            Stream stream = new FileStream($"{outputFileName}.pdf", FileMode.Create, FileAccess.ReadWrite);
            pdfDocument.Compression = PdfCompressionLevel.Normal;
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
