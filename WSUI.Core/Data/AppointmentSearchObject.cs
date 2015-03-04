using System;
using System.Data.Common;
using WSUI.Core.Enums;

namespace WSUI.Core.Data
{
    public class AppointmentSearchObject : AbstractSearchObject
    {
        public AppointmentSearchObject()
        {
            TypeItem = TypeSearchItem.Calendar;
        }

        public string Location
        {
            get { return Get(() => Location); }
            set { Set(() => Location, value); }
        }

        public DateTime StartTime
        {
            get { return Get(() => StartTime); }
            set { Set(() => StartTime, value); }
        }

        public DateTime EndTime
        {
            get { return Get(() => EndTime); }
            set { Set(() => EndTime, value); }
        }

        public string TimeZone
        {
            get { return Get(() => TimeZone); }
            set { Set(() => TimeZone, value); }
        }

        public int Duration
        {
            get { return Get(() => Duration); }
            set { Set(() => Duration, value); }
        }

        public int MeetingStatus
        {
            get { return Get(() => MeetingStatus); }
            set { Set(() => MeetingStatus, value); }
        }

        public string AllAttendees
        {
            get { return Get(() => AllAttendees); }
            set { Set(() => AllAttendees, value); }
        }

        public string ToAttendees
        {
            get { return Get(() => ToAttendees); }
            set { Set(() => ToAttendees, value); }
        }

        public string CcAttendees
        {
            get { return Get(() => CcAttendees); }
            set { Set(() => CcAttendees, value); }
        }

        public bool IsOnlineMeeting
        {
            get { return Get(() => IsOnlineMeeting); }
            set { Set(() => IsOnlineMeeting, value); }
        }

        public string NetMeetingStatus
        {
            get { return Get(() => NetMeetingStatus); }
            set { Set(() => NetMeetingStatus, value); }
        }

        public string NetShowUrl
        {
            get { return Get(() => NetShowUrl); }
            set { Set(() => NetShowUrl, value); }
        }

        public string ReqiredAttendees
        {
            get { return Get(() => ReqiredAttendees); }
            set { Set(() => ReqiredAttendees, value); }
        }


    }
}