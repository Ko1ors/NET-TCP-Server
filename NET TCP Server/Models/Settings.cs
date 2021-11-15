using System.ComponentModel;

namespace NET_TCP_Server.Models
{
    public class Settings : INotifyPropertyChanged
    {
        public Settings()
        {
            ipAddress = "127.0.0.1";
            port = 4444;
            useCache = true;
            CacheTime = 30;
        }

        private bool started;

        public bool Started
        {
            get { return started; }
            set
            {
                started = value;
                OnPropertyChanged("Started");
                OnPropertyChanged("ButtonText");
            }
        }

        public string ButtonText => Started ? "Stop" : "Start";

        private string ipAddress;

        public string IpAddress
        {
            get { return ipAddress; }
            set
            {
                ipAddress = value;
                OnPropertyChanged("IpAddress");
            }
        }

        private int port;

        public int Port
        {
            get { return port; }
            set
            {
                port = value;
                OnPropertyChanged("Port");
            }
        }

        private int cacheTime;

        public int CacheTime
        {
            get { return cacheTime; }
            set
            {
                cacheTime = value;
                OnPropertyChanged("CacheTime");
            }
        }

        private bool useCache;

        public bool UseCache
        {
            get { return useCache; }
            set
            {
                useCache = value;
                OnPropertyChanged("UseCache");
            }
        }

        private bool updateWhileConnected;

        public bool UpdateWhileConnected
        {
            get { return updateWhileConnected; }
            set
            {
                updateWhileConnected = value;
                OnPropertyChanged("UpdateWhileConnected");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
