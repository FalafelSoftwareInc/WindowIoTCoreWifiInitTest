using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowIoTCoreHotspotInitTest.Classes
{
    class AvailableInterfaces
    {
        public IEnumerable<WifiInterface> Interfaces { get; set; }
    }

    class WifiInterface
    {
        public String Description { get; set; }
        public String GUID { get; set; }
        public int Index { get; set; }
    }
}
