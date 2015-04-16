///////////////////////////////////////////////////////////
//  AdressSearchObject.cs
//  Implementation of the Class AdressSearchObject
//  Generated by Enterprise Architect
//  Created on:      27-Sep-2013 1:33:32 AM
//  Original author: Yariki
///////////////////////////////////////////////////////////

using OF.Core.Core.Attributes;

namespace OF.Core.Data
{
    public class AdressSearchObject : BaseSearchObject
    {
        public string ToAddress { get; set; }
        public string FromAddress { get; set; }
        public string CcAddress { get; set; }

        public AdressSearchObject()
        {
        }

    }//end AdressSearchObject
}//end namespace Data