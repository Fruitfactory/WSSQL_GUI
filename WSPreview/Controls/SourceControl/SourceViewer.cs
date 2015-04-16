using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Microsoft.Office.Interop.Outlook;
using OFPreview.PreviewHandler.PreviewHandlerFramework;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using OF.Core.Data;

namespace OFPreview.PreviewHandler.Controls
{
    [KeyControl(ControlsKey.Source)]
    public partial class SourceViewer : UserControl,IPreviewControl
    {
#region fields

        private TextEditor _textEditor;
        private ElementHost _hosting;


#endregion

        public SourceViewer()
        {
            InitializeComponent();
            _textEditor = new TextEditor();
            _hosting = new ElementHost();
            _hosting.Dock = DockStyle.Fill;
            _hosting.Child = _textEditor;
            Controls.Add(_hosting);
        }

        #region public

        public void LoadFile(string filename)
        {
            if (_textEditor == null || string.IsNullOrEmpty(filename))
                return;
            _textEditor.Load(filename);
            _textEditor.SyntaxHighlighting =
                HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(filename));
        }

        public void LoadFile(Stream stream)
        {
        }

        public void LoadObject(BaseSearchObject obj)
        {
            
        }

        public void Clear()
        {
            if (_textEditor == null)
                return;
            _textEditor.Clear();
        }

        #endregion

    }
}
