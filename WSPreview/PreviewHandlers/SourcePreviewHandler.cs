using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using C4F.DevKit.PreviewHandler.PreviewHandlerFramework;
using C4F.DevKit.PreviewHandler.Controls;

namespace C4F.DevKit.PreviewHandler.PreviewHandlers
{
    [PreviewHandler("WSSQL Source Preview Handler", ".c;.cpp;.h;.hpp;.java;.bat;.asm;.html;.htm;.css;.cs;.vb;.sql;.ini;.config;.csproj;.sln;.js", "{61AE389A-C926-46A1-9170-CD1A348388B9}")]
    [ProgId("C4F.DevKit.PreviewHandler.PreviewHandlers.SourcePreviewHandler")]
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

            public override void Load(FileInfo file)
            {
                SourceViewer viewer = new SourceViewer();
                try
                {
                    viewer.LoadFile(file.FullName);
                    viewer.Dock = DockStyle.Fill;
                    Controls.Add(viewer);
                }
                catch (Exception)
                {
                    throw new Exception("Source Preview Error");
                }
            }
        }
    }
}
