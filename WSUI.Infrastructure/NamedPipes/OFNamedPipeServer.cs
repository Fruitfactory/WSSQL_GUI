using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Documents;
using Newtonsoft.Json;
using OF.Core.Data.NamedPipeMessages.Response;
using OF.Core.Enums;
using OF.Core.Extensions;
using OF.Core.Interfaces;
using OF.Core.Logger;

namespace OF.Infrastructure.NamedPipes
{
    public class OFNamedPipeServer<T> : IOFNamedPipeServer<T>
    {
        private readonly List<IOFNamedPipeObserver<T>> _observers = new List<IOFNamedPipeObserver<T>>();
        private readonly object LOCK = new object();

        private NamedPipeServerStream _pipeServer;
        private StreamReader _reader;
        private StreamWriter _writer;
        private static readonly int BufferSize = 1024;
        private readonly Thread _serverThread;
        private readonly string _pipeName;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private volatile bool _close = false;

        public OFNamedPipeServer(string pipeName)
        {
            _serverThread = new Thread(run);
            _serverThread.Name = "Thread Pipe: " + pipeName;
            _serverThread.Priority = ThreadPriority.BelowNormal;
            _pipeName = pipeName;
        }


        public void Attach(IOFNamedPipeObserver<T> observer)
        {
            lock (LOCK)
            {
                _observers.Add(observer);
            }
        }

        public void Deattach(IOFNamedPipeObserver<T> observer)
        {
            lock (LOCK)
            {
                if (_observers.Contains(observer))
                    _observers.Remove(observer);
            }
        }

        public void Start()
        {
            _serverThread.Start();
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            if (!_serverThread.Join(1500))
            {
                try
                {
                    _serverThread.Abort();
                }
                catch (Exception e)
                {
                    OFLogger.Instance.LogError(e.ToString());
                }
            }
        }


        private void run()
        {
            try
            {
                _pipeServer = new NamedPipeServerStream(_pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
                _writer = new StreamWriter(_pipeServer);
                _reader = new StreamReader(_pipeServer);
                var cancelToken = _cancellationTokenSource.Token;
                while (true)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        break;
                    }
                    _pipeServer.WaitForConnection();
                    try
                    {
                        var request = _reader.ReadLine();
                        if (request.IsNotNull())
                        {
                            var responses = DeserializeAndUpdateObservers(request);
                            var response =
                                new OFNamedServerResponse() {Status = ofServerResponseStatus.Ok, Body = responses};
                            SerializeAndSend(response);
                        }
                    }
                    catch (Exception e)
                    {
                        SerializeAndSend(
                            new OFNamedServerResponse()
                            {
                                Status = ofServerResponseStatus.Failed,
                                Message = e.ToString()
                            });
                    }
                    finally
                    {
                        _pipeServer.Disconnect();
                    }
                }
            }
            catch (Exception e)
            {
                OFLogger.Instance.LogError(e.ToString());
            }
        }

        private void SerializeAndSend(OFNamedServerResponse response)
        {
            if (_writer.IsNull())
            {
                return;
            }
            var serialized = JsonConvert.SerializeObject(response);
            _writer.WriteLine(serialized);
            _writer.Flush();
        }

        private IEnumerable<object> DeserializeAndUpdateObservers(string toString)
        {
            if (string.IsNullOrEmpty(toString))
            {
                return null;
            }
            try
            {
                T message = (T)JsonConvert.DeserializeObject<T>(toString);
                IEnumerable<object> list = null;
                lock (LOCK)
                {
                    list = _observers.Select(o => o.Update(message)).ToList();
                }
                return list;
            }
            catch (Exception e)
            {
                OFLogger.Instance.LogError(e.ToString());
            }
            return null;
        }
    }
}