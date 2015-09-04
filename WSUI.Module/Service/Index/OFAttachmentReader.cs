﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Practices.Unity;
using OF.Core.Core.MVVM;
using OF.Core.Data.ElasticSearch;
using OF.Core.Enums;
using OF.Core.Extensions;
using OF.Core.Helpers;
using OF.Core.Interfaces;
using OF.Core.Logger;
using OF.Module.Interface.Service;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace OF.Module.Service.Index
{
    public class OFAttachmentReader : ViewModelBase, IAttachmentReader
    { 
        private string AttachSchema = "http://schemas.microsoft.com/mapi/proptag/0x37010102";
        private Thread _thread;
        private CancellationTokenSource _cancellationSource;
        private CancellationToken _cancellationToken;
        private IElasticSearchIndexAttachmentClient _indexAttachmentClient;
        private DateTime? _lastUpdated;

        private static readonly string DateFormat = "MM/dd/yyyy HH:mm:ss";
        private static int MaxSize = 65536;

        [InjectionConstructor]
        public OFAttachmentReader(IUnityContainer unityContainer)
        {
            Status = PstReaderStatus.None;
            _cancellationSource = new CancellationTokenSource();
            _cancellationToken = _cancellationSource.Token;
            _indexAttachmentClient = unityContainer.Resolve<IElasticSearchIndexAttachmentClient>();
        }

        public PstReaderStatus Status
        {
            get { return Get(() => Status); }
            private set { Set(() => Status, value); }
        }

        public void Start(DateTime? lastUpdated)
        {
            if (_thread == null)
            {
                _thread = new Thread(ProcessAttachment);
            }
            _lastUpdated = lastUpdated;
            _thread.Start();
            Status = PstReaderStatus.Busy;
        }

        public void Stop()
        {
            _cancellationSource.Cancel();
            _thread.Join();
            Status = PstReaderStatus.Finished;
        }

        private void ProcessAttachment(object arg)
        {
            try
            {
                var folderList = OutlookHelper.Instance.GetFolders().OfType<Outlook.MAPIFolder>();
                if (!folderList.Any())
                {
                    return;
                }
                foreach (var mapiFolder in folderList)
                {
                    if (mapiFolder.Items.Count == 0)
                    {
                        continue;
                    }
                    foreach (var result in mapiFolder.Items.OfType<Outlook.MailItem>())
                    {
                        if (result.Attachments.Count == 0 || (_lastUpdated.HasValue && result.ReceivedTime < _lastUpdated.Value))
                        {
                            continue;
                        }

                        List<OFAttachmentContent> attachmentContents = new List<OFAttachmentContent>();
                        foreach (var attachment in result.Attachments.OfType<Outlook.Attachment>())
                        {
                           
                            if (attachment.Size < MaxSize)
                            {
                                continue;
                            }
                            try
                            {
                                byte[] contentBytes = GetContentByProperty(attachment);
                                if (contentBytes.IsNotNull())
                                {
                                    AddAttachment(attachmentContents,result,attachment,contentBytes);
                                }
                            }
                            catch (COMException comEx)
                            {
                                OFLogger.Instance.LogError("----COM Attachment Failed => {0}", attachment.FileName);
                                OFLogger.Instance.LogError(comEx.Message);
                                byte[] conBytes = GetContentByTempFile(attachment);
                                if (conBytes.IsNotNull())
                                {
                                    AddAttachment(attachmentContents,result,attachment,conBytes);
                                }
                            }
                            catch(Exception ex)
                            {
                                OFLogger.Instance.LogError("---- Attachment Failed => {0}", attachment.FileName);
                                OFLogger.Instance.LogError("---- Type Failed => {0}", ex.GetType().Name);
                                OFLogger.Instance.LogError(ex.Message);               
                            }
                        }
                        SendAttachments(attachmentContents,AttachmentIndexProcess.Chunk);
                        CheckCancellation();
                    }
                    CheckCancellation();
                }
            }
            catch (AggregateException ex)
            {
                OFLogger.Instance.LogError("Canceled: {0}",ex.Message);
            }
            catch (Exception common)
            {
                OFLogger.Instance.LogError(common.Message);
            }
            finally
            {
                SendAttachments(null, AttachmentIndexProcess.End);
                Status = PstReaderStatus.Finished;
                System.Diagnostics.Debug.WriteLine("!!!!!!!! Exit From Attachment Reader");
            }
        }


        private byte[] GetContentByProperty(Outlook.Attachment attachment)
        {
            Outlook.PropertyAccessor pacc = attachment.PropertyAccessor;
            byte[] filebyte = (byte[])pacc.GetProperty(AttachSchema);
            return filebyte;
        }

        private byte[] GetContentByTempFile(Outlook.Attachment attachment)
        {
            byte[] buffer = null;
            try
            {
                attachment.SaveAsFile(attachment.FileName);
                buffer = File.ReadAllBytes(attachment.FileName);
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.Message);
            }
            finally
            {
                if (File.Exists(attachment.FileName))
                {
                    File.Delete(attachment.FileName);
                }
            }
            return buffer;
        }

        private void AddAttachment(List<OFAttachmentContent> attachments,Outlook.MailItem email, Outlook.Attachment attachment, byte[] content)
        {
            OFAttachmentContent indexAttach = new OFAttachmentContent();
            indexAttach.Size = attachment.Size;

            int hash = email.Subject.GetIternalHashCode() +
                       email.ReceivedTime.ToString(DateFormat).GetIternalHashCode();
            indexAttach.Emailid = hash.ToString();
            System.Diagnostics.Debug.WriteLine("Subject => {0} ReceivedTime => {1} Id => {2}",
                email.Subject, email.ReceivedTime.ToString(DateFormat),
                hash.ToString());
            OFLogger.Instance.LogError("---- Attachment => {0}", attachment.FileName);
            indexAttach.Filename = attachment.FileName;
            indexAttach.Content = Convert.ToBase64String(content);
            attachments.Add(indexAttach);
        }



        private void SendAttachments(IEnumerable<OFAttachmentContent> attachments, AttachmentIndexProcess process)
        {
            if (_indexAttachmentClient.IsNotNull())
            {
                OFAttachmentIndexingContainer container = new OFAttachmentIndexingContainer() { Attachments = attachments, Process = process };
                _indexAttachmentClient.SendAttachmentToIndex(container);
            }
        }

        private void CheckCancellation()
        {
            if (_cancellationToken.IsCancellationRequested)
            {
                _cancellationToken.ThrowIfCancellationRequested();
            }
        }

    }
}