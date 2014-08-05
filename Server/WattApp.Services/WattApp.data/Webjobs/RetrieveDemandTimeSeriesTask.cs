using BuildingApi;
using Microsoft.WindowsAzure;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WattApp.data.Models;
using WattApp.data.Repositories;
using WattApp.WebJobs.API;

namespace WattApp.data.Webjobs
{
    public class RetrieveDemandTimeSeriesTask : Task
    {
        private readonly APIClient _apiClient;

        public RetrieveDemandTimeSeriesTask(Logger logger, IDataRepository rep, APIClient apiClient)
            : base(logger, rep, "RetrieveDemandTimeSeries")
        {
            _apiClient = apiClient;
        }

        public override bool DoWork()
        {
            var enabledEquipmentByCustomerMap = DataHelpers.FindEnabledEquipment(_dataRep);
            string customerGuid = string.Empty;
            // TO DO
            // Partial try and catch
            if (enabledEquipmentByCustomerMap.Keys.Count == 0)
                _logger.Warn("No Customer/Equipment enabled");
            foreach (var key in enabledEquipmentByCustomerMap.Keys)
            {
                customerGuid = key;
                _logger.Debug("CustomerGuid {0} # of Equipment {1}", key, enabledEquipmentByCustomerMap[key].Count);
                var currentCompany = new Company { Id = key };
                _pullTimeSeriesDataByCustomer(currentCompany, enabledEquipmentByCustomerMap[key]);
            }
            return true;
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
                        var pxsamples = _apiClient.GetSamples(item.PxGuid, startTime, endTime, c);
                        var wsamples = _mapTowSample(item, pxsamples);
                        _dataRep.Insert(wsamples);
                        _logger.Info(string.Format("Initial BackFilled stored # {0} samples for the equipment {1} pt: {2}", wsamples.Count(), equip.Name, item.PxGuid));
                    }
                    else
                    {
                        // Store data in WattApp
                        // TO DO error handling
                        startTime = wlatestSample.TimeStamp;
                        var pxsamples = _apiClient.GetSamples(item.PxGuid, startTime, endTime, c);
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
                wsamples.Add(new data.Models.Sample { SampleType = SampleType.Demand, Point = pt, Value = Math.Round(item.Value, 2), TimeStamp = item.Timestamp });

            return wsamples;
        }

    }
}
