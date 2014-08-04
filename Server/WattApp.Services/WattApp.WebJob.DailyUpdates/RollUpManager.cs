using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WattApp.data.Models;
using WattApp.data.Repositories;

namespace WattApp.WebJob.DailyUpdates
{
    class RollUpManager
    {
        private readonly Logger _logger;
        private readonly IDataRepository _dataRep;
        private Stopwatch _stopWatch = new Stopwatch();

        public RollUpManager(Logger logger, IDataRepository dataRep)
        {
            _logger = logger;
            _dataRep = dataRep;
        }

        public void CalculateDailyPeaks(Dictionary<string, List<data.Models.Equipment>> map, DateTime currentDay)
        {
            try
            {
                _stopWatch.Restart();
                foreach (var key in map.Keys)
                {
                    foreach (var equip in map[key])
                    {
                        if (equip.PointsList.Count() > 0)
                        {
                            DateTime startTime = currentDay.Subtract(TimeSpan.FromHours(24));
                            DateTime endTime = currentDay;
                            var pointID = equip.PointsList.FirstOrDefault().id;
                            var dailyPeakDemand = (from n in _dataRep.Samples
                                                   where n.PointId == pointID &&
                                                         n.SampleType == SampleType.Demand &&
                                                         n.TimeStamp >= startTime && 
                                                         n.TimeStamp < endTime
                                                   group n by n.PointId into g
                                                   select g.OrderByDescending(s => s.Value).FirstOrDefault()).FirstOrDefault();
                            DataHelpers.AddDailyPeakDemand(_dataRep, dailyPeakDemand.PointId, dailyPeakDemand.Value, currentDay);
                            string str = string.Format("equipment {0}, pointId {1}, val {2}, ts {3}", equip.Name, dailyPeakDemand.PointId, Math.Round(dailyPeakDemand.Value,2), dailyPeakDemand.TimeStamp);
                            _logger.Debug(str);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error("CalculateDailyPeaks->Unhandle Exception ", e);
                Trace.WriteLine("CalculateDailyPeaks -> Unhandle Exception " + e.Message);
            }
            _logger.Info(string.Format("CalculateDailyPeaks executed in (ms) {0}", _stopWatch.ElapsedMilliseconds));
        }

        public void CalculateDailyConsumption(Dictionary<string, List<data.Models.Equipment>> map, DateTime currentDay)
        {
            try
            {
                _stopWatch.Restart();
                foreach (var key in map.Keys)
                {
                    foreach (var equip in map[key])
                    {
                        if (equip.PointsList.Count() > 0 && equip.PointsList.FirstOrDefault().id ==22)
                        {
                            DateTime startTime = currentDay.Subtract(TimeSpan.FromHours(24));
                            DateTime endTime = currentDay;
                            var pointID = equip.PointsList.FirstOrDefault().id;
                            var dailyDemand = from n in _dataRep.Samples
                                              where n.PointId == pointID &&
                                                    n.SampleType == SampleType.Demand &&
                                                    n.TimeStamp > startTime &&
                                                    n.TimeStamp < endTime
                                              select n;
                            double totalDailyConsumption = 0;
                            foreach (var item in dailyDemand)
                            {
                                Console.WriteLine(string.Format("{0}-{1}",item.TimeStamp,item.Value));
                                totalDailyConsumption = item.Value + totalDailyConsumption;
                            }
                            Console.WriteLine("before split by 4 " + totalDailyConsumption);
                            totalDailyConsumption = Math.Round(totalDailyConsumption/4, 2);
                            Console.WriteLine(totalDailyConsumption);
                            DataHelpers.AddDailyConsumption(_dataRep, pointID, totalDailyConsumption, currentDay);
                            string str = string.Format("Equipment {0}, pointId {1}, val {2}", equip.Name, pointID, totalDailyConsumption);
                            _logger.Debug(str);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error("CalculateDailyConsumption->Unhandle Exception ", e);
                Trace.WriteLine("CalculateDailyConsumption -> Unhandle Exception " + e.Message);
            }
            _logger.Info(string.Format("CalculateDailyConsumption executed in (ms) {0}", _stopWatch.ElapsedMilliseconds));
        }
    }
}
