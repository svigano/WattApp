using BuildingApi;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WattApp.WebJobs
{
    internal class EquipmentManager
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private ITokenProvider _tokenProvider;
        private string _baseAPIRoot = string.Empty;
        private EquipmentClient _equipmentClient;
        private const string kEletricMeter = "EletricMeter";

        public EquipmentManager(ITokenProvider provider, string baseRootAPI)
        {
            _tokenProvider = provider;
            _baseAPIRoot = baseRootAPI;
            _equipmentClient = new EquipmentClient(_tokenProvider, _baseAPIRoot);

        }

        public void DiscoverEletricMetersData(Company c)
        { 
            IEnumerable<Equipment> eqList = _discoverEquipment(c, kEletricMeter);

            _logger.Info(string.Format("Found {0} {1} on customer {2}", eqList.Count(), kEletricMeter, c.Name));
            foreach (var item in eqList)
            {
                _logger.Info(string.Format("Meter name {0}", item.Name));
            }

        }

        private IEnumerable<Equipment> _discoverEquipment(Company c, string equipmentType) 
        {
            return _equipmentClient.GetEquipmentAndPointRoles(equipmentType, c);
        }
    }
}
