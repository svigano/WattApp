using BuildingApi;
using Microsoft.WindowsAzure;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WattApp.data.Models;
using WattApp.data.Repositories;
using WattApp.WebJobs.API;

namespace WattApp.WebJobs
{
    internal class TimeSeriesManager
    {
        private const string kEletricMeter = "EletricMeter";

        private static Logger _logger;
        private string _baseAPIRoot = string.Empty;
        private ITokenProvider _tokenProvider;
        private EquipmentClient _equipmentClient;
        private SampleClient _sampleClient;
        private IDataRepository _dataRep = null;
        private Stopwatch _stopWatch = new Stopwatch();

        public TimeSeriesManager(Logger logger, ITokenProvider provider, string baseRootAPI)
        {
            _logger = logger;
            _tokenProvider = provider;
            _baseAPIRoot = baseRootAPI;
            _equipmentClient = new EquipmentClient(_tokenProvider, _baseAPIRoot);
            _sampleClient = new SampleClient(_tokenProvider, _baseAPIRoot);
            _dataRep = new DataRepository(new WattAppContext());
        }

        public Dictionary<string, List<data.Models.Equipment>> FindEnabledEquipment()
        {
            List<data.Models.Equipment> equipList = new List<data.Models.Equipment>();
            var map = new Dictionary<string, List<WattApp.data.Models.Equipment>>();

            var q =
                    from c in _dataRep.Equipment
                    join p in _dataRep.Customers on c.Customer equals p
                    where c.Customer.Enabled == true
                    select new { Equipment = c, CustomerGuid = p.Guid };


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

        public void PullTimeSeriesTask(Dictionary<string, List<data.Models.Equipment>> map)
        {
            _logger.Info("****PullTimeSeriesTask****");
            _stopWatch.Restart();
            try
            {
                if (map.Keys.Count == 0)
                    _logger.Info("No Customer/Equipment enabled");
                foreach (var key in map.Keys)
                {
                    _logger.Debug("CustomerGuid {0} # of Equipment {1}", key, map[key].Count);
                    var currentCompany = new Company { Id = key };
                    _pullTimeSeriesDataByCustomer(currentCompany, map[key]);
                }
            }
            catch (Exception e)
            {
                _logger.Error("PullTimeSeriesTask->Unhandle Exception ", e);
                Trace.WriteLine("WebJob -> Unhandle Exception " + e.Message);
            }
            _logger.Info(string.Format("PullTimeSeriesTask executed in (ms) {0}", _stopWatch.ElapsedMilliseconds));
        }

        public void ExecuteRollupAndUpdatesTask(Dictionary<string, List<data.Models.Equipment>> map) 
        {
            _logger.Info("****ExecuteRollupAndUpdatesTask****");
            var numOfUpdatedEquipment = 0;
            _stopWatch.Restart();
            try
            {
                // Update All the equipment last value demand and calculate the new deltademand value
                foreach (var key in map.Keys)
                {
                    foreach (var equip in map[key])
                    {
                        if (equip.PointsList.Count() > 0)
                        {
                            var lastSample = _dataRep.GetLastSampleByPoint(equip.PointsList.First().id, SampleType.Demand);

                            // New data, update demand and delta demand
                            if (lastSample.TimeStamp > equip.LastUpdateTime)
                            {
                                equip.LastUpdateTime = lastSample.TimeStamp;
                                equip.LastDemand = lastSample.Value;
                                equip.DeltaDemand = 0; // Assume not find the previous 24h sample
                                // get closest lastSample in the previous 24h
                                // TO DO set a tollerance 1h ??
                                var yesterdaySample = _dataRep.GetSampleByPoint(equip.PointsList.First().id, lastSample.TimeStamp.Subtract(TimeSpan.FromHours(24)), SampleType.Demand);
                                if (yesterdaySample != null)
                                {
                                    var delta = Math.Round((lastSample.Value/yesterdaySample.Value-1)*100,1);
                                    _logger.Debug(string.Format("Latest lastSample time {0} val: {1} closest 24h past lastSample time {2} val {3} delta {4}",
                                                  lastSample.TimeStamp, lastSample.Value, yesterdaySample.TimeStamp, yesterdaySample.Value, delta));
                                    equip.DeltaDemand = delta;
                                }
                                else
                                    _logger.Debug(string.Format("Latest lastSample time {0} val: {1} closest 24h past lastSample time not found",
                                                  lastSample.TimeStamp, lastSample.Value));
                                _dataRep.Update(equip);
                                numOfUpdatedEquipment++;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error("ExecuteRollupAndUpdatesTask->Unhandle Exception ", e);
                Trace.WriteLine("WebJob -> Unhandle Exception " + e.Message);
            }
            _logger.Info(string.Format("The task has updated {0} equipment", numOfUpdatedEquipment));
            _logger.Info(string.Format("ExecuteRollupAndUpdatesTask executed in (ms) {0}", _stopWatch.ElapsedMilliseconds));
        }

        private void _pullTimeSeriesDataByCustomer(Company c, IList<data.Models.Equipment> eqList)
        { 
            var ptIds = new List<string>();
            int kDefaultBackfillTime = int.Parse(CloudConfigurationManager.GetSetting("DEFAULT_BACKFILL_TIME_HOURS"));
            foreach (var equip in eqList)
            {
                _logger.Debug("Equipment {0} # of Points {1}", equip.Name, equip.PointsList.Count());
                foreach (var item in equip.PointsList)
                {
                    DateTime startTime = DateTime.UtcNow.Subtract(TimeSpan.FromHours(kDefaultBackfillTime));
                    DateTime endTime = DateTime.UtcNow;

                    // Check the latest lastSample available in wattapp system
                    var wlatestSample = _dataRep.GetLastSampleByPoint(item.id, SampleType.Demand);
                    // New point, let's bring back the default backfill data
                    if (wlatestSample == null)
                    {
                        // Store data in WattApp
                        // TO DO error handling
                        var pxsamples = _sampleClient.GetSamples(item.PxGuid, startTime, endTime, c);
                        var wsamples = _mapTowSample(item, pxsamples);
                        _dataRep.Insert(wsamples);
                        _logger.Info(string.Format("Initial BackFilled stored # {0} samples for the equipment {1} pt: {2}", wsamples.Count(), equip.Name, item.PxGuid));
                    }
                    else 
                    {
                        // Store data in WattApp
                        // TO DO error handling
                        startTime = wlatestSample.TimeStamp;
                        var pxsamples = _sampleClient.GetSamples(item.PxGuid, startTime, endTime, c);
                        var pxLatestBatchSample = pxsamples.OrderByDescending(t => t.Timestamp).FirstOrDefault();
                        if (pxLatestBatchSample != null && pxLatestBatchSample.Timestamp > wlatestSample.TimeStamp)
                        {
                            var sampleTobeInserted = new List<BuildingApi.Sample>(pxsamples);
                            sampleTobeInserted.RemoveAll(x => x.Timestamp <= wlatestSample.TimeStamp);
                            var wsamples = _mapTowSample(item, sampleTobeInserted);
                            _dataRep.Insert(wsamples);
                            _logger.Info(string.Format("Stored # {0} samples for the equipment {1} pt: {2}", wsamples.Count(), equip.Name, item.PxGuid));
                        }
                    }
                }
            }
        }

        private IEnumerable<WattApp.data.Models.Sample> _mapTowSample(data.Models.Point pt, IEnumerable<BuildingApi.Sample> pxsamples)
        {
            var wsamples = new List<WattApp.data.Models.Sample>();

            foreach (var item in pxsamples)
                wsamples.Add(new data.Models.Sample { SampleType = SampleType.Demand, Point = pt, Value = Math.Round(item.Value,2), TimeStamp = item.Timestamp });   

            return wsamples;
        }
    }
}
