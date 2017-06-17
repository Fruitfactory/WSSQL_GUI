using System;
using System.IO;
using System.IO.Pipes;
using Newtonsoft.Json;
using OF.Core.Data.NamedPipeMessages.Response;
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

        public OFNamedServerResponse Send(T message)
        {
            try
            {
                string strMessage = JsonConvert.SerializeObject(message);
                if (string.IsNullOrEmpty(strMessage))
                {
                    return null;
                }
                return InternalSend(strMessage);
            }
            catch (Exception e)
            {
                OFLogger.Instance.LogError(e.ToString());
            }
            return null;
        }

        private OFNamedServerResponse InternalSend(string message)
        {
            using (NamedPipeClientStream client = new NamedPipeClientStream(".", _pipeName, PipeDirection.InOut, PipeOptions.Asynchronous))
            {
                var writer = new StreamWriter(client);
                var reader = new StreamReader(client);
                try
                {
                    client.Connect(2000);
                }
                catch (Exception e)
                {
                    OFLogger.Instance.LogError(e.ToString());
                }
                writer.WriteLine(message);
                writer.Flush();
                var response = reader.ReadLine();
                if (response != null)
                {
                    return JsonConvert.DeserializeObject<OFNamedServerResponse>(response);
                }
                return null;
            }
        }
    }
}