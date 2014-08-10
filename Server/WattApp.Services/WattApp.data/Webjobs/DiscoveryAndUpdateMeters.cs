using BuildingApi;
using Microsoft.WindowsAzure;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WattApp.data.Repositories;
using WattApp.WebJobs.API;

namespace WattApp.data.Webjobs
{
    public class DiscoveryAndUpdateMeters : Task
    {
        private readonly APIClient _apiClient;
        private readonly string _customerGuid;

        public DiscoveryAndUpdateMeters(Logger logger, IDataRepository rep, APIClient apiClient, string customerGuid)
            : base(logger, rep, "DiscoveryAndUpdateMeters")
        {
            _apiClient = apiClient;
            _customerGuid = customerGuid;
        }

        public override bool DoWork()
        {
            string meterType = CloudConfigurationManager.GetSetting("DISCOVERY_ELETRIC_METER");
            string[] supportedDemandTypes = CloudConfigurationManager.GetSetting("DISCOVERY_SUPPORTED_DEMAND_TYPE").Split(',');
            var enabledCustomer = _dataRep.Customers.Where(c => c.Guid == _customerGuid).SingleOrDefault();
            var addedMeters = new List<WattApp.data.Models.Equipment>();
            if (enabledCustomer != null)
            {
                var company = new BuildingApi.Company() { Id = _customerGuid};
                var eletricMeters = _apiClient.EquipmentClient.GetEquipmentAndPointRoles(meterType, company);
                foreach (var item in eletricMeters)
                {
                    if (item.PointRoles.Items.Where(p => p.Type.Id.Contains(CloudConfigurationManager.GetSetting("DISCOVERY_SUPPORTED_DEMAND"))).Count() > 0)
                    {
                        // Check if the equipment has been discovered
                        if (enabledCustomer.EquipmentList.Where(e => e.PxGuid == item.Id).Count() == 0)
                        {
                            var equip = new WattApp.data.Models.Equipment() { Name = item.Name, Type = item.Type.Id, PxGuid = item.Id, Location = "TBD", CustomerId = enabledCustomer.Id };
                            // Retrieve Point info
                            var ptInfo = item.PointRoles.Items.FirstOrDefault(p => p.Type.Id == supportedDemandTypes[0]);
                            if (ptInfo == null)
                                ptInfo = item.PointRoles.Items.FirstOrDefault(p => p.Type.Id == supportedDemandTypes[1]);
                            var point = new WattApp.data.Models.Point() { Name = ptInfo.Point.Name, Type = ptInfo.Type.Id, PxGuid = ptInfo.Point.Id, Enabled = true };
                            equip.PointsList.Add(point);

                            // Retrive Building / location info
                            var building = _apiClient.GetContainerBuilding(item.Location, company);
                            if (building != null)
                                equip.Location = building.Name;
                            addedMeters.Add(equip);
                        }
                    }
                }

                // Add new meters
                if (addedMeters.Count() > 0)
                    _dataRep.Insert(addedMeters);
                _logger.Info(string.Format("Added {0} new meters to customer={1}", addedMeters.Count(), _customerGuid));
            }

            return true;
        }
    }
}
