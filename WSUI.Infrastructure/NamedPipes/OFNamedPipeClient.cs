using System;
using System.IO;
using System.IO.Pipes;
using Newtonsoft.Json;
using OF.Core.Interfaces;
using OF.Core.Logger;

namespace OF.Infrastructure.NamedPipes
{
    public class OFNamedPipeClient<T> : IOFNamedPipeClient<T>
    {
        private readonly string _pipeName;

        public OFNamedPipeClient(string pipeName)
        {
            _pipeName = pipeName;
        }

        public void Send(T message)
        {
            try
            {
                string strMessage = JsonConvert.SerializeObject(message);
                if (string.IsNullOrEmpty(strMessage))
                {
                    return;
                }
                InternalSend(strMessage);
            }
            catch (Exception e)
            {
                OFLogger.Instance.LogError(e.ToString());
            }
        }

        private void InternalSend(string message)
        {
            using (NamedPipeClientStream client = new NamedPipeClientStream(".",_pipeName,PipeDirection.Out,PipeOptions.Asynchronous))
            {
                try
                {
                    client.Connect(2000);
                }
                catch (Exception e)
                {
                    OFLogger.Instance.LogError(e.ToString());
                }
                using (StreamWriter sw = new StreamWriter(client))
                {
                    sw.WriteLine(message);
                }
            }
        }
    }
}