﻿using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using WSPreview.PreviewHandler.PreviewHandlerFramework;
using WSPreview.PreviewHandler.Controls.Office;
using WSUI.Core.Enums;
using WSUI.Core.EventArguments;
using WSUI.Core.Interfaces;

namespace WSPreview.PreviewHandler.PreviewHandlers
{
    [PreviewHandler("WSSQL Office Preview Handler", ".msg", "{CE4CB591-6E33-4CA1-9E0C-BD6F774AFEB1}")]
    [PreviewForSearchObject(TypeSearchItem.Email)]
    [ProgId("WSPreview.PreviewHandler.PreviewHandlers.OutlookPreviewHandler")]
    [Guid("326A2452-981E-403B-9921-911011E677E6")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public sealed class OutlookPreviewHandler : FileBasedPreviewHandler, ISearchWordHighlight, ITranslateMessage,IOutlookFolder,ICommandPreviewControl
    {

        protected override PreviewHandlerControl CreatePreviewHandlerControl()
        {
            return new OutlookPreviewHandlerControl(this);
        }

        public sealed class OutlookPreviewHandlerControl : FileBasedPreviewHandlerControl, ITranslateMessage
        {
            private OutlookFilePreview _preview;
            private OutlookPreviewHandler _parent;
            public OutlookPreviewHandlerControl(OutlookPreviewHandler parent)
            {
                _parent = parent;
            }

            protected override Control GetCustomerPreviewControl()
            {
                _preview = new OutlookFilePreview();
                return _preview;
            }

            protected override Control GetPreviewControl()
            {
                Control ctrl = base.GetPreviewControl();
                if (ctrl is OutlookFilePreview)
                {
                    _preview = (OutlookFilePreview)ctrl;
                }
                return ctrl;
            }

            protected override ControlsKey GetControlsKey()
            {
                return ControlsKey.Outlook;
            }

            public void PassMessage(IWSAction action)
            {
                switch (action.Action)
                {
                    case WSActionType.Copy:
                        _preview.CopySelectedText();
                        break;
                }
            }

            protected override void PrepareForLoading()
            {
                base.PrepareForLoading();
                if (_preview != null)
                {
                    _preview.HitString = _parent.HitString;
                    _preview.FullFolderPath = _parent.FolderPath;
                    (_preview as ICommandPreviewControl).PreviewCommandExecuted -= OnPreviewCommandExecuted;
                    (_preview as ICommandPreviewControl).PreviewCommandExecuted += OnPreviewCommandExecuted;
                }
            }

            private void OnPreviewCommandExecuted(object sender, WSUIPreviewCommandArgs wsuiPreviewCommandArgs)
            {
               if(_parent == null)
                   return;
                _parent.RaisePreviewCommandExecuted(wsuiPreviewCommandArgs);
            }
        }

        public string HitString
        {
            get;
            set;
        }

        public void PassMessage(IWSAction action)
        {
            if (_previewControl == null)
                return;
            ((OutlookPreviewHandlerControl)_previewControl).PassMessage(action);
        }

        private void RaisePreviewCommandExecuted( WSUIPreviewCommandArgs args)
        {
            var temp = PreviewCommandExecuted;
            if (temp != null)
            {
                temp(this, args);
            }
        }

        public string FolderPath { get; set; }

        public event EventHandler<WSUIPreviewCommandArgs> PreviewCommandExecuted;
    }
}
