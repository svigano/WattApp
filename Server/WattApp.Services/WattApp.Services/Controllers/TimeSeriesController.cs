using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WattApp.data.Models;
using WattApp.data.Repositories;

namespace WattApp.api.Controllers
{
    public class TimeSeriesController : ApiController
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private Stopwatch _stopwatch = new Stopwatch();
        private IDataRepository _dataRep = null;

        public TimeSeriesController()
        {
            _dataRep = new DataRepository(new WattAppContext());
        }

        [Route("api/customer/{customerGuid}/Dashboard/{ptID}/DemandVsYesterday")]
        public IEnumerable<DoubleReading> GetTodayVsYesterday(string customerGuid, int ptID)
        {
            var readings = new List<DoubleReading>();
            // TEMPORARY 
            // MOCK DATA
            if (customerGuid == "123mock123")
                readings = _mockDemandVsYesterdayData();
            else
            {
                var startTime = DateTime.UtcNow.Subtract(TimeSpan.FromHours(24));
                var todaySamples = _dataRep.Samples.Where(s => s.PointId == ptID && s.TimeStamp > startTime && s.TimeStamp.Minute == 0).ToList();
                startTime = DateTime.UtcNow.Subtract(TimeSpan.FromHours(48));
                var endTime = startTime.AddHours(24);
                var yesterdaySamples = _dataRep.Samples.Where(s => s.PointId == ptID && s.TimeStamp > startTime && s.TimeStamp < endTime && s.TimeStamp.Minute == 0).ToList();

                // # of samples line up
                // Simply combine the value sequentially
                if (todaySamples.Count == yesterdaySamples.Count)
                    for (int i = 0; i < todaySamples.Count(); i++)
                        readings.Add(new DoubleReading() { t = todaySamples[i].TimeStamp, val1 = todaySamples[i].Value, val2 = yesterdaySamples[i].Value });
                else // gaps in data
                {
                    _logger.Warn(string.Format("GetTodayVsYesterday -> different # of samples: Today {0} yesterday {1}", todaySamples.Count, yesterdaySamples.Count));
                    // TO be improved
                    // Need to align time
                    if (yesterdaySamples.Count > todaySamples.Count)
                        for (int i = 0; i < todaySamples.Count(); i++)
                            readings.Add(new DoubleReading() { t = todaySamples[i].TimeStamp, val1 = todaySamples[i].Value, val2 = yesterdaySamples[i].Value });
                }
            }
            return readings;
        }

        [Route("api/customer/{customerGuid}/Dashboard/{ptID}/demandAndWeather")]
        public IEnumerable<DoubleReading> GetDemandAndWeather(string customerGuid, int ptID)
        {
            var readings = new List<DoubleReading>();
            // TEMPORARY 
            // MOCK DATA
            //if (customerGuid == "123mock123")
            readings = _mockDemandAndWeatherData();
            return readings;
        }

        [Route("api/customer/{customerGuid}/Dashboard/{ptID}/lastweekConsumption")]
        public IEnumerable<DailyConsumption> GetLastweekConsumption(string customerGuid, int ptID)
        {
            var weeklyData = new List<DailyConsumption>();

            // TEMPORARY 
            // MOCK DATA
            if (customerGuid == "123mock123")
                weeklyData = _mockConsumptionData();
            else
            {
                var startTime = DateTime.Today.Subtract(TimeSpan.FromDays(7));
                var last7ConsumptionSamples = from s in _dataRep.Samples
                                              where s.PointId == ptID &&
                                                    s.SampleType == SampleType.DailyConsumption &&
                                                    s.TimeStamp > startTime
                                              select s;
                foreach (var item in last7ConsumptionSamples)
                    weeklyData.Add(_mapSampleToDailyConsumption(item));
            }
            
            return weeklyData; 
        }

        // TO DO
        // Evaluate to use AuotMapper in the future
        private DailyConsumption _mapSampleToDailyConsumption(Sample s)
        {
            return new DailyConsumption() { t = s.TimeStamp, val = (int)s.Value };
        }

        private List<DoubleReading> _mockDemandVsYesterdayData()
        {
            var samples = new List<DoubleReading>();
            var startDate = new DateTime(2014, 5, 23, 0, 0, 0);

            var d = new DoubleReading { t = new DateTime(2014, 5, 23, 0, 0, 0), val1 = 19, val2 = 18 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 1, 0, 0), val1 = 23, val2 = 17 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 2, 0, 0), val1 = 24, val2 = 19 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 3, 0, 0), val1 = 23, val2 = 24 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 4, 0, 0), val1 = 20, val2 = 25 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 5, 0, 0), val1 = 19, val2 = 24 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 6, 0, 0), val1 = 16, val2 = 24 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 7, 0, 0), val1 = 16, val2 = 20 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 8, 0, 0), val1 = 16, val2 = 14 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 9, 0, 0), val1 = 12, val2 = 12 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 10, 0, 0), val1 = 12, val2 = 13 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 11, 0, 0), val1 = 16, val2 = 13 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 12, 0, 0), val1 = 18, val2 = 16 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 13, 0, 0), val1 = 18, val2 = 24 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 14, 0, 0), val1 = 23, val2 = 25 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 15, 0, 0), val1 = 20, val2 = 24 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 16, 0, 0), val1 = 19, val2 = 24 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 17, 0, 0), val1 = 16, val2 = 20 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 18, 0, 0), val1 = 16, val2 = 14 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 19, 0, 0), val1 = 16, val2 = 12 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 20, 0, 0), val1 = 12, val2 = 13 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 21, 0, 0), val1 = 12, val2 = 13 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 22, 0, 0), val1 = 16, val2 = 16 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 23, 0, 0), val1 = 18, val2 = 16 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 0, 0, 0), val1 = 18, val2 = 18 };
            samples.Add(d);

            return samples;
        }
        private List<DoubleReading> _mockDemandAndWeatherData()
        {
            var samples = new List<DoubleReading>();
            var startDate = new DateTime(2014, 5, 23, 0, 0, 0);

            var d = new DoubleReading { t = new DateTime(2014, 5, 23, 0, 0, 0), val1 = 190, val2 = 55 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 1, 0, 0), val1 = 230, val2 = 55 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 2, 0, 0), val1 = 240, val2 = 54 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 3, 0, 0), val1 = 230, val2 = 54 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 4, 0, 0), val1 = 200, val2 = 55 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 5, 0, 0), val1 = 190, val2 = 56 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 6, 0, 0), val1 = 160, val2 = 56 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 7, 0, 0), val1 = 160, val2 = 60 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 8, 0, 0), val1 = 160, val2 = 61 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 9, 0, 0), val1 = 120, val2 = 63 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 10, 0, 0), val1 = 120, val2 = 63 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 11, 0, 0), val1 = 160, val2 = 65 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 12, 0, 0), val1 = 180, val2 = 68 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 13, 0, 0), val1 = 180, val2 = 70 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 14, 0, 0), val1 = 230, val2 = 70 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 15, 0, 0), val1 = 200, val2 = 71 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 16, 0, 0), val1 = 190, val2 = 70 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 17, 0, 0), val1 = 160, val2 = 70 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 18, 0, 0), val1 = 160, val2 = 70 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 19, 0, 0), val1 = 160, val2 = 69 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 20, 0, 0), val1 = 120, val2 = 68 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 21, 0, 0), val1 = 120, val2 = 66 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 22, 0, 0), val1 = 160, val2 = 60 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 23, 0, 0), val1 = 180, val2 = 59 };
            samples.Add(d);
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 0, 0, 0), val1 = 180, val2 = 58 };
            samples.Add(d);

            return samples;
        }

        private List<DailyConsumption> _mockConsumptionData()
        {
            var weeklyData = new List<DailyConsumption>();
            var today = DateTime.Today;
            weeklyData.Add(new DailyConsumption() { t = today.Subtract(TimeSpan.FromDays(6)), val = 550 });
            weeklyData.Add(new DailyConsumption() { t = today.Subtract(TimeSpan.FromDays(5)), val = 590 });
            weeklyData.Add(new DailyConsumption() { t = today.Subtract(TimeSpan.FromDays(4)), val = 400 });
            weeklyData.Add(new DailyConsumption() { t = today.Subtract(TimeSpan.FromDays(3)), val = 430 });
            weeklyData.Add(new DailyConsumption() { t = today.Subtract(TimeSpan.FromDays(2)), val = 600 });
            weeklyData.Add(new DailyConsumption() { t = today.Subtract(TimeSpan.FromDays(1)), val = 520 });
            weeklyData.Add(new DailyConsumption() { t = today, val = 480 });

            return weeklyData; 
        }
    }
}
