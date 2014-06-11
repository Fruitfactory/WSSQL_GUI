using System.Windows.Forms;
using WSUI.CA.Enums;

namespace WSUI.CA.ClosePromt
{
    public interface IClosePromptForm
    {
        eClosePrompt Result { get; set; }
    }
}