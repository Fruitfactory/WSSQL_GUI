///////////////////////////////////////////////////////////
//  FieldAttribute.cs
//  Implementation of the Class FieldAttribute
//  Generated by Enterprise Architect
//  Created on:      27-Sep-2013 1:36:24 AM
//  Original author: Yariki
///////////////////////////////////////////////////////////

namespace WSUI.Core.Core.Attributes
{
    public class FieldAttribute : BaseWSUIAttribute
    {
        public string FieldName { get; private set; }

        public int Index { get; private set; }

        public bool CanBeIgnored { get; private set; }

        public FieldAttribute(string fieldName, int index, bool canBeIngnored)
        {
            FieldName = fieldName;
            Index = index;
            CanBeIgnored = canBeIngnored;
        }
    }//end FieldAttribute
}//end namespace Attributes