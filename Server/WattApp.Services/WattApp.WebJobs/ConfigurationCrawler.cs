using BuildingApi;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WattApp.data.Models;
using WattApp.data.Repositories;
using WattApp.WebJobs.API;
using Flurl;
using Microsoft.WindowsAzure;

namespace WattApp.WebJobs
{
    public class ConfigurationCrawler
    {
        private static Logger _logger;
        private readonly string _baseAPIRoot = string.Empty;
        private readonly ITokenProvider _tokenProvider;
        private readonly EquipmentClient _equipmentClient;
        private readonly IDataRepository _dataRep = null;
        private Stopwatch _stopWatch = new Stopwatch();
        private string[] _temporaryJCIMeterName = { "Building 36 - Electric Meter", "CTU - Electric Demand Meter", "Electric Meter" };

        public ConfigurationCrawler(Logger logger, ITokenProvider provider, string baseRootAPI)
        {
            _logger = logger;
            _tokenProvider = provider;
            _baseAPIRoot = baseRootAPI;
            _equipmentClient = new EquipmentClient(_tokenProvider, _baseAPIRoot);
            _dataRep = new DataRepository(new WattAppContext());
        }

        public void DiscoveryAndUpdateCustomersTask()
        {
            _logger.Info("****DiscoveryAndUpdateCustomersTask****");
            _stopWatch.Restart();
            var addedCustomers = new List<Customer>();
            try
            {
                // Get latest list of customers defined in Px
                var token = _tokenProvider.Get();
                IEnumerable<Company> companies = HttpHelper.Get<Company[]>(_baseAPIRoot.AppendPathSegment("companies").ToString(), token);

                // Get the list of customers discovered
                if (companies.Count() != _dataRep.Customers.Count())
                {
                    foreach (var item in companies)
                    {
                        if (_dataRep.Customers.SingleOrDefault(c => c.Guid == item.Id) == null)
                        { 
                            // Add the customer to the dscovered list
                            // Disabled by default
                            // TO DO Auto exclude TEST/Fake Customer
                            addedCustomers.Add(new Customer { Name = item.Name, Guid = item.Id, Enabled = false });
                        }
                    }
                }
                _dataRep.Insert(addedCustomers);
            }
            catch (Exception e)
            {
                _logger.Error("DiscoveryAndUpdateCustomersTask->Unhandle Exception ", e);
                Trace.WriteLine("DiscoveryAndUpdateCustomersTask -> Unhandle Exception " + e.Message);
            }
            _logger.Info(string.Format("Added {0} new cusotmers ", addedCustomers.Count()));
            _logger.Info(string.Format("DiscoveryAndUpdateCustomersTask executed in (ms) {0}", _stopWatch.ElapsedMilliseconds));
        }

        public void DiscoveryAndUpdateMetersTask()
        {
            _logger.Info("****DiscoveryAndUpdateMetersTask****");
            _stopWatch.Restart();
            var addedMeters = new List<WattApp.data.Models.Equipment>();
            try
            {
                string meterType = CloudConfigurationManager.GetSetting("DISCOVERY_ELETRIC_METER");
                string demandType = CloudConfigurationManager.GetSetting("DISCOVERY_SUPPORTED_DEMAND_TYPE");
                var enabledCustomer = _dataRep.Customers.Where(c => c.Enabled == true);
                Console.WriteLine("Enabled Customers " + enabledCustomer.Count());

                // Dev Only one Customer enabled
                if (enabledCustomer.Count() > 0)
                {
                    var customer = enabledCustomer.SingleOrDefault();
                    var company = new BuildingApi.Company() { Id = customer.Guid };
                    var eletricMeters = _equipmentClient.GetEquipmentAndPointRoles(meterType, company);
                    Console.WriteLine("Found Meters " + eletricMeters.Count());
                    foreach (var item in eletricMeters)
                    {
                        if (item.PointRoles.Items.Where(p => p.Type.Id == demandType).Count() > 0)
                        {
                            if (_temporaryJCIMeterName.Contains(item.Name))
                            {
                                // Check if the equipment has been discovered
                                if (customer.EquipmentList.Where(e => e.PxGuid == item.Id).Count() == 0)
                                {
                                    var equip = new WattApp.data.Models.Equipment() { Name = item.Name, Type = item.Type.Id, PxGuid = item.Id, Location = "TBD", CustomerId = customer.Id };
                                    var ptInfo = item.PointRoles.Items.First(p => p.Type.Id == demandType);
                                    var point = new WattApp.data.Models.Point() { Name = ptInfo.Point.Name, Type = ptInfo.Type.Id, PxGuid = ptInfo.Point.Id, Enabled = true };
                                    equip.PointsList.Add(point);
                                    addedMeters.Add(equip);
                                }
                            }
                        }
                    }
                }

                // Add new meters
                if (addedMeters.Count() > 0)
                    _dataRep.Insert(addedMeters);

            }
            catch (Exception e)
            {
                _logger.Error("DiscoveryAndUpdateMetersTask->Unhandle Exception ", e);
                Trace.WriteLine("DiscoveryAndUpdateMetersTask -> Unhandle Exception " + e.Message);
            }
            _logger.Info(string.Format("Added {0} new meters ", addedMeters.Count()));
            _logger.Info(string.Format("DiscoveryAndUpdateMetersTask executed in (ms) {0}", _stopWatch.ElapsedMilliseconds));
        }
    }
}
