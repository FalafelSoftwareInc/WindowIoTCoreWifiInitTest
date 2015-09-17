using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowIoTCoreHotspotInitTest.Classes
{
    public class AvailableNetworksObject
    {
        public IEnumerable<WifiNetwork> AvailableNetworks { get; set; }
    }
    public class WifiNetwork
    {        
        public bool AlreadyConnected { get; set; }
        public String AuthenticationAlgorithm { get; set; }
        public String CipherAlgorithm { get; set; }
        public int Connectable { get; set; }
        public String InfrastructureType { get; set; }
        public bool ProfileAvailable { get; set; }
        public String ProfileName { get; set; }
        public String SSID { get; set; }
        public int SecurityEnabled { get; set; }
        public int SignalQuality { get; set; }
        public IEnumerable<String> PhysicalTypes { get; set; }
    }

}
