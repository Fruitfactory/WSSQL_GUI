using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WSPreview.PreviewHandler.PInvoke;
using WSPreview.PreviewHandler.PreviewHandlerFramework;
using WSPreview.PreviewHandler.Service;
using WSUI.Core.Logger;
using WSPreview.PreviewHandler.Service.Preview;
using WSUI.Core.Interfaces;

namespace WSPreview.PreviewHandler.PreviewHandlerHost
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

        private System.Windows.Forms.WebBrowser webMessage = new System.Windows.Forms.WebBrowser();

        private string _filePath;
        private string _searchCriteria;
        private object _comInstance = null;
        private bool _registreHandler = true;
        private readonly List<Stream> _listOpenStream = new List<Stream>();
        private readonly RegistrationData _dataHandler;

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
            HelperPreviewHandlers.Instance.Inititialize();
            _dataHandler = PreviewHandlerRegistryAccessor.LoadRegistrationInformation();
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
                if(Marshal.IsComObject(_comInstance))
                    Marshal.ReleaseComObject(_comInstance);
                _comInstance = null;
            }
            ClearAllStreams();
        }

        public void PassAction(IWSAction action)
        {
            if (_comInstance != null && _comInstance is ITranslateMessage)
            {
                ((ITranslateMessage)_comInstance).PassMessage(action);
            }
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
            if (this.Controls.Contains(webMessage))
            {
                this.Controls.Remove(webMessage);
                webMessage.Visible = false;
            }
            ClearAllStreams();
            if (_comInstance != null)
            {
                ((IPreviewHandler)_comInstance).Unload();
                _comInstance = null;
            }
            _registreHandler = true;
            
            
            //check registry
            PreviewHandlerInfo handler = null;
            Type comType = null;
            var extens = Path.GetExtension(_filePath).ToUpperInvariant();
            if (!string.IsNullOrEmpty(extens) && _dataHandler.Extensions.ContainsKey(extens) && !IsBlockedExt(_filePath))
            {
                handler = _dataHandler.Extensions[extens].Handler;
            }

            // check application handlers
            if (IsBlockedExt(_filePath) || handler == null)
            {
                _comInstance = GetOwnPreviewHandler();
                if (_comInstance == null)
                {
                    comType = GetOwnPreviewHandlersType();
                    if (comType == null)
                    {
                        string ext = Path.GetExtension(_filePath).ToLower();
                        webMessage.Dock = DockStyle.Fill;
                        this.Controls.Add(webMessage);
                        webMessage.Visible = true;
                        var str = Regex.Replace(Properties.Resources.NoPreview, "replace", ext);
                        webMessage.DocumentText = str;
                        WSSqlLogger.Instance.LogWarning(string.Format("{0}: {1}", "No Preview", _filePath));
                        return;
                    }
                }
                _registreHandler = false;    
            } 
            else
                comType = Type.GetTypeFromCLSID(new Guid(handler.ID));

            try
            {
                GenerateProviewByType(comType);
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(string.Format("{0}: {1}", "WinError", Marshal.GetLastWin32Error()));
                WSSqlLogger.Instance.LogError(string.Format("{0}: {1}", "Error", ex.Message));
                _comInstance = null;
                SecondChance();
            }
        }

        private bool IsBlockedExt(string filename)
        {
            return filename.ToLower().EndsWith(".msg") ||
                   filename.ToLower().EndsWith(".html") ||
                   filename.ToLower().EndsWith(".htm");
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

        private void SecondChance()
        {
            try
            {
                object comInstance = GetOwnPreviewHandler();
                if (comInstance == null)
                {
                    Type comType = GetOwnPreviewHandlersType();
                    if (comType == null)
                        return;
                    GenerateProviewByType(comType);
                }
                else
                {
                    _comInstance = comInstance;
                    GeneratePreviewByInstance();
                }
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(string.Format("{0}: {1}", "WinError", Marshal.GetLastWin32Error()));
                WSSqlLogger.Instance.LogError(string.Format("{0}: {1}", "Error", ex.Message));
                _comInstance = null;
            }
        }

        private void GenerateProviewByType(Type type)
        {
            // Create an instance of the preview handler
            if (_comInstance == null)
                _comInstance = Activator.CreateInstance(type);
            GeneratePreviewByInstance();            
        }

        private void GeneratePreviewByInstance()
        {
            //// Check if it is a stream or file handler
            if (_comInstance is IInitializeWithFile)
            {
                if (_comInstance is ISearchWordHighlight)
                    ((ISearchWordHighlight)_comInstance).HitString = SearchCriteria;
                int res = ((IInitializeWithFile)_comInstance).Initialize(_filePath, 0);
                WSSqlLogger.Instance.LogInfo(string.Format("HRESULT(Initialize)={0}", res));
            }
            else if (File.Exists(_filePath))
            {
                if (_comInstance is IInitializeWithStream)
                {
                    Stream filestream = File.Open(_filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    StreamWrapper stream = new StreamWrapper(filestream);
                    ((IInitializeWithStream)_comInstance).Initialize(stream, 0);
                    AddStream(filestream);
                }
                else if (_comInstance is IInitializeWithItem)
                {
                    IShellItem shellItem = null;
                    UnsafeNativeMethods.SHCreateItemFromParsingName(_filePath, IntPtr.Zero, typeof(IShellItem).GUID,
                        out shellItem);
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

            if (_comInstance == null)
                throw new NullReferenceException("Com type can't be founded");
            RECT r;
            r.top = 0;
            r.bottom = this.Height;
            r.left = 0;
            r.right = this.Width;
            int wndRes = ((IPreviewHandler)_comInstance).SetWindow(this.Handle, ref r);
            WSSqlLogger.Instance.LogInfo(string.Format("HRESULT(SetWindow)={0}", wndRes));
            ((IPreviewHandler)_comInstance).DoPreview();
        }

        private Type GetOwnPreviewHandlersType()
        {
            string ext = Path.GetExtension(_filePath).ToLower();
            return HelperPreviewHandlers.Instance.HandlersDictionary.ContainsKey(ext)
                       ? HelperPreviewHandlers.Instance.HandlersDictionary[ext]
                       : null;
        }

        private PreviewHandlerFramework.PreviewHandler GetOwnPreviewHandler()
        {
            string ext = Path.GetExtension(_filePath).ToLower();
            return HelperPreviewHandlers.Instance.GetReadyHandler(ext);
        }

        private void AddStream(Stream stream)
        {
            _listOpenStream.Add(stream);
        }

        private void ClearAllStreams()
        {
            if (!_listOpenStream.Any())
                return;
            _listOpenStream.ForEach(s => s.Close());
            _listOpenStream.Clear();
        }

    }
}