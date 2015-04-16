﻿using OF.Core.Core.Attributes;
using OF.Core.Enums;

namespace OF.Core.Data
{
    public class EmailContactSearchObject : BaseEmailSearchObject
    {

        public EmailContactSearchObject()
        {
            TypeItem = TypeSearchItem.Contact;
            Tag = "Click to email recipient";
        }

        public string EMail { get; set; }

        public string ContactName { get; set; }

        public string AddressType { get; set; }

    }
}