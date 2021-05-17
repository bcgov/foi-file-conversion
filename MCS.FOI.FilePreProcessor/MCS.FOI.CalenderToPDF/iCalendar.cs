using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MCS.FOI.CalenderToPDF
{   
    public class iCalendar
    {
        private string _dtStamp { get; set; }
        private string _dtStart { get; set; }
        private string _dtEnd { get; set; }
        public string Version { get; set; }
        public string ProdId { get; set; }
        public DateTime? Stamp { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Organizer { get; set; }
        public string Attendees { get; set; }
        public int Priority { get; set; }       

        private DateTime? ParseStringToDate(string value)
        {
            //Ensure value exists
            if (!String.IsNullOrEmpty(value))
            {
                var date = new DateTime();

                //This is a simple datetime parser. According to the iCal standard, a DateTime string ending in 'Z' indicates that the datetime is in UTC rather than local time. This code assumes the date is in UTC.
                if (DateTime.TryParseExact(value.ToLower().Replace("t", " ").Replace("z", ""), "yyyyMMdd HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out date))
                {
                    return date;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        public string DTStamp
        {
            get
            {
                return _dtStamp;
            }
            set
            {
                _dtStamp = value;
                Stamp = this.ParseStringToDate(value);
            }
        }
        public string DTStart
        {
            get
            {
                return _dtStart;
            }
            set
            {
                _dtStart = value;
                Start = this.ParseStringToDate(value);
            }
        }
        public string DTEnd
        {
            get
            {
                return _dtEnd;
            }
            set
            {
                _dtEnd = value;
                End = this.ParseStringToDate(value);
            }
        }
    }
}
