using NET_TCP_Server.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NET_TCP_Server
{
    public class TcpServer
    {
        private IDataService? service { get; set; }

        private TcpListener Listener { get; set; }

        private string Page { get; set; }

        private DateTime LastUpdate { get; set; }

        private bool NeedUpdate => string.IsNullOrEmpty(CachedPage) || (DateTime.Now - LastUpdate).TotalSeconds > 30;

        private string CachedPage { get; set; }

        public TcpServer(IPAddress address, int port, string page, IDataService? dataService = null)
        {
            Page = page;
            Listener = new TcpListener(address, port);
            service = dataService;
        }

        public void Start()
        {
            try
            {
                Listener?.Start();
                while (true)
                {
                    TcpClient client = Listener.AcceptTcpClient();
                    Task.Run(() =>
                    {
                        try
                        {
                            var stream = client.GetStream();
                            StreamWriter writer = new StreamWriter(stream);

                            if (service is not null)
                            {
                                if (NeedUpdate)
                                    UpdateCachedPage();
                                SendCachedPage(writer);
                            }

                            while (IsConnected(client))
                            {
                                if (NeedUpdate)
                                {
                                    try
                                    {
                                        UpdateCachedPage();
                                        writer = new StreamWriter(stream);
                                        SendCachedPage(writer);
                                    }
                                    catch (Exception ex)
                                    {
                                        Trace.WriteLine($"On update exception: {ex}");
                                    }
                                }
                                Thread.Sleep(50);
                            }

                            stream.Socket.Shutdown(SocketShutdown.Send);

                            stream.Close();
                            client.Client.Close();
                            client.Close();
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine(ex);
                        }
                    });
                }
            }
            catch
            {
                Stop();
            }
        }

        private void SendCachedPage(StreamWriter writer)
        {
            writer.Write("HTTP/1.0 200 OK");
            writer.Write(Environment.NewLine);
            writer.Write("Content-Length: " + CachedPage.Length);
            writer.Write(Environment.NewLine);
            writer.Write(Environment.NewLine);
            writer.Write(CachedPage);

            writer.Flush();
        }

        private void UpdateCachedPage()
        {
            string page = Page;
            var sb = new StringBuilder(page);
            var data = service.GetData();
            foreach (var item in data)
            {
                sb.Replace(item.Key, item.Value);
            }
            page = sb.ToString();
            CachedPage = page;
            LastUpdate = DateTime.Now;
        }

        public static bool IsConnected(TcpClient client)
        {
            bool sConnected = true;

            if (client.Client.Poll(0, SelectMode.SelectRead))
            {
                if (!client.Connected) 
                    sConnected = false;
                else
                {
                    byte[] b = new byte[1];
                    try
                    {
                        if (client.Client.Receive(b, SocketFlags.Peek) == 0)
                        {
                            sConnected = false;
                        }
                    }
                    catch { sConnected = false; }
                }
            }
            return sConnected;
        }

        public void Stop()
        {
            Listener?.Stop();
        }
    }
}
