using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using OFPreview.PreviewHandler.PreviewHandlerFramework;
using OFPreview.PreviewHandler.Controls;

namespace OFPreview.PreviewHandler.PreviewHandlers
{
    [PreviewHandler("WSSQL Source Preview Handler", ".txt;.log;.c;.cpp;.h;.hpp;.java;.bat;.asm;.css;.cs;.vb;.sql;.ini;.config;.csproj;.sln;.js", "{61AE389A-C926-46A1-9170-CD1A348388B9}")]
    [ProgId("OFPreview.PreviewHandler.PreviewHandlers.SourcePreviewHandler")]
    [Guid("28E58D5E-2FDB-4D8F-92CF-1DF9519DF0FA")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public sealed class SourcePreviewHandler :FileBasedPreviewHandler
    {

        protected override PreviewHandlerControl CreatePreviewHandlerControl()
        {
            return new SourcePreviewHandlerControl();
        }

        private sealed class SourcePreviewHandlerControl : FileBasedPreviewHandlerControl
        {

            protected override Control GetCustomerPreviewControl()
            {
                SourceViewer viewer = new SourceViewer();
                return viewer;
            }

            protected override ControlsKey GetControlsKey()
            {
                return ControlsKey.Source;
            }
        }
    }
}
