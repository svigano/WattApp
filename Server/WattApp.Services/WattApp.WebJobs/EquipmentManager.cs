using BuildingApi;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WattApp.data.Models;
using WattApp.data.Repositories;
using WattApp.WebJobs.API;

namespace WattApp.WebJobs
{
    internal class EquipmentManager
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private const string kEletricMeter = "EletricMeter";
        private string _baseAPIRoot = string.Empty;
        private ITokenProvider _tokenProvider;
        private EquipmentClient _equipmentClient;
        private SampleClient _sampleClient;
        private WattAppContext _wattAppDB = new WattAppContext();
        private IDataRepository _dataRep = null;

        public EquipmentManager(ITokenProvider provider, string baseRootAPI)
        {
            _tokenProvider = provider;
            _baseAPIRoot = baseRootAPI;
            _equipmentClient = new EquipmentClient(_tokenProvider, _baseAPIRoot);
            _sampleClient = new SampleClient(_tokenProvider, _baseAPIRoot);
            _dataRep = new DataRepository(_wattAppDB);
        }

        public void DiscoverEletricMetersData(Company c)
        { 
            IEnumerable<BuildingApi.Equipment> eqList = _discoverEquipment(c, kEletricMeter);

            _logger.Info(string.Format("Found {0} {1} on customer {2}", eqList.Count(), kEletricMeter, c.Name));
            foreach (var item in eqList)
            {
                _logger.Info(string.Format("Meter name {0}", item.Name));
            }
        }

        public void PullTimeSeriesData()
        {
            var map = _findEnableEquipment();

            foreach (var key in map.Keys)
            {
                var currentCompany = new Company { Id = key };
                _pullTimeSeriesDataByCustomer(currentCompany, map[key]);
                foreach (var item in map[key])
                {
                    Console.WriteLine(string.Format("CustomerGuid {0} Name {1}", key, item.Name));
                }
            }

        }
        private IEnumerable<BuildingApi.Equipment> _discoverEquipment(Company c, string equipmentType) 
        {
            return _equipmentClient.GetEquipmentAndPointRoles(equipmentType, c);
        }

        private void _pullTimeSeriesDataByCustomer(Company c, IList<data.Models.Equipment> eqList)
        { 
            var ptIds = new List<string>();
            foreach (var item in eqList)
                foreach (var pt in item.Points)
                    ptIds.Add(pt.PxGuid);

            var samples = _sampleClient.GetSamples(ptIds.First(), new DateTime(2014, 6, 10, 0, 0, 0), new DateTime(2014, 6, 11, 0, 0, 0), c);
            Console.WriteLine(samples.Count());
        }

        private Dictionary<string, List<data.Models.Equipment>> _findEnableEquipment()
        {
            List<data.Models.Equipment> equipList = new List<data.Models.Equipment>();
            var map = new Dictionary<string, List<WattApp.data.Models.Equipment>>();

            var q =
                    from c in _wattAppDB.Equipment
                    join p in _wattAppDB.Customers on c.Customer equals p
                    where c.Customer.Enabled == true
                    select new { Equipment = c, CustomerGuid = p.Guid};

            foreach (var item in q)
            {
                if (!map.ContainsKey(item.CustomerGuid))
                {
                    var l = new List<data.Models.Equipment>();
                    l.Add(item.Equipment);
                    map.Add(item.CustomerGuid, l);
                }
                else
                    map[item.CustomerGuid].Add(item.Equipment);
            }
            return map;
        }
    }
}
