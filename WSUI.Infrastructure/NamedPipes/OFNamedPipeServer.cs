using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Windows.Documents;
using Newtonsoft.Json;
using OF.Core.Interfaces;
using OF.Core.Logger;

namespace OF.Infrastructure.NamedPipes
{
    public class OFNamedPipeServer<T> : IOFNamedPipeServer<T>
    {
        private readonly List<IOFNamedPipeObserver<T>> _observers = new List<IOFNamedPipeObserver<T>>();
        private readonly object LOCK = new object();

        private NamedPipeServerStream _pipeServer;
        private static readonly int BufferSize = 1024;
        private readonly  Thread _serverThread;
        private readonly  string _pipeName;
        private readonly  CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

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
                if(_observers.Contains(observer))
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
                Decoder decoder = Encoding.Default.GetDecoder();
                Byte[] bytes = new Byte[BufferSize];
                char[] chars = new char[BufferSize];
                int numBytes = 0;
                StringBuilder msg = new StringBuilder();
                _pipeServer = new NamedPipeServerStream(_pipeName, PipeDirection.In, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
                var cancelToken = _cancellationTokenSource.Token;
                while (true)
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        break;
                    }
                    _pipeServer.WaitForConnection();

                    do
                    {
                        msg.Length = 0;
                        do
                        {
                            numBytes = _pipeServer.Read(bytes, 0, BufferSize);
                            if (numBytes > 0)
                            {
                                int numChars = decoder.GetCharCount(bytes, 0, numBytes);
                                decoder.GetChars(bytes, 0, numBytes, chars, 0, false);
                                msg.Append(chars, 0, numChars);
                            }
                        } while (numBytes > 0 && !_pipeServer.IsMessageComplete);
                        decoder.Reset();
                        if (numBytes > 0)
                        {
                            DeserializeAndUpdateObservers(msg.ToString());
                        }
                    } while (numBytes != 0);
                }
            }
            catch (Exception e)
            {
                OFLogger.Instance.LogError(e.ToString());
            }
        }

        private void DeserializeAndUpdateObservers(string toString)
        {
            if (string.IsNullOrEmpty(toString))
            {
                return;
            }
            try
            {
                T message = (T)JsonConvert.DeserializeObject<T>(toString);

                lock (LOCK)
                {
                    foreach (var ofNamedPipeObserver in _observers)
                    {
                        ofNamedPipeObserver.Update(message);
                    }
                }
            }
            catch (Exception e)
            {
                OFLogger.Instance.LogError(e.ToString());
            }
        }
    }
}