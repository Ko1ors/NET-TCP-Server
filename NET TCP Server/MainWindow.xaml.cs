using NET_TCP_Server.Models;
using NET_TCP_Server.Services;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace NET_TCP_Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Settings settings;
        private TcpServer server;

        public MainWindow()
        {
            InitializeComponent();
            settings = new Settings();
            DataContext = settings;
            var homePage = Properties.Resources.home;
            var service = new SystemDataService();

            server = new TcpServer(homePage, settings, service);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (settings.Started)
                server.Stop();
            else
            {
                if (!IPAddress.TryParse(settings.IpAddress, out var ip))
                    return;
                Task.Run(() => server.Start());
            }
        }
    }
}
