using BuildingApi;
using Microsoft.WindowsAzure;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using Flurl;
using System.Threading;
using System.Diagnostics;
using WattApp.data.Webjobs;
using WattApp.data.Repositories;
using WattApp.data.Models;
using WattApp.WebJobs.API;

namespace WattApp.WebJobs
{
    class Program
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            Stopwatch stopWatch = new Stopwatch();
            IDataRepository dataRep = new DataRepository(new WattAppContext());

            stopWatch.Start();
            try
            {
                // TO DO Simplify initialization
                var apiClient = _initAPIClient();

                var taskList = new List<ITask>();
                taskList.Add(new RetrieveDemandTimeSeriesTask(_logger, dataRep, apiClient));
                taskList.Add(new DemandRollupAndUpdatesTask(_logger, dataRep));

                foreach (var item in taskList)
                    item.Execute();

                // CRAWLER Section
                //var crawler = new ConfigurationCrawler(_logger, _tokenProvider, _baseAPIRoot);
                
                // Customers
                //crawler.DiscoveryAndUpdateCustomersTask();

                // Meters
                //crawler.DiscoveryAndUpdateMetersTask();

                // Time Series Section
                // TO DO Refactor: Common assembly + single exe per webjob

                // UNCOMMENT BEFORE DEPLOY WEBJOB
                //TimeSeriesManager timeseriesManager = new TimeSeriesManager(_logger, _tokenProvider, _baseAPIRoot);
                //var enabledEquipmentByCustomerMap = timeseriesManager.FindEnabledEquipment();
                //timeseriesManager.PullTimeSeriesTask(enabledEquipmentByCustomerMap);
                //timeseriesManager.ExecuteRollupAndUpdatesTask(enabledEquipmentByCustomerMap);
            }
            catch (Exception e)
            {
                _logger.Error("WebJob -> Unhandle Exception ", e);
            }
            string str = string.Format("DemandTimeSeriesUpdates Elapsed={0} ms", stopWatch.ElapsedMilliseconds);
            _logger.Info(str);
            Thread.Sleep(2000);
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

            var tokenProvider = new TokenClient(clientId, clientSecret, tokenEndpoint, WebProxy.GetDefaultProxy());
            return new APIClient(tokenProvider, baseAPIRoot);
        }
    }

}
