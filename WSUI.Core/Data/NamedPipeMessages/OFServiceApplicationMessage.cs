using OF.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OF.Core.Data.NamedPipeMessages
{
    public class OFServiceApplicationMessage
    {

        public ofServiceApplicationMessageType MessageType { get; set; }


        public object Payload { get; set; }
    }
}
