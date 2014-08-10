using BuildingApi;
using Microsoft.Azure.Jobs;
using Microsoft.WindowsAzure;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WattApp.data.Models;
using WattApp.data.Repositories;
using WattApp.data.Webjobs;
using WattApp.WebJobs.API;

namespace WattApp.Webjob.Config
{
    class Program
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            var host = new JobHost();
            host.RunAndBlock();
            //DiscoveryAndUpdateMeters(new CustomerDiscoverQueueInfo() { DiscoveryOption = DiscoveryOption.eMeterAndSamples, Guid = "950BRrQp5Ea2O-CJNdVV3A" });
        }

        public static void DiscoveryAndUpdateMeters([QueueTrigger("wattappconfigrequest")] CustomerDiscoverQueueInfo info)
        {
            _logger.Debug("DiscoverCustomer has been triggered on " + info.Guid);
            IDataRepository dataRep = new DataRepository(new WattAppContext());
            var apiclient = _initAPIClient(); 
            var taskList = new List<ITask>();

            try
            {
                if (info.DiscoveryOption == DiscoveryOption.eMeterAndSamples)
                {
                    taskList.Add(new DiscoveryAndUpdateMeters(_logger, dataRep, apiclient, info.Guid));
                    taskList.Add(new RetrieveDemandTimeSeriesTask(_logger, dataRep, apiclient, info.Guid));
                    taskList.Add(new DemandRollupAndUpdatesTask(_logger, dataRep, info.Guid));
                    taskList.Add(new DailyConsumptionTask(_logger, dataRep, info.Guid));
                    taskList.Add(new DailyPeaksTask(_logger, dataRep, info.Guid));
                }

                foreach (var item in taskList)
                    item.Execute();
            }
            catch (Exception e)
            {
                _logger.Error("WebJob -> Unhandle Exception ", e);
            }
        }

        private static APIClient _initAPIClient()
        {
            var clientId = CloudConfigurationManager.GetSetting("JciClientId");
            var clientSecret = CloudConfigurationManager.GetSetting("JciClientSecret");
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                throw new ArgumentException("JciClientId and/or JciClientSecret is not valid");

            var tokenEndpoint = CloudConfigurationManager.GetSetting("JciTokenEndpoint");
            var baseAPIRoot = CloudConfigurationManager.GetSetting("JciBuildingApiEndpoint");

            if (string.IsNullOrEmpty(tokenEndpoint) || string.IsNullOrEmpty(baseAPIRoot))
                throw new ArgumentException("API base Urls are not valid");

            var tokenProvider = new TokenClient(clientId, clientSecret, tokenEndpoint, System.Net.WebProxy.GetDefaultProxy());
            return new APIClient(tokenProvider, baseAPIRoot);
        }

    }
}
