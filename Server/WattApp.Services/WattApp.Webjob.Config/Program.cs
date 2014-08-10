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
            // var host = new JobHost();
            // host.RunAndBlock();

            //var info = new CustomerQueueInfo { Guid = "perxZsvOZ0SLYqTQQP_KUg" };
            //DiscoveryAndUpdateMeters(info);
            IDataRepository dataRep = new DataRepository(new WattAppContext());
            Console.WriteLine(dataRep.Points.Count());
            var equip = new WattApp.data.Models.Equipment() { id = 27 };
            dataRep.Delete(equip);
            Console.WriteLine(dataRep.Points.Count());
        }

        public static void DiscoveryAndUpdateMeters([QueueTrigger("wattappconfigrequest")] CustomerQueueInfo info)
        {
            _logger.Debug("DiscoverCustomer has been triggered on " + info.Guid);
            IDataRepository dataRep = new DataRepository(new WattAppContext());
            var apiclinet = _initAPIClient();
            try
            {
                var task = new DiscoveryAndUpdateMeters(_logger, dataRep, apiclinet, info.Guid);
                task.Execute();
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
