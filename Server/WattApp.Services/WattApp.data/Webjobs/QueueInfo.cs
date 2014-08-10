using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WattApp.data.Webjobs
{
    public enum DiscoveryOption
    {
        eMeterAndSamples,
        eMeterOnly,
        eForceUpdateMeterAndSamples
    }
    public class CustomerDiscoverQueueInfo
    {
        public DiscoveryOption DiscoveryOption { get; set; }
        public int Id { get; set; }
        public string Guid { get; set; }
    }
}
