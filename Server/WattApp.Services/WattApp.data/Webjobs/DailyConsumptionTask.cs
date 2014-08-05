using Microsoft.WindowsAzure;
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
    public class DailyConsumptionTask : Task
    {
        public DailyConsumptionTask(Logger logger, IDataRepository rep)
            : base(logger, rep, "CalculateDailyConsumptionTask")
        {
        }

        public override bool DoWork()
        {
            var enabledEquipmentByCustomerMap = DataHelpers.FindEnabledEquipment(_dataRep);

            foreach (var key in enabledEquipmentByCustomerMap.Keys)
            {
                foreach (var equip in enabledEquipmentByCustomerMap[key])
                {
                    if (equip.PointsList.Count() > 0)
                    {
                        var pointID = equip.PointsList.FirstOrDefault().id;
                        // TO DO
                        // Just for now let's always calculate Daily peaks over the last 10 days
                        int kBufferDays = int.Parse(CloudConfigurationManager.GetSetting("DEFAULT_DAYS_BUFFERS_IN_DAYS"));
                        // No need to consider the time partion / UTC.
                        var today = DateTime.Today;
                        for (int i = kBufferDays; i > 0; i--)
                            _calculateDailyConsumptionByPoint(pointID, today.Subtract(TimeSpan.FromDays(i)), equip);
                    }
                }
            }

            return true;
        }

        private void _calculateDailyConsumptionByPoint(int pointID, DateTime currentDay, Equipment equip)
        {
            DateTime startTime = currentDay.Subtract(TimeSpan.FromHours(24));
            DateTime endTime = currentDay;
            var dailyDemand = from n in _dataRep.Samples
                              where n.PointId == pointID &&
                                    n.SampleType == SampleType.Demand &&
                                    n.TimeStamp > startTime &&
                                    n.TimeStamp < endTime
                              select n;

            double totalDailyConsumption = 0;
            // Remark IntervalDemand to Consumption:
            // Kw over 15min -> Sum (Kw/4) = KwH
            foreach (var item in dailyDemand)
                totalDailyConsumption = item.Value + totalDailyConsumption;
            totalDailyConsumption = Math.Round(totalDailyConsumption / 4, 2);

            DataHelpers.AddDailyConsumption(_dataRep, pointID, totalDailyConsumption, currentDay);
            string str = string.Format("Equipment {0}, pointId {1}, val {2}, day {3}", equip.Name, pointID, totalDailyConsumption, currentDay);
            _logger.Debug(str);
        }

    }
}
