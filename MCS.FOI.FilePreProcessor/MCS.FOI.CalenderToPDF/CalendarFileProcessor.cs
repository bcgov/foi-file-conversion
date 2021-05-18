using Ical.Net;
using Syncfusion.HtmlConverter;
using Syncfusion.Pdf;
using System;
using System.IO;
using System.Text;

namespace MCS.FOI.CalenderToPDF
{
    public class CalendarFileProcessor
    {
        public string BasePath { get; set; }
        public string SourcePath { get; set; }
        public string DestinationPath { get; set; }
        public string FileName { get; set; }

        public CalendarFileProcessor(string basePath, string sourcePath, string destinationPath)
        {
            BasePath = basePath;
            SourcePath = sourcePath;
            DestinationPath = destinationPath;
        }       

        public void ProcessCalendarFiles()
        {
            //BasePath = basePath;
            DirectoryInfo dirInput = new DirectoryInfo(BasePath);
            //DestinationPath = destinationPath;
            //SourcePath = sourcePath;
            try
            {
                //ProcessDirectories(dirInput, DestinationPath);
                FileName = Path.GetFileNameWithoutExtension(SourcePath);
                string htmlString = ReadFIle();
                PDFConverter(htmlString);

            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }
        public string ReadFIle()
        {
            FileStream fs = null;
            try
            {
                //var sharedpath = @"/app/input";
                //var sharedpath = sourcePath; // @"\\DESKTOP-9L1FGS3\SharedLAN\Req1";
                // FileStream fs = new FileStream(@$"{sharedpath}/iCalendar.ics", FileMode.Open, FileAccess.Read);
                string ical = string.Empty;
                fs = new FileStream(SourcePath, FileMode.Open, FileAccess.Read);
                using (StreamReader sr = new StreamReader(fs))
                {
                    ical = sr.ReadToEnd();
                }
                Calendar calendar = Calendar.Load(ical);
                iCalendar cal = new iCalendar();
                var events = calendar.Events;
                StringBuilder sb = new StringBuilder();
                sb.Append(@"
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
                            File.WriteAllBytes(fileName, attch.Data);
                        }
                    }
                    sb.AppendFormat(@"<div class='header" + i + "'><h1>" + e.Summary + "</h1><hr><table style='border: 5px; padding: 0;'>");
                    cal.Summary = e.Summary;
                    sb.AppendFormat(@"<tr>
                        <td><b>From: </b></td>
                        <td>" + e.Organizer.CommonName + "(" + e.Organizer.Value.AbsoluteUri + ")" + "</td></tr>");
                    string name = e.Organizer.CommonName;
                    cal.Organizer = e.Organizer.CommonName + "(" + e.Organizer.Value.AbsoluteUri + ")";
                    string attName = "";
                    foreach (var attendee in e.Attendees)
                    {
                        attName += ";" + attendee.CommonName + "(" + attendee.Value.AbsoluteUri + ")";
                    }
                    attName = attName.Substring(1);
                    sb.AppendFormat(@"<tr>
                        <td><b>To: </b></td>
                        <td>" + attName + "</td></tr>");
                    cal.Attendees = attName;
                    sb.AppendFormat(@"<tr>
                        <td><b>Sent: </b></td>
                        <td>" + e.DtStamp.Date + "</td></tr>");
                    cal.Stamp = e.DtStamp.Date;
                    sb.AppendFormat(@"<tr>
                        <td><b>Priority: </b></td>
                        <td>" + e.Priority + "</td></tr>");
                    cal.Priority = e.Priority;
                    sb.AppendFormat(@"<tr>
                        <td><b>Start Time: </b></td>
                        <td>" + e.DtStart.Date + "</td></tr>");
                    cal.Start = e.DtStart.Date;
                    sb.AppendFormat(@"<tr>
                        <td><b>End Time: </b></td>
                        <td>" + e.DtEnd.Date + "</td></tr>");
                    cal.End = e.DtEnd.Date;
                    sb.AppendFormat(@"<tr>
                        <td><b>Description: </b></td>
                        </tr>
                        <tr><td></td><td>" + e.Description.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\n", "<br>") + "</td></tr>");
                    cal.Description = e.Description;
                    sb.Append(@"
                                </table>
                            </div><hr>");
                    i++;
                }

                sb.Append(@"
                            </body>
                        </html>");
                string htmlString = sb.ToString();

                return sb.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                if (fs != null)
                    fs.Dispose();
            }
        }

        private void PDFConverter(string strHTML)
        {
            string baseUrl = SourcePath; //@"/app/SharedLAN/Req1"; //@"\\DESKTOP-9L1FGS3\SharedLAN\Req1";

            //Initialize HTML to PDF converter with Blink rendering engine
            HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter(HtmlRenderingEngine.WebKit);

            WebKitConverterSettings webKitConverterSettings = new WebKitConverterSettings() { EnableHyperLink = true };
            //webKitConverterSettings.WebKitPath = $@"/app/QtBinariesLinux"; 
            string path = @"" + Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\QtBinariesWindows";
            webKitConverterSettings.WebKitPath = path; // $@"/QtBinariesLinux";

            //Assign WebKit converter settings to HTML converter
            htmlConverter.ConverterSettings = webKitConverterSettings;

            //Convert HTML string to PDF
            PdfDocument document = htmlConverter.Convert(strHTML, baseUrl);
            document.PageSettings.Size = PdfPageSize.Letter;
            document.PageSettings.Margins.All = 200;

            string outputPath = @$"{DestinationPath}/{FileName}.pdf";
            FileStream fileStream = new FileStream(outputPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);

            //Save and close the PDF document 
            document.Save(fileStream);
            document.Close(true);

        }

        ////private void ProcessDirectories(DirectoryInfo dirInput, string dirOutput)
        ////{
        ////    string dirOutputfix = String.Empty;
        ////    bool isCalendar = false;
        ////    foreach (DirectoryInfo di in dirInput.GetDirectories())
        ////    {
        ////        if (di.Name != "Output")
        ////        {
        ////            dirOutputfix = dirOutput + "/" + di.Name;
        ////            try
        ////            {
        ////                foreach (var file in di.GetFiles())
        ////                {
        ////                    if (file.Extension == ".ics")
        ////                    {
        ////                        isCalendar = true;
        ////                        if (!Directory.Exists(dirOutputfix))
        ////                            Directory.CreateDirectory(dirOutputfix);
        ////                        //string htmlString = ReadFIle(file.FullName, dirOutputfix);
        ////                        string fileName = Path.GetFileNameWithoutExtension(file.FullName);
        ////                        //Converter(htmlString, fileName, dirOutputfix);
        ////                    }
        ////                }
        ////                if (isCalendar)
        ////                {
        ////                    if (!Directory.Exists(dirOutputfix))
        ////                        Directory.CreateDirectory(dirOutputfix);
        ////                    isCalendar = false;
        ////                }
        ////            }
        ////            catch (Exception e)
        ////            {
        ////                throw (e);
        ////            }
        ////        }

        ////        ProcessDirectories(di, dirOutputfix);


        ////    }
        ////}
    }
}
