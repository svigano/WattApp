using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WattApp.data.Models;
using WattApp.data.Repositories;

namespace WattApp.data.Webjobs
{
    public class DemandRollupAndUpdatesTask : Task
    {
        public DemandRollupAndUpdatesTask(Logger logger, IDataRepository rep)
            : base(logger, rep, "DemandRollupAndUpdates")
        {
        }

        public override bool DoWork()
        {
            var numOfUpdatedEquipment = 0;
            var enabledEquipmentByCustomerMap = DataHelpers.FindEnabledEquipment(_dataRep);

            // Update All the equipment last value demand and calculate the new deltademand value
            foreach (var key in enabledEquipmentByCustomerMap.Keys)
            {
                foreach (var equip in enabledEquipmentByCustomerMap[key])
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
                                var delta = Math.Round((lastSample.Value / yesterdaySample.Value - 1) * 100, 1);
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

            return true;
        }

    }
}
