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
    public class DailyPeaksTask : Task
    {
        public DailyPeaksTask(Logger logger, IDataRepository rep) : base(logger, rep, "CalculateDailyPeaks")
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
                            _calculateDailyPeakByPoint(pointID, today.Subtract(TimeSpan.FromDays(i)), equip);
                    }
                }
            }
            return true;
        }

        private void _calculateDailyPeakByPoint(int pointID, DateTime currentDay, Equipment equip)
        {
            DateTime startTime = currentDay.Subtract(TimeSpan.FromHours(24));
            DateTime endTime = currentDay;
            var dailyPeakDemand = (from n in _dataRep.Samples
                                   where n.PointId == pointID &&
                                         n.SampleType == SampleType.Demand &&
                                         n.TimeStamp >= startTime &&
                                         n.TimeStamp < endTime
                                   group n by n.PointId into g
                                   select g.OrderByDescending(s => s.Value).FirstOrDefault()).FirstOrDefault();
            if (dailyPeakDemand != null)
            {
                DataHelpers.AddDailyPeakDemand(_dataRep, dailyPeakDemand.PointId, dailyPeakDemand.Value, currentDay);
                string str = string.Format("equipment {0}, pointId {1}, val {2}, ts {3}", equip.Name, dailyPeakDemand.PointId, Math.Round(dailyPeakDemand.Value, 2), dailyPeakDemand.TimeStamp);
                _logger.Debug(str);
            }
            else
                _logger.Debug(string.Format("No peak/data found for equipment {0}, pointId {1}, starttime {2}, endtime {3}", equip.Name, pointID, startTime, endTime));
        }
    }
}
