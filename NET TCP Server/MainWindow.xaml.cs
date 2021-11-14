using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace NET_TCP_Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var homePage = Properties.Resources.home;

            var server = new TcpServer(IPAddress.Parse("127.0.0.1"), 4444, homePage);
            Task.Run(() => server.Start());
        }
    }
}
