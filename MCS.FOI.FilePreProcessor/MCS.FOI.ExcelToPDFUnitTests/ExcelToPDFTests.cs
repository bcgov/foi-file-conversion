
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCS.FOI.ExcelToPDF;
using System.IO;

namespace MCS.FOI.ExcelToPDFUnitTests
{
    [TestClass]
    public class ExcelToPDFTests
    {



        [TestMethod]
        public void XLSConvertToPDFTest()
        {

            ExcelFileProcessor excelFileProcessor = new ExcelFileProcessor();
            excelFileProcessor.ExcelFileName = "bc_fin_2021_supplement_estimates.xls";
            excelFileProcessor.ExcelSourceFilePath = getExcelRootFolder();
            excelFileProcessor.IsSinglePDFOutput = true;
            excelFileProcessor.PdfOutputFilePath = Path.Combine(getExcelRootFolder(), "output");
            bool isconverted = excelFileProcessor.ConvertToPDF();

            Assert.IsTrue(isconverted == true, $"Excel to PDF Conversion failed for {excelFileProcessor.ExcelFileName}");

            string outputfile = Path.Combine(getExcelRootFolder(), "output", $"{Path.GetFileNameWithoutExtension(excelFileProcessor.ExcelFileName)}.pdf");
            bool fileexists = File.Exists(outputfile);
            Assert.IsTrue(fileexists == true, $"Converted PDF file does not exists {excelFileProcessor.ExcelFileName}");

        }

        [TestMethod]
        public void XLSXConvertToPDFTest()
        {

            ExcelFileProcessor excelFileProcessor = new ExcelFileProcessor();
            excelFileProcessor.ExcelFileName = "capbudg.xlsx";
            excelFileProcessor.ExcelSourceFilePath = getExcelRootFolder();
            excelFileProcessor.IsSinglePDFOutput = true;
            excelFileProcessor.PdfOutputFilePath = Path.Combine(getExcelRootFolder(), "output");
            bool isconverted = excelFileProcessor.ConvertToPDF();

            Assert.IsTrue(isconverted == true, $"Excel to PDF Conversion failed for {excelFileProcessor.ExcelFileName}");

            string outputfile = Path.Combine(getExcelRootFolder(), "output", $"{Path.GetFileNameWithoutExtension(excelFileProcessor.ExcelFileName)}.pdf");
            bool fileexists = File.Exists(outputfile);
            Assert.IsTrue(fileexists == true, $"Converted PDF file does not exists {excelFileProcessor.ExcelFileName}");

        }


        [TestMethod]
        public void ProblematicXLSX1ConvertToPDFTest()
        {

            ExcelFileProcessor excelFileProcessor = new ExcelFileProcessor();
            excelFileProcessor.ExcelFileName = "IRIS Export - Masked.xlsx";
            excelFileProcessor.ExcelSourceFilePath = getExcelRootFolder();
            excelFileProcessor.IsSinglePDFOutput = true;
            excelFileProcessor.PdfOutputFilePath = Path.Combine(getExcelRootFolder(), "output");
            bool isconverted = excelFileProcessor.ConvertToPDF();

            Assert.IsTrue(isconverted == true, $"Excel to PDF Conversion failed for {excelFileProcessor.ExcelFileName}");

            string outputfile = Path.Combine(getExcelRootFolder(), "output", $"{Path.GetFileNameWithoutExtension(excelFileProcessor.ExcelFileName)}.pdf");
            bool fileexists = File.Exists(outputfile);
            Assert.IsTrue(fileexists == true, $"Converted PDF file does not exists {excelFileProcessor.ExcelFileName}");

        }

        [TestMethod]
        public void ProblematicXLSX2ConvertToPDFTest()
        {

            ExcelFileProcessor excelFileProcessor = new ExcelFileProcessor();
            excelFileProcessor.ExcelFileName = "Excel File 2.xlsx";
            excelFileProcessor.ExcelSourceFilePath = getExcelRootFolder();
            excelFileProcessor.IsSinglePDFOutput = false;
            excelFileProcessor.PdfOutputFilePath = Path.Combine(getExcelRootFolder(), "output");
            bool isconverted = excelFileProcessor.ConvertToPDF();

            Assert.IsTrue(isconverted == true, $"Excel to PDF Conversion failed for {excelFileProcessor.ExcelFileName}");
        }

        [TestMethod]
        public void FolderLevelSetupExcelToPdfTest()
        {
            string rootLocation = getExcelRootFolder();
            ExcelFileProcessor excelFileProcessor = new ExcelFileProcessor();
            excelFileProcessor.ExcelFileName = "capbudg.xlsx";
            excelFileProcessor.ExcelSourceFilePath = string.Concat(rootLocation, @"\Folder2\Folder21\Folder211\");
            excelFileProcessor.IsSinglePDFOutput = true;
            excelFileProcessor.PdfOutputFilePath = string.Concat(rootLocation, @"\output\", excelFileProcessor.ExcelSourceFilePath.Replace(rootLocation,""));
            bool isconverted = excelFileProcessor.ConvertToPDF();

            Assert.IsTrue(isconverted == true, $"Excel to PDF Conversion failed for {excelFileProcessor.ExcelFileName}");

            string outputfile = Path.Combine(excelFileProcessor.PdfOutputFilePath, $"{Path.GetFileNameWithoutExtension(excelFileProcessor.ExcelFileName)}.pdf");
            bool fileexists = File.Exists(outputfile);
            Assert.IsTrue(fileexists == true, $"Converted PDF file does not exists {excelFileProcessor.ExcelFileName}");
        }


        private string getExcelRootFolder()
        {
            string unittestexecutionDirectory = Directory.GetCurrentDirectory();
            string approot = unittestexecutionDirectory.Replace(@"\bin\Debug\netcoreapp3.1", "");
            return Path.Combine(approot, "SourceExcel");

        }
    }
}
