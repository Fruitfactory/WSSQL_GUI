using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSUI.Infrastructure.Service.Enums
{
    [Flags]
    public enum TypeSearchItem
    {
        None = 0x00,
        Email = 0x01,
        Contact = 0x02,
        Attachment = 0x04,
        File = 0x08,
        Picture = 0x10,
        Calendar = 0x20,
        FileAll = File | Attachment | Picture 
    }
}
