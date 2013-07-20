using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSPreview.PreviewHandler.Controls.Calendar
{
    internal class ParseAttribute : Attribute
    {
        private string _beginPattern = string.Empty;
        private string _endPattern = string.Empty;
        private string _display = string.Empty;



        public string Begin
        {
            get { return _beginPattern; }
            set { _beginPattern = value; }
        }

        public string End
        {
            get { return _endPattern; }
            set { _endPattern = value; }
        }

        public string Display
        {
            get { return _display; }
            set { _display = value; }
        }

        public TypePropertyIcs Type { get; set; }

    }

    enum TypePropertyIcs
    {
        Line,
        Multiline
    }


}
