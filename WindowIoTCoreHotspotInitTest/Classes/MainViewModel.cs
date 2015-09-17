using Falafel.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace WindowIoTCoreHotspotInitTest.Classes
{
    public class MainViewModel : INotifyPropertyChanged
    {
        const string PASSWORD = "yourPassword";
        public MainViewModel()
        {
            _DeviceName = "minwinpc";            
            _GetInterfaces = new DelegateCommand(async (x) => {
                this.Status = "Getting Interfaces";
                using (HttpClient client = new HttpClient())
                {
                    using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, string.Format("http://{0}:8080/api/wifi/interfaces", this.DeviceName)))
                    {
                        request.Headers.Authorization = CreateBasicCredentials("Administrator", PASSWORD);
                        using (HttpResponseMessage response = await client.SendAsync(request))
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            var interfaces = JsonConvert.DeserializeObject<AvailableInterfaces>(data);
                            var ourInterface = interfaces.Interfaces.First();
                            this.InterfaceDescription = ourInterface.Description;
                            this.InterfaceGUID = ourInterface.GUID.Replace('{', ' ').Replace('}', ' ').Trim();
                            this.Status = "Getting Interfaces Succeeded";
                        }
                    }
                }
            }, (y) => { return true; } );
            _RefreshWifi = new DelegateCommand(async (x) => {
                this.Status = "Refreshing Wifi";
                using (HttpClient client = new HttpClient())
                {
                    using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, string.Format("http://{0}:8080/api/wifi/networks?interface={1}", this.DeviceName, this.InterfaceGUID)))
                    {
                        request.Headers.Authorization = MainViewModel.CreateBasicCredentials("Administrator", PASSWORD);
                        using (HttpResponseMessage response = await client.SendAsync(request))
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            var networks = JsonConvert.DeserializeObject<AvailableNetworksObject>(data);
                            this.Networks = networks.AvailableNetworks;
                            this.Status = "Refresh Wifi Succeeded";
                        }
                    }
                }
            }, (y) => { return true; });
            _ConnectWifi = new DelegateCommand(async (x) => {
                this.Status = "Connecting Wifi";
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = MainViewModel.CreateBasicCredentials("Administrator", PASSWORD);
                    using (HttpResponseMessage response = await client.PostAsync(string.Format("http://{0}:8080/api/wifi/network?interface={1}&ssid={2}&op=connect&createprofile=yes&key={3}", this.DeviceName, this.InterfaceGUID, this.SelectedWifi.SSID, EncodeTo64(this.WifiKey)), null))
                    {
                        this.Status = "Connecting Wifi Succeeded";
                    }
                }
            }, (y) => { return this.SelectedWifi != null; });
            _DisconnectWifi = new DelegateCommand(async (x) => {
                this.Status = "Disconnecting Wifi";
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = MainViewModel.CreateBasicCredentials("Administrator", PASSWORD);
                    using (HttpResponseMessage response = await client.PostAsync(string.Format("http://{0}:8080/api/wifi/network?interface={1}&op=disconnect", this.DeviceName, this.InterfaceGUID), null))
                    {
                        this.Status = "Disconnecting Wifi Succeeded";
                    }
                }
            }, (y) => { return this.SelectedWifi != null; });
        }

        string _DeviceName;
        public string DeviceName
        {
            get
            {
                return _DeviceName;
            }
            set
            {
                _DeviceName = value;
                OnPropertyChanged("DeviceName");
            }
        }

        string _WifiKey;
        public string WifiKey
        {
            get
            {
                return _WifiKey;
            }
            set
            {
                _WifiKey = value;
                OnPropertyChanged("WifiKey");
            }
        }

        string _InterfaceDescription;
        public string InterfaceDescription
        {
            get
            {
                return _InterfaceDescription;
            }
            set
            {
                _InterfaceDescription = value;
                OnPropertyChanged("InterfaceDescription");
            }
        }

        string _InterfaceGUID;
        public string InterfaceGUID
        {
            get
            {
                return _InterfaceGUID;
            }
            set
            {
                _InterfaceGUID = value;
                OnPropertyChanged("InterfaceGUID");
            }
        }

        string _Status;
        public string Status
        {
            get
            {
                return _Status;
            }
            set
            {
                _Status = value;
                OnPropertyChanged("Status");
            }
        }

        IEnumerable<WifiNetwork> _Networks;
        public IEnumerable<WifiNetwork> Networks
        {
            get
            {
                return _Networks;
            }
            set
            {
                _Networks = value;
                OnPropertyChanged("Networks");
            }
        }

        WifiNetwork _SelectedWifi;
        public WifiNetwork SelectedWifi
        {
            get
            {
                return _SelectedWifi;
            }
            set
            {
                _SelectedWifi = value;
                this.WifiKey = string.Empty;
                OnPropertyChanged("SelectedWifi");
                OnPropertyChanged("ConnectWifi");
                OnPropertyChanged("DisconnectWifi");
            }
        }

        DelegateCommand _GetInterfaces;
        public DelegateCommand GetInterfaces {
            get
            {
                return _GetInterfaces;
            }
        }

        DelegateCommand _RefreshWifi;
        public DelegateCommand RefreshWifi
        {
            get
            {
                return _RefreshWifi;
            }
        }

        DelegateCommand _ConnectWifi;
        public DelegateCommand ConnectWifi
        {
            get
            {
                return _ConnectWifi;
            }
        }
        
        DelegateCommand _DisconnectWifi;
        public DelegateCommand DisconnectWifi
        {
            get
            {
                return _DisconnectWifi;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public static AuthenticationHeaderValue CreateBasicCredentials(string userName, string password)
        {
            string toEncode = userName + ":" + password;
            Encoding encoding = Encoding.GetEncoding("iso-8859-1");
            byte[] toBase64 = encoding.GetBytes(toEncode);
            string parameter = Convert.ToBase64String(toBase64);
            return new AuthenticationHeaderValue("Basic", parameter);
        }

        static public string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);
            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }
    }

    public class DesignTimeMainViewModel : MainViewModel
    {
        public DesignTimeMainViewModel()
        {
            this.InterfaceDescription = "Wifi Dongle";
            this.InterfaceGUID = "GUID";
            this.Networks = new List<WifiNetwork>();
            (this.Networks as List<WifiNetwork>).Add(new WifiNetwork()
            {
                SSID = "Wifi 1",
                AlreadyConnected = false,
                SignalQuality = 20,
            });
            (this.Networks as List<WifiNetwork>).Add(new WifiNetwork()
            {
                SSID = "Wifi 2",
                AlreadyConnected = true,
                SignalQuality = 50,
            });
            (this.Networks as List<WifiNetwork>).Add(new WifiNetwork()
            {
                SSID = "Wifi 3",
                AlreadyConnected = false,
                SignalQuality = 10,
            });
        }
    }
}
