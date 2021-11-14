using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NET_TCP_Server
{
    public class TcpServer
    {
        private TcpListener Listener { get; set; }

        private string Page { get; set; }

        public TcpServer(IPAddress address, int port, string page)
        {
            Page = page;
            Listener = new TcpListener(address, port);
        }

        public void Start()
        {
            try
            {
                Listener?.Start();
                while (true)
                {
                    TcpClient client = Listener.AcceptTcpClient();
                    StreamWriter writer = new System.IO.StreamWriter(client.GetStream());
                    writer.Write("HTTP/1.0 200 OK");
                    writer.Write(Environment.NewLine);
                    writer.Write("Content-Length: " + Page.Length);
                    writer.Write(Environment.NewLine);
                    writer.Write(Environment.NewLine);
                    writer.Write(Page);

                    writer.Flush();
                    client.Close();
                }
            }
            catch
            {
                Stop();
            }
        }

        public void Stop()
        {
            Listener?.Stop();
        }
    }
}
