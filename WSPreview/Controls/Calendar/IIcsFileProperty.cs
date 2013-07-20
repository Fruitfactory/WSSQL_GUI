using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSPreview.PreviewHandler.Controls.Calendar
{
    interface IIcsFileProperty
    {
        DateTime DateStart { get; set; }
        DateTime DateEnd { get; set; }
        string Organizer { get; set; }
        string MailTo { get; set; }
        DateTime DateCreated { get; set; }
        string Description { get; set; }
        string Status { get; set; }
        string Summary { get; set; }
    }
}
