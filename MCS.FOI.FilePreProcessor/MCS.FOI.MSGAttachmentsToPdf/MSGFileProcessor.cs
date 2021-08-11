using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MsgReader.Outlook;
namespace MCS.FOI.MSGAttachmentsToPdf
{
    public class MSGFileProcessor : IMSGFileProcessor
    {
        public string MSGSourceFilePath { get; set; }
        public string OutputFilePath { get; set; }
        public string MSGFileName { get; set; }
        public bool IsSinglePDFOutput { get; set; }
        public int FailureAttemptCount { get; set; }
        public int WaitTimeinMilliSeconds { get; set; }

        public (bool, string, string) MoveAttachments()
        {
            var message = $"No attachments to move to output folder";
            bool moved = true;
            string outputpath = string.Empty;
            try
            {
                string sourceFile = Path.Combine(MSGSourceFilePath, MSGFileName);
                Dictionary<string,Object> problematicFiles = null;
                using (var msg = new Storage.Message(sourceFile))
                {
                    var attachments = msg.Attachments;
                    foreach (Object attachment in attachments)
                    {

                        var type = attachment.GetType().FullName;
                     
                        if(type.ToLower().Contains("message"))
                        {
                            var file = (Storage.Message)attachment;
                            problematicFiles = problematicFiles == null ? new Dictionary<string, Object>() : problematicFiles;
                            problematicFiles.Add(file.FileName, file);
                                
                        }
                        else
                        {
                            var file = (Storage.Attachment)attachment;
                            if (file.FileName.ToLower().Contains(".xls") || file.FileName.ToLower().Contains(".ics") || file.FileName.ToLower().Contains(".msg"))
                            {
                                problematicFiles = problematicFiles == null ? new Dictionary<string, Object>() : problematicFiles;
                                problematicFiles.Add(file.FileName, file);

                            }

                        }

                    }

                    if (problematicFiles != null)
                    {
                        int cnt = 0;
                        foreach (var attachmenttomove in problematicFiles)
                        {
                            
                            if (attachmenttomove.Key.ToLower().Contains(".msg"))
                            {
                                var _attachment = (Storage.Message)attachmenttomove.Value;
                                string fileName = @$"{OutputFilePath}\msgattachments\{Path.GetFileNameWithoutExtension(MSGFileName)}_{_attachment.FileName}";
                                foreach (var subattachment in _attachment.Attachments)
                                {
                                    var type = subattachment.GetType().FullName;
                                    if (type.ToLower().Contains("attachment"))
                                    {
                                        var file = (Storage.Attachment)subattachment;
                                        CreateOutputFolder("subattachments");
                                        string _fileName = @$"{OutputFilePath}\msgattachments\subattachments\{Path.GetFileNameWithoutExtension(MSGFileName)}_{file.FileName}";
                                        File.WriteAllBytes(_fileName, file.Data);

                                    }

                                }
                                
                            }
                            else
                            {
                                var _attachment = (Storage.Attachment)attachmenttomove.Value;
                                string fileName = @$"{OutputFilePath}\msgattachments\{Path.GetFileNameWithoutExtension(MSGFileName)}_{_attachment.FileName}";
                                CreateOutputFolder();
                                File.WriteAllBytes(fileName, _attachment.Data);
                                outputpath += fileName;
                            }

                       
                            cnt++;
                           
                        }
                        moved = true;
                        message = string.Concat($"{cnt} problematic files moved", outputpath);
                    }
                   
                }
            }
            catch
            {
                message = $"Error happened while moving attachments on {MSGSourceFilePath}\\{MSGFileName}";
                moved = false;
            }

            return (moved, message, outputpath);
        }

        public MSGFileProcessor()
        {

        }

        private void CreateOutputFolder(string subpath = "")
        {
            string msgfilefolder = string.Concat(OutputFilePath, @"\msgattachments",!string.IsNullOrEmpty(subpath) ? @$"\{subpath}" : "");
            if (!Directory.Exists(msgfilefolder))
                Directory.CreateDirectory(msgfilefolder);
        }

        public MSGFileProcessor(string sourcePath, string destinationPath, string fileName)
        {
            this.MSGSourceFilePath = sourcePath;
            this.OutputFilePath = destinationPath;
            this.MSGFileName = fileName;

        }
    }
}
