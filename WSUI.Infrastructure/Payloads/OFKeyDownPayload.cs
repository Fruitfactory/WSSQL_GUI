using System.Windows.Forms;
using OF.Core.Core.Payload;

namespace OF.Infrastructure.Payloads
{
    public class OFKeyDownPayload : BasePayload<Keys>
    {
        public OFKeyDownPayload(Keys arg, bool control, bool shift, bool alt) : base(arg)
        {
            Control = control;
            Shift = shift;
            Alt = alt;
        }

        public bool Control { get; }

        public bool Shift { get; }

        public bool Alt { get; }


    }
}