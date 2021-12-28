﻿namespace MCS.FOI.MSGAttachmentsToPdf
{
    public interface IMSGFileProcessor
    {
        public (bool, string, string) MoveAttachments();
        public string MSGSourceFilePath { get; set; }

        public string OutputFilePath { get; set; }

        public string MSGFileName { get; set; }

        public bool IsSinglePDFOutput { get; set; }

        public int FailureAttemptCount { get; set; }

        public int WaitTimeinMilliSeconds { get; set; }
    }
}
