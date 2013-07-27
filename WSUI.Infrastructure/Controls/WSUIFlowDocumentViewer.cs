using System;
using System.Reflection;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace WSUI.Infrastructure.Controls
{
    public class WSUIFlowDocumentViewer : FlowDocumentScrollViewer
    {
        private Frame _contentHost;
        private dynamic _view;


        private const string _contentHostTemplateName = "PART_ContentHost"; 

        public override void OnApplyTemplate()
        {
            _contentHost = GetTemplateChild(_contentHostTemplateName) as Frame;
            if (_contentHost != null)
            {
                if (_contentHost.Content != null)
                {
                    this._contentHost.Content = (object)null;
                }

                // Initialize the content of the Frame.
                Assembly ass = typeof (FlowDocumentScrollViewer).Assembly;
                Type t = ass.GetType("MS.Internal.Documents.FlowDocumentView");
                // call internal constructor ))
                _view = t.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null).Invoke (null);
                _contentHost.Content = _view;
                if (_view != null)
                {
                    var pi = _view.GetType().GetProperty("Document", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (pi != null)
                    {
                        pi.SetValue(_view, Document, null);
                    }    
                }
            }
            base.OnApplyTemplate();
        }

        public void SetDocument(FlowDocument document)
        {
            Document = document;
            AddLogicalChild(document);
            if (_view != null)
            {
                var pi = _view.GetType().GetProperty("Document", BindingFlags.NonPublic | BindingFlags.Instance);
                if (pi != null)
                {
                    pi.SetValue(_view, document, null);
                }
            }
            FlowDocumentScrollViewerAutomationPeer peer = UIElementAutomationPeer.FromElement(this) as FlowDocumentScrollViewerAutomationPeer;
            if (peer != null)
            {
                peer.InvalidatePeer();
            }
        }


        //protected override void OnMouseWheel(System.Windows.Input.MouseWheelEventArgs e)
        //{
        //    e.Handled = true;
        //}

        //protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        //{
        //    //e.Handled = true;
        //}

        //protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    base.OnMouseDown(e);
        //    Focusable = true;
        //}

        //protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        //{
        //    base.OnMouseLeave(e);
        //    Focusable = false;
        //}

        //protected override void OnPreviewMouseWheel(System.Windows.Input.MouseWheelEventArgs e)
        //{
        //    e.Handled = true;
        //}

    }
}