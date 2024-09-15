using System;
using System.Net;
#if !NETFX_CORE
using System.Net.Sockets;
#else
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using System.Threading.Tasks;
#endif
using System.Text;
using PlayFab.Public;

namespace PlayFab
{
#if !NETFX_CORE
    public class UdpPaperTrailLogger : PlayFabLoggerBase
    {
        private Socket socket;
        private IPEndPoint endPoint;

        public UdpPaperTrailLogger(string url, int port)
        {
            IPAddress[] addr = Dns.GetHostAddresses(url);
            ip = addr[0];
            this.url = url;
            this.port = port;
        }

        protected override void BeginUploadLog()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            endPoint = new IPEndPoint(ip, port);
        }

        protected override void EndUploadLog()
        {
            socket.Close();
        }

        protected override void UploadLog(string message)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            socket.SendTo(buffer, endPoint);
        }
    }
#else
    public class UdpPaperTrailLogger2 : PlayFabLoggerBase
    {
        protected DatagramSocket socket;

        public UdpPaperTrailLogger2(string url, int port)
        {
            this.url = url;
            this.port = port;
        }

        protected override void BeginUploadLog()
        {
            socket = new DatagramSocket();
            socket.MessageReceived += ReceiveCallback;
        }

        public void ReceiveCallback(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
            //We don't care about the response.
        }

        protected override void EndUploadLog()
        {
            socket.MessageReceived -= ReceiveCallback;
            socket = null;
        }

        protected override void UploadLog(string message)
        {
            var t = LogAsync(message);
            t.Wait();
        }

        public async Task LogAsync(string message)
        {
            await socket.BindServiceNameAsync(port.ToString());
            using (var stream = await socket.GetOutputStreamAsync(new HostName(url), port.ToString()))
            {
                using (var writer = new DataWriter(stream))
                {
                    var data = Encoding.UTF8.GetBytes(message);
                    writer.WriteBytes(data);
                    writer.StoreAsync();
                }
            }
        }

    }
#endif
}
