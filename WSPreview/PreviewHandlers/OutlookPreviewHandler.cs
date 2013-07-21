﻿using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Globalization;
using WSPreview.PreviewHandler;
using WSPreview.PreviewHandler.PreviewHandlerFramework;
using WSPreview.PreviewHandler.Controls.Office;
using WSPreview.PreviewHandler.Service;

namespace WSPreview.PreviewHandler.PreviewHandlers
{
    [PreviewHandler("WSSQL Office Preview Handler", ".msg", "{CE4CB591-6E33-4CA1-9E0C-BD6F774AFEB1}")]
    [ProgId("WSPreview.PreviewHandler.PreviewHandlers.OutlookPreviewHandler")]
    [Guid("326A2452-981E-403B-9921-911011E677E6")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public sealed class OutlookPreviewHandler : FileBasedPreviewHandler,ISearchWordHighlight,ITranslateMessage
    {

        protected override PreviewHandlerControl CreatePreviewHandlerControl()
        {
            return new OutlookPreviewHandlerControl(this);
        }

        public sealed class OutlookPreviewHandlerControl : FileBasedPreviewHandlerControl,ITranslateMessage
        {
            private OutlookFilePreview _preview;
            private OutlookPreviewHandler _parent;
            public OutlookPreviewHandlerControl(OutlookPreviewHandler parent)
            {
                _parent = parent;
            }

            public override void Load(FileInfo file)
            {
                if (_preview != null)
                {
                    _preview.HitString = _parent.HitString;
                }
                base.Load(file);
            }

            protected override Control GetCustomerPreviewControl()
            {
                _preview = new OutlookFilePreview();
                return _preview;
            }

            protected override Control GetPreviewControl()
            {
                Control ctrl = base.GetPreviewControl();
                if(ctrl is OutlookFilePreview)
                {
                    _preview = (OutlookFilePreview) ctrl;
                }
                return ctrl;
            }

            protected override ControlsKey GetControlsKey()
            {
                return ControlsKey.Outlook;
            }

            public void PassMessage(WSActionType action)
            {
                switch (action)
                {
                    case WSActionType.Copy:
                        _preview.CopySelectedText();
                        break;
                }
            }
        }

        public string HitString
        {
            get; set;
        }

        public void PassMessage(WSActionType action)
        {
            if(_previewControl == null)
                return;
            ((OutlookPreviewHandlerControl)_previewControl).PassMessage(action);
        }
    }
}
