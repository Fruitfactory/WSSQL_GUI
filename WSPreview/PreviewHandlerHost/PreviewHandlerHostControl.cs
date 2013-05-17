using System;
using System.IO;
using System.Reflection;
using System.ComponentModel;
using System.Drawing;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using C4F.DevKit.PreviewHandler.PInvoke;
using C4F.DevKit.PreviewHandler.PreviewHandlerFramework;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices;
using C4F.DevKit.PreviewHandler.Service;
using C4F.DevKit.PreviewHandler.Service.Logger;

namespace C4F.DevKit.PreviewHandler.PreviewHandlerHost
{
    /// <summary>
    /// This control is dependent on the managed framework for preview handlers implemented by Stephen Toub
    /// and published in the December 2006 issue of MSDN Magazine.  http://msdn.microsoft.com/msdnmag/issues/07/01/PreviewHandlers/default.aspx
    /// In this article, he implements a managed wrapper to the COM Preview Handler interfaces IPreviewHandler, IInitializeWithFile and IInitializeWithStream
    /// 
    /// In this class, we look up the registered preview handler for a given file extension, using reflection, instantiate an instance of the handler.
    /// We then check if the handler is a Stream or File Handler by checking which interface is implemented.  We then initialize the handler, pass a handle to 
    /// our control and the bounds of our control, and call DoPreview (part of the IPreviewHandler interface)
    /// 
    /// Developed by Ryan Powers - Clarity Consulting - http://www.claritycon.com
    /// </summary>
    [ToolboxItem(true), ToolboxBitmap(typeof(PreviewHandlerHostControl))]
    public partial class PreviewHandlerHostControl : UserControl
    {

        private string _filePath;
        private string _searchCriteria;
        private object _comInstance = null;
        private bool _registreHandler = true;


        public event EventHandler StartLoad;

        private void OnStartLoad(EventArgs e)
        {
            EventHandler handler = StartLoad;
            if (handler != null) handler(this, e);
        }

        public event EventHandler StopLoad;

        private void OnStopLoad(EventArgs e)
        {
            EventHandler handler = StopLoad;
            if (handler != null) handler(this, e);
        }

        public PreviewHandlerHostControl()
        {
            InitializeComponent();
        }

        public string SearchCriteria
        {
            get { return _searchCriteria; }
            set { _searchCriteria = value; }
        }

        /// <summary>
        /// Full path to file to be previewed
        /// 
        /// Whenever a new path is set, the preview is generated
        /// </summary>
        public string FilePath
        {
            get { return _filePath; }
            set { 
                _filePath = value;
                if (value != null && !IsDesignTime())
                {
                    OnStartLoad(null);
                    GeneratePreview();
                    OnStopLoad(null);
                }
            }
        }

        public void UnloadPreview()
        {
            if (_comInstance != null)
            {
                ((IPreviewHandler)_comInstance).Unload();
                _comInstance = null;
            }
        }

        public void PassAction(WSActionType actionType)
        {
            
        }

        private bool IsDesignTime()
        {
            return (this.Site != null && this.Site.DesignMode);
        }

        /// <summary>
        /// 1) Look up the preview handler associated with the file extension
        /// 2) Create an instance of the handler using its CLSID and reflection
        /// 3) Check if it is a file or stream handler
        /// 4) Initialize with File or Stream using Initialize from the appropriate interface
        /// 5) Call SetWindow passing in a handle to this control and the bounds of the control
        /// 6) Call DoPreview
        /// </summary>
        private void GeneratePreview()
        {
          
            webMessage.Visible = false;
            if (_comInstance != null)
            {
                ((IPreviewHandler)_comInstance).Unload();
                _comInstance = null;
            }
            _registreHandler = true;
            RECT r;
            r.top = 0;
            r.bottom = this.Height;
            r.left = 0;
            r.right = this.Width;
            
            //check registry
            RegistrationData data = PreviewHandlerRegistryAccessor.LoadRegistrationInformation();
            PreviewHandlerInfo handler = null;
            Type comType = null;
            foreach (ExtensionInfo ei in data.Extensions)
            {
                if (_filePath.ToUpper().EndsWith(ei.Extension.ToUpper()))
                {
                    handler = ei.Handler;
                    if (handler != null)
                        break;
                }
            }
            // check application handlers
            if (_filePath.ToLower().EndsWith(".msg") || handler == null )//
            {
                string ext = Path.GetExtension(_filePath).ToLower();
                if(HelperPreviewHandlers.HandlersDictionary.ContainsKey(ext))
                    comType = HelperPreviewHandlers.HandlersDictionary[ext];
                if (comType == null)
                {
                    webMessage.Visible = true;
                    var str = Regex.Replace(Properties.Resources.NoPreview, "replace", ext);
                    webMessage.DocumentText = str;
                    WSSqlLogger.Instance.LogWarning(string.Format("{0}: {1}", "No Preview", _filePath));
                    return;
                }
                _registreHandler = false;
            } 
            else
                comType = Type.GetTypeFromCLSID(new Guid(handler.ID));

            try
            {
                // Create an instance of the preview handler
                _comInstance = Activator.CreateInstance(comType);

                // Check if it is a stream or file handler
                if (_comInstance is IInitializeWithFile)
                {
                    if (_comInstance is ISearchWordHighlight)
                        ((ISearchWordHighlight) _comInstance).HitString = SearchCriteria;
                    ((IInitializeWithFile)_comInstance).Initialize(_filePath, 0);
                }
                else if (File.Exists(_filePath))
                {
                    if (_comInstance is IInitializeWithStream)
                    {
                        StreamWrapper stream = new StreamWrapper(File.Open(_filePath, FileMode.Open));
                        ((C4F.DevKit.PreviewHandler.PreviewHandlerFramework.IInitializeWithStream)_comInstance).Initialize(stream, 0);
                    }
                    else if (_comInstance is IInitializeWithItem)
                    {
                        IShellItem shellItem = null;
                        UnsafeNativeMethods.SHCreateItemFromParsingName(_filePath, IntPtr.Zero, typeof(IShellItem).GUID, out shellItem);
                        if (shellItem != null)
                        {
                            ((IInitializeWithItem)_comInstance).Initialize(shellItem, 2);
                        }
                    }
                }
                else
                {
                    WSSqlLogger.Instance.LogError(string.Format("{0}: {1}", "File Not Found", _filePath));
                    throw new FileNotFoundException(_filePath);
                }


                ((IPreviewHandler)_comInstance).SetWindow(this.Handle, ref r);
                ((IPreviewHandler)_comInstance).DoPreview();
            }
            catch(Exception ex)
            {
                _comInstance = null;
                webMessage.Visible = true;
                webMessage.DocumentText = Regex.Replace(Properties.Resources.Error1,"replace",ex.Message);
                WSSqlLogger.Instance.LogError(string.Format("{0}: {1}", "Error", ex.Message));
            }

        }


        private void Control_Resize(object sender, EventArgs e)
        {
            if(_comInstance != null)
            {
                RECT r;
                r.top = 0;
                r.bottom = this.Height;
                r.left = 0;
                r.right = this.Width;
                ((IPreviewHandler)_comInstance).SetRect(ref r);
            }
            this.Invalidate();
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WindowAPI.WM_DESTROY:
                    if (_comInstance != null)
                    {
                        ((IPreviewHandler)_comInstance).Unload();
                        _comInstance = null;
                    }
                    break;
                case WindowAPI.WM_SIZE:
                    if(_registreHandler)
                        WindowAPI.EnumChildWindows(this.Handle, SendSizeMessage, m.LParam);
                    break;
                case WindowAPI.WM_COPY:
                case WindowAPI.WM_KEYDOWN:
                    if (_comInstance is ITranslateMessage)
                    {
                        ((ITranslateMessage)_comInstance).PassMessage(m);
                    }
                    break;
            }
            base.WndProc(ref m);
        }


        private bool SendSizeMessage(IntPtr hWnd, IntPtr lParam)
        {
            if (hWnd != IntPtr.Zero)
            {
                WindowAPI.SendMessage(hWnd, WindowAPI.WM_SIZE, IntPtr.Zero, lParam);
            }
            return true;
        }


    }
}