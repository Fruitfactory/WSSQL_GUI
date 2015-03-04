using System;
using Nest;

namespace WSUI.Core.Data.ElasticSearch
{
    [ElasticType(Name = "calendar")]
    public class WSUIAppointment : WSUIElasticSearchBaseEntity
    {
        public string Location { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string TimeZone { get; set; }

        public int Duration { get; set; }

        public int MeetingStatus { get; set; }

        public string AllAttendees { get; set; }

        public string ToAttendees { get; set; }

        public string CcAttendees { get; set; }

        public Boolean IsOnlineMeeting { get; set; }

        public string NetMeetingServer { get; set; }

        public string MeetingDocumentPath { get; set; }

        public string NetShowUrl { get; set; }

        public string RequiredAttendees { get; set; }


    }
}