using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Configuration;
using OFPreview.PreviewHandler.PreviewHandlerFramework;
using System.Management;
using System.Windows.Threading;
using OFPreview.PreviewHandler.TypeResolver;
using OF.Core.Data;
using OF.Core.Extensions;
using OF.Core.Logger;
using OF.Core.Win32;

namespace OFPreview.PreviewHandler
{
    [KeyControl(ControlsKey.Pdf)]
    public partial class PDFViewer : UserControl, IPreviewControl
    {

        public enum LinkActionKind
        {
            actionGoTo,
            actionGoToR,
            actionLaunch,
            actionURI,
            actionNamed,
            actionMovie,
            actionUnknown,
        }

        public delegate void RenderNotifyInvoker(int page, bool isCurrent);

        [DllImport("user32.dll")]
        static extern int GetForegroundWindow();

        public static PDFViewer Instance;

        #region Mouse Scrolling/Navigation Private Fields
        public enum CursorStatus
        {
            Select,
            Move,
            Zoom,
            Snapshot
        }

        Rectangle EmptyRectangle = new Rectangle(-1, -1, 0, 0);
        CursorStatus _cursorStatus = CursorStatus.Move;
        Point _pointStart = Point.Empty;
        Point _pointCurrent = Point.Empty;

        Point _bMouseCapturedStart = Point.Empty;
        bool _bMouseCaptured = false;

        #endregion

        dynamic _pdfDoc = null;
        public PDFViewer()
        {
            InitializeComponent();
            pageViewControl1.PageSize = new Size(pageViewControl1.Width, (int)(pageViewControl1.Width * 11 / 8.5));
            pageViewControl1.Visible = true;
            Instance = this;
            this.Resize += frmPDFViewer_Resize;
        }

        public void LoadFile(string filename)
        {
            OFLogger.Instance.LogError("Check");
            try
            {
                Clear();
                _pdfDoc = PdfTypeLoader.Instance.GetPdfWrapper();
                int ts = Environment.TickCount;
                bool res = false;
                if (res = LoadFile(filename, _pdfDoc))
                {

                    _pdfDoc.NextPage();
                    _pdfDoc.PreviousPage();
                    //_pdfDoc.RenderPage(pageViewControl1.Handle);
                    Render();
                }
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.Message);
            }
#pragma warning disable 1058
            catch
            {
                uint codeError = WindowsFunction.GetLastError();
                OFLogger.Instance.LogError("Unmanaged Error. Code: {0}", codeError);
            }
#pragma warning restore 1058
        }

        public void LoadFile(Stream stream)
        {
        }

        public void LoadObject(OFBaseSearchObject obj)
        {

        }

        public void Clear()
        {
            OFLogger.Instance.LogError("Check");
            if (_pdfDoc != null)
            {
                _pdfDoc.Dispose();
                _pdfDoc = null;
            }
        }

        #region Mouse Scrolling
        private CursorStatus getCursorStatus(MouseEventArgs e)
        {
            OFLogger.Instance.LogError("Check");
            if (e.Button == MouseButtons.Middle)
            {
                return CursorStatus.Move;
            }
            return _cursorStatus;
        }
        private bool IsActive
        {
            get
            {
                return true;// GetForegroundWindow() == this.Handle.ToInt32() ;
            }
        }
        private bool MouseInPage(Point p)
        {
            OFLogger.Instance.LogError("Check");
            if (IsActive)
            {
                return pageViewControl1.MouseInPage(p);
            }
            return false;
        }

        void HookManager_MouseMove(object sender, MouseEventArgs e)
        {
            Point pos = pageViewControl1.PointToClient(e.Location);
            if (_pdfDoc != null)
            {
                if (MouseInPage(pos) && _bMouseCaptured)
                {
                    //Handled by the control
                }
                else if (MouseInPage(pos))
                {
                    //Check if we have the pointer on a link
                    if (getCursorStatus(e) == CursorStatus.Move)
                    {
                        dynamic link = SearchLink(e.Location);
                        if (link != null)
                        {
                            pageViewControl1.Cursor = Cursors.Hand;
                        }
                        else
                        {
                            pos = pageViewControl1.PointUserToPage(pos);
                            if (!_pdfDoc.IsBusy && _pdfDoc.Pages.Count > 0 &&
                                _pdfDoc.Pages[_pdfDoc.CurrentPage].HasText(pos.X, pos.Y))
                            {
                                /*if (_pdfDoc.Pages[_pdfDoc.CurrentPage].ImagesCount > 0)
                                {
                                    Image img = _pdfDoc.Pages[_pdfDoc.CurrentPage].GetImage(0);
                                    img.Save("C:\\image_extracted_0.jpg");
                                }*/

                                /*img = _pdfDoc.Pages[_pdfDoc.CurrentPage].GetImage(1);
                                img.Save("C:\\image_extracted_1.jpg");
                                img = _pdfDoc.Pages[_pdfDoc.CurrentPage].GetImage(2);
                                img.Save("C:\\image_extracted_2.jpg");*/

                                pageViewControl1.Cursor = Cursors.IBeam;
                            }
                            else
                                pageViewControl1.Cursor = Cursors.Default;
                        }
                    }
                }
            }
            _pointCurrent = e.Location; //Update current Point
        }

        private dynamic SearchLink(Point location)
        {
            if (_pdfDoc != null)
            {
                Point p = pageViewControl1.PointToClient(location);
                List<dynamic> links = _pdfDoc.GetLinks(_pdfDoc.CurrentPage);
                if (links != null)
                {
                    //Search for a link
                    foreach (dynamic pl in links)
                    {
                        //Convert coordinates
                        Point p1 = Point.Ceiling(_pdfDoc.PointUserToDev(new PointF(pl.Bounds.Left, pl.Bounds.Top)));
                        Point p2 = Point.Ceiling(_pdfDoc.PointUserToDev(new PointF(pl.Bounds.Right, pl.Bounds.Bottom)));
                        Rectangle linkLoc = new Rectangle(p1.X, p1.Y, p2.X - p1.X, p1.Y - p2.Y);
                        //Translate
                        linkLoc.Offset(-pageViewControl1.CurrentView.X, -pageViewControl1.CurrentView.Y);
                        linkLoc.Offset(pageViewControl1.PageBounds.X, pageViewControl1.PageBounds.Y);
                        linkLoc.Offset(0, p2.Y - p1.Y);
                        if (linkLoc.Contains(p))
                            //Link found!
                            return pl;
                    }
                }
            }
            return null;
        }

        void HookManager_MouseUp(object sender, MouseEventArgs e)
        {
            Point pos = pageViewControl1.PointToClient(e.Location);
            if (_pdfDoc != null && MouseInPage(pos) && _bMouseCaptured)
            {
                switch (getCursorStatus(e))
                {
                    case CursorStatus.Move:

                        break;
                    case CursorStatus.Zoom:
                        if (!_pointCurrent.Equals(EmptyPoint))
                        {
                            if (e.Button == MouseButtons.Left && _pdfDoc != null)
                                _pdfDoc.ZoomIN();
                            else if (e.Button == MouseButtons.Right && _pdfDoc != null)
                                _pdfDoc.ZoomOut();
                        }
                        else
                        {
                            //Zoom on rectangle

                        }
                        break;
                }
                pageViewControl1.Cursor = Cursors.Default;
            }
            ReleaseRubberFrame();
            _bMouseCaptured = false;
        }

        void HookManager_MouseDown(object sender, MouseEventArgs e)
        {
            Point pos = pageViewControl1.PointToClient(e.Location);
            if (_pdfDoc != null && MouseInPage(pos) && e.Button == MouseButtons.Left)
            {
                dynamic link = SearchLink(e.Location);
                if (link != null)
                {
                    var linkKind = (LinkActionKind)link.Action.Kind;
                    switch (linkKind)
                    {
                        case LinkActionKind.actionGoTo:

                            dynamic plgo = link.Action;
                            if (plgo.Destination != null)
                            {
                                _pdfDoc.CurrentPage = plgo.Destination.Page;
                                PointF loc = _pdfDoc.PointUserToDev(new PointF((float)plgo.Destination.Left, (float)plgo.Destination.Top));
                                if (plgo.Destination.ChangeTop)
                                    ScrolltoTop((int)loc.Y);
                                else
                                    ScrolltoTop(0);
                                _pdfDoc.RenderPage(pageViewControl1.Handle);
                                Render();
                            }
                            else if (plgo.DestinationName != null)
                            {

                            }
                            break;
                        case LinkActionKind.actionURI:
                            dynamic uri = link.Action;
                            if (MessageBox.Show("Ejecutar aplicación externa?" + Environment.NewLine + uri.URL, Text, MessageBoxButtons.OKCancel) == DialogResult.OK)
                            {
                                System.Diagnostics.Process p = new System.Diagnostics.Process();
                                p.StartInfo.FileName = GetDefaultBrowserPath();
                                p.StartInfo.Arguments = uri.URL;
                                p.Start();
                            }
                            break;
                    }
                }
                else
                {
                    _pointCurrent = e.Location;
                    _pointStart = e.Location;
                    _bMouseCaptured = true;
                }
            }
        }


        #endregion

        void frmPDFViewer_Resize(object sender, EventArgs e)
        {
            OFLogger.Instance.LogError("Check");
            if (_pdfDoc != null)
            {
                FitSize();
                Render();
            }
        }

        private void Render()
        {
            OFLogger.Instance.LogError("Check");
            pageViewControl1.PageSize = new Size(_pdfDoc.PageWidth, _pdfDoc.PageHeight);
            labelPage.Text = string.Format("{0}/{1}", _pdfDoc.CurrentPage, _pdfDoc.PageCount);
            pageViewControl1.Refresh();
            Invalidate();
        }

        private void FitSize()
        {
            if (_pdfDoc != null && _pdfDoc.CurrentPage > 0)
            {
                using (PictureBox box = new PictureBox())
                {
                    box.Width = pageViewControl1.ClientSize.Width;
                    box.Height = pageViewControl1.ClientSize.Height;
                    _pdfDoc.FitToWidth(box.Handle);
                }
                _pdfDoc.RenderPage(pageViewControl1.Handle);
            }
        }

        private void FitWidth()
        {
            OFLogger.Instance.LogError("Check");
            if (_pdfDoc != null && _pdfDoc.CurrentPage > 0)
            {
                using (PictureBox box = new PictureBox())
                {
                    box.Width = pageViewControl1.ClientSize.Width;
                    _pdfDoc.FitToWidth(box.Handle);
                }
                _pdfDoc.RenderPage(pageViewControl1.Handle);
            }
        }

        private void FitHeight()
        {
            if (_pdfDoc != null && _pdfDoc.CurrentPage > 0)
            {
                _pdfDoc.FitToHeight(pageViewControl1.Handle);
                _pdfDoc.RenderPage(pageViewControl1.Handle);
            }
        }


        private void tsbNext_Click(object sender, EventArgs e)
        {
            OFLogger.Instance.LogError("Check");
            if (!PdfOK())
                return;

            if (_pdfDoc != null)
            {
                try
                {
                    _pdfDoc.NextPage();
                    _pdfDoc.RenderPage(pageViewControl1.Handle);
                    Render();

                }
                catch (Exception ex)
                {
                    OFLogger.Instance.LogError(ex.Message);
                }

            }

        }

        private void tsbPrev_Click(object sender, EventArgs e)
        {
            OFLogger.Instance.LogError("Check");
            if (!PdfOK())
                return;

            if (_pdfDoc != null && !IsDisposed)
            {
                try
                {
                    _pdfDoc.PreviousPage();
                    _pdfDoc.RenderPage(pageViewControl1.Handle);
                    Render();
                }
                catch (Exception ex)
                {
                    OFLogger.Instance.LogError(ex.Message);
                }
            }
        }

        public bool LoadStream(System.IO.Stream fileStream)
        {
            OFLogger.Instance.LogError("Check");
            if (_pdfDoc != null)
            {
                _pdfDoc.Dispose();
                _pdfDoc = null;
            }
            _pdfDoc = PdfTypeLoader.Instance.GetPdfWrapper();

            try
            {
                bool bRet = _pdfDoc.LoadPDF(fileStream);
                return bRet;
            }
            catch (System.Security.SecurityException)
            {
                return false;
            }

        }
        public bool ShowStream(System.IO.Stream fileStream)
        {
            OFLogger.Instance.LogError("Check");
            if (LoadStream(fileStream))
            {

                _pdfDoc.CurrentPage = 1;
                _pdfDoc.FitToWidth(pageViewControl1.Handle);
                _pdfDoc.RenderPage(pageViewControl1.Handle);

                Render();

                dynamic pg = _pdfDoc.Pages[1];

                return true;
            }
            return false;
        }

        private bool LoadFile(string filename, dynamic pdfDoc)
        {
            OFLogger.Instance.LogError("Check");
            try
            {
                bool bRet = pdfDoc.LoadPDF(filename);
                return bRet;
            }
            catch (System.Security.SecurityException)
            {

                return false;

            }
        }
        private void ScrolltoTop(int y)
        {
            OFLogger.Instance.LogError("Check");
            Point dr = this.pageViewControl1.ScrollPosition;
            if (_pdfDoc.PageHeight > pageViewControl1.Height)
                dr.Y = y;
            pageViewControl1.ScrollPosition = dr;
        }


        private void tsbZoomIn_Click(object sender, EventArgs e)
        {
            OFLogger.Instance.LogError("Check");
            try
            {

                if (_pdfDoc != null)
                {
                    _pdfDoc.ZoomIN();
                    _pdfDoc.RenderPage(pageViewControl1.Handle);
                    Render();
                }

            }
            catch (Exception) { }

        }

        private void tsbZoomOut_Click(object sender, EventArgs e)
        {
            OFLogger.Instance.LogError("Check");
            try
            {
                if (_pdfDoc != null)
                {
                    _pdfDoc.ZoomOut();
                    _pdfDoc.RenderPage(pageViewControl1.Handle);
                    Render();
                }
            }
            catch (Exception) { }
        }

        #region Rubber Frame
        Point EmptyPoint = new Point(-1, -1);
        Point _lastPoint = new Point(-1, -1);
        private void DrawRubberFrame()
        {

            if (!_lastPoint.Equals(EmptyPoint) ||
                (_bMouseCaptured && !_pointCurrent.Equals(EmptyPoint))
            )

                if (!_lastPoint.Equals(EmptyPoint))
                {
                    ReleaseRubberFrame();
                }
            _lastPoint = _pointCurrent;
            DrawRubberFrame(_pointStart, _pointCurrent);

        }
        private void ReleaseRubberFrame()
        {
            if (!_lastPoint.Equals(EmptyPoint))
            {
                DrawRubberFrame(_pointStart, _lastPoint);
            }
            _lastPoint = EmptyPoint;
        }
        private void DrawRubberFrame(Point p1, Point p2)
        {
            Rectangle rc = new Rectangle();

            // Convert the points to screen coordinates.
            //p1 = PointToScreen(p1);
            //p2 = PointToScreen(p2);
            // Normalize the rectangle.
            if (p1.X < p2.X)
            {
                rc.X = p1.X;
                rc.Width = p2.X - p1.X;
            }
            else
            {
                rc.X = p2.X;
                rc.Width = p1.X - p2.X;
            }
            if (p1.Y < p2.Y)
            {
                rc.Y = p1.Y;
                rc.Height = p2.Y - p1.Y;
            }
            else
            {
                rc.Y = p2.Y;
                rc.Height = p1.Y - p2.Y;
            }
            // Draw the reversible frame.
            ControlPaint.DrawReversibleFrame(rc,
                            Color.Gray, FrameStyle.Dashed);

        }
        #endregion
        private static string GetDefaultBrowserPath()
        {
            string key = @"htmlfile\shell\open\command";
            Microsoft.Win32.RegistryKey registryKey =
            Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(key, false);
            // get default browser path
            return ((string)registryKey.GetValue(null, null)).Split('"')[1];

        }


        private void doubleBufferControl1_PaintControl(object sender, Rectangle view, Point location, Graphics g)
        {
            OFLogger.Instance.LogError("Check");
            if (_pdfDoc != null)
            {
                Size sF = new Size(view.Right, view.Bottom);
                Rectangle r = new Rectangle(location, sF);
                _pdfDoc.ClientBounds = r;
                _pdfDoc.CurrentX = view.X;
                _pdfDoc.CurrentY = view.Y;
                _pdfDoc.DrawPageHDC(g.GetHdc());
                g.ReleaseHdc();
                //_pdfDoc.RenderPage(pageViewControl1.Handle);
                //Render();
                //Invalidate();

                /*
                Size sF = new Size(view.Right, view.Bottom);
                Rectangle r = new Rectangle(location, sF);

                _pdfDoc.SliceBox = new Rectangle(Convert.ToInt32(view.X * _pdfDoc.Zoom / 72), Convert.ToInt32(view.Y * _pdfDoc.Zoom / 72), view.Width, view.Height);
                _pdfDoc.RenderPage(pageViewControl1.Handle, true);

                _pdfDoc.ClientBounds = r;
                _pdfDoc.CurrentX = 0;
                _pdfDoc.CurrentY = 0;

                _pdfDoc.DrawPageHDC(g.GetHdc());
                g.ReleaseHdc();*/
                /*
                if (_pdfDoc.RenderDPI >= g.DpiX)
                {
                    foreach (PageLink pl in _pdfDoc.GetLinks(_pdfDoc.CurrentPage))
                    {
                        //Convert coordinates
                        Point p1 = Point.Ceiling(_pdfDoc.PointUserToDev(new PointF(pl.Bounds.Left, pl.Bounds.Top)));
                        Point p2 = Point.Ceiling(_pdfDoc.PointUserToDev(new PointF(pl.Bounds.Right, pl.Bounds.Bottom)));
                        Rectangle linkLoc = new Rectangle(p1.X, p1.Y, p2.X - p1.X, p1.Y - p2.Y);
                        //Translate
                        linkLoc.Offset(-view.X, -view.Y);
                        linkLoc.Offset(r.X, r.Y);
                        linkLoc.Offset(0, p2.Y - p1.Y);
                        //Draw Rectangle
                        g.DrawRectangle(Pens.Blue, linkLoc);
                    }
                }
                */
            }
        }

        private bool doubleBufferControl1_NextPage(object sender)
        {
            OFLogger.Instance.LogError("Check");
            if (_pdfDoc == null)
            {
                return false;
            }
            try
            {

                if (_pdfDoc.CurrentPage < _pdfDoc.PageCount)
                {

                    _pdfDoc.NextPage();
                    _pdfDoc.RenderPage(pageViewControl1.Handle);
                    Render();
                    return true;
                }

            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.Message);
            }
            return false;
        }

        private bool doubleBufferControl1_PreviousPage(object sender)
        {
            OFLogger.Instance.LogError("Check");
            if (_pdfDoc == null)
            {
                return false;
            }
            try
            {

                if (_pdfDoc.CurrentPage > 1)
                {
                    _pdfDoc.PreviousPage();

                    _pdfDoc.RenderPage(pageViewControl1.Handle);
                    Render();
                    return true;
                }

            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.Message);
            }
            return false;
        }

        bool PdfOK()
        {
            OFLogger.Instance.LogError("Check");
            if (_pdfDoc != null && _pdfDoc.PageCount > 0)
                return true;
            return false;
        }

        private void ApplyDynamicEvents(object o, string eventname, string methodname, bool isAdd = true)
        {
            EventInfo ei = o.GetType().GetEvent(eventname);
            var type = this.GetType();
            MethodInfo mi = type.GetMethod(methodname, BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance);
            if (ei.IsNotNull() && mi.IsNotNull())
            {
                Delegate del = Delegate.CreateDelegate(ei.EventHandlerType, null, mi);
                if (isAdd)
                {
                    ei.AddEventHandler(o, del);
                }
                else
                {
                    ei.RemoveEventHandler(o, del);
                }
            }
        }
    }




    public delegate int SearchPdfHandler(object sender, SearchArgs e);

    public class SearchArgs : EventArgs
    {
        public string Text;
        public bool FromBegin;
        public bool Exact;
        public bool WholeDoc;
        public bool FindNext;
        public bool Up;
        public bool WholeWord;

        internal SearchArgs(string text, bool frombegin, bool exact, bool wholedoc, bool findnext, bool up, bool wholeword)
        {
            Text = text;
            FromBegin = frombegin;
            Exact = exact;
            WholeDoc = wholedoc;
            FindNext = findnext;
            WholeWord = wholeword;
            Up = up;
        }
    }



}