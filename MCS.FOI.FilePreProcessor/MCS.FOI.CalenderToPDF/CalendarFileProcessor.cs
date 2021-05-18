using Ical.Net;
using Syncfusion.HtmlConverter;
using Syncfusion.Pdf;
using System;
using System.IO;
using System.Text;

namespace MCS.FOI.CalenderToPDF
{
    public class CalendarFileProcessor : ICalendarFileProcessor
    {

        public string SourcePath { get; set; }

        public string DestinationPath { get; set; }

        public string FileName { get; set; }
        public CalendarFileProcessor()
        {

        }
        public CalendarFileProcessor(string sourcePath, string destinationPath, string fileName)
        {
            this.SourcePath = sourcePath;
            this.DestinationPath = destinationPath;
            this.FileName = fileName;

        }

        public bool ProcessCalendarFiles()
        {
            bool isProcessed;
            try
            {
                string htmlString = ConvertCalendartoHTML();
                isProcessed = ConvertHTMLtoPDF(htmlString);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return isProcessed;
        }
        private string ConvertCalendartoHTML()
        {
            FileStream fileStream = null;
            try
            {
                string ical = string.Empty;
                string sourceFile = Path.Combine(SourcePath, FileName);
                if (File.Exists(sourceFile))
                {
                    fileStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read);
                    using (StreamReader sr = new StreamReader(fileStream))
                    {
                        ical = sr.ReadToEnd();
                    }
                    Calendar calendar = Calendar.Load(ical);
                    var events = calendar.Events;
                    StringBuilder htmlString = new StringBuilder();
                    htmlString.Append(@"
                        <html>
                            <head>
                            </head>
                            <body style='border: 50px solid white;'>
                                
                                ");

                    int i = 1;
                    foreach (var e in events)
                    {
                        if (e.Attachments.Count > 0)
                        {
                            foreach (var attch in e.Attachments)
                            {
                                var file = attch.Parameters.Get("X-FILENAME");
                                string fileName = @$"{DestinationPath}/{file}";
                                CreateOutputFolder();
                                File.WriteAllBytes(fileName, attch.Data);
                            }
                        }
                        //Meeting Title
                        htmlString.AppendFormat(@"<div class='header" + i + "'><h1>" + e.Summary + "</h1><hr><table style='border: 5px; padding: 0;'>");

                        //Organizer Name and Email
                        htmlString.AppendFormat(@"<tr>
                        <td><b>From: </b></td>
                        <td>" + e.Organizer.CommonName + "(" + e.Organizer.Value.AbsoluteUri + ")" + "</td></tr>");

                        //Attendees name and Email
                        string attName = "";
                        foreach (var attendee in e.Attendees)
                        {
                            attName += ";" + attendee.CommonName + "(" + attendee.Value.AbsoluteUri + ")";
                        }
                        attName = attName.Substring(1);
                        htmlString.AppendFormat(@"<tr>
                        <td><b>To: </b></td>
                        <td>" + attName + "</td></tr>");

                        //Meeting created timestamp
                        htmlString.AppendFormat(@"<tr>
                        <td><b>Sent: </b></td>
                        <td>" + e.DtStamp.Date + "</td></tr>");

                        //Priority
                        htmlString.AppendFormat(@"<tr>
                        <td><b>Priority: </b></td>
                        <td>" + e.Priority + "</td></tr>");

                        //Meeting Start Timestamp
                        htmlString.AppendFormat(@"<tr>
                        <td><b>Start Time: </b></td>
                        <td>" + e.DtStart.Date + "</td></tr>");

                        //Meeting End Timestamp
                        htmlString.AppendFormat(@"<tr>
                        <td><b>End Time: </b></td>
                        <td>" + e.DtEnd.Date + "</td></tr>");
                        //Meeting Message
                        htmlString.AppendFormat(@"<tr>
                        <td><b>Description: </b></td>
                        </tr>
                        <tr><td></td><td>" + e.Description.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\n", "<br>") + "</td></tr>");
                        htmlString.Append(@"
                                </table>
                            </div><hr>");
                        i++;
                    }

                    htmlString.Append(@"
                            </body>
                        </html>");

                    return htmlString.ToString();
                }
                else
                {
                    string errorMessage = $"File not found at the {SourcePath}";
                    return errorMessage;
                }
            }
            catch (Exception ex)
            {
                string error = $"Exception Occured while coverting file at {SourcePath} to HTML , exception :  {ex.Message} , stacktrace : {ex.StackTrace}";
                Console.WriteLine(error);
                return error;
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Dispose();
            }
        }

        private bool ConvertHTMLtoPDF(string strHTML)
        {
            bool isConverted;
            FileStream fileStream = null;
            try
            {
                //Initialize HTML to PDF converter with Blink rendering engine
                HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter(HtmlRenderingEngine.WebKit);

                WebKitConverterSettings webKitConverterSettings = new WebKitConverterSettings() { EnableHyperLink = true };

                //TODO: The path needs to be replaced with @"/QtBinariesLinux"; when containerizing the code
                string path = @"" + Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\QtBinariesWindows";  
                webKitConverterSettings.WebKitPath = path; // $@"/QtBinariesLinux";

                //Assign WebKit converter settings to HTML converter
                htmlConverter.ConverterSettings = webKitConverterSettings;

                //Convert HTML string to PDF
                PdfDocument document = htmlConverter.Convert(strHTML, SourcePath);
                document.PageSettings.Size = PdfPageSize.Letter;
                document.PageSettings.Margins.All = 200;

                CreateOutputFolder();
                string outputPath = Path.Combine(DestinationPath, $"{Path.GetFileNameWithoutExtension(FileName)}.pdf");
                fileStream = new FileStream(outputPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);

                //Save and close the PDF document 
                document.Save(fileStream);
                document.Close(true);

                isConverted = true;
            }
            catch (Exception ex)
            {
                isConverted = false;
                string error = $"Exception Occured while coverting file at {SourcePath} to PDF , exception :  {ex.Message} , stacktrace : {ex.StackTrace}";
                Console.WriteLine(error);
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Dispose();
            }
            return isConverted;
        }

        private void CreateOutputFolder()
        {
            if (!Directory.Exists(DestinationPath))
                Directory.CreateDirectory(DestinationPath);
        }

    }
}
