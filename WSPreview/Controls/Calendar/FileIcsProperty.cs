using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C4F.DevKit.PreviewHandler.Controls.Calendar
{
    class FileIcsProperty : IIcsFileProperty
    {
        #region Implementation of IIcsFileProperty

        [Parse(Begin = "^DTSTART:(?<start>.*)",Display = "Start date",Type = TypePropertyIcs.Line)]
        public DateTime DateStart { get; set; }

        [Parse(Begin = "^DTEND:(?<end>.*)",Display = "End date",Type = TypePropertyIcs.Line)]
        public DateTime DateEnd { get; set; }

        [Parse(Begin = "^ORGANIZER;CN=(?<org>.*):mailto:.*", Display = "Organizer", Type = TypePropertyIcs.Line)]
        public string Organizer { get; set; }

        [Parse(Begin = "^ORGANIZER;CN=.*:mailto:(?<mailto>.*)", Display = "Email to", Type = TypePropertyIcs.Line)]
        public string MailTo { get; set; }

        [Parse(Begin = "^CREATED:(?<created>.*)", Display = "Created", Type = TypePropertyIcs.Line)]
        public DateTime DateCreated { get; set; }

        [Parse(Begin = "^DESCRIPTION:", End = "^LAST-MODIFIED:.*", Display = "Description", Type = TypePropertyIcs.Multiline)]
        public string Description { get; set; }

        [Parse(Begin = "^STATUS:(?<status>.*)", Display = "Status", Type = TypePropertyIcs.Line)]
        public string Status { get; set; }

        [Parse(Begin = "^SUMMARY:(?<sum>.*)", Display = "Summary", Type = TypePropertyIcs.Line)]
        public string Summary { get; set; }

        #endregion
    }
}
