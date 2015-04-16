///////////////////////////////////////////////////////////
//  AttachmentSearchObject.cs
//  Implementation of the Class AttachmentSearchObject
//  Generated by Enterprise Architect
//  Created on:      27-Sep-2013 1:33:31 AM
//  Original author: Yariki
///////////////////////////////////////////////////////////

using System;
using System.Globalization;
using OF.Core.Core.Attributes;
using OF.Core.Enums;

namespace OF.Core.Data
{
    public class AttachmentSearchObject : BaseEmailSearchObject
    {
        public DateTime DateModified { get; set; }

        public AttachmentSearchObject()
        {
            TypeItem = TypeSearchItem.Attachment;
        }

    }//end AttachmentSearchObject
}//end namespace Data