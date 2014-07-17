using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WattApp.api.Controllers
{
    public class DoubleReading
    {
        public DateTime t { get; set; }
        public double val1 { get; set; }
        public double val2 { get; set; }
    }

    public class DailyConsumption
    {
        public DateTime t { get; set; }
        public int val { get; set; }
    }

    public class MockMeterHistoryController : ApiController
    {
        public IEnumerable<DoubleReading> GetTodayVsYesterday()
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
            d = new DoubleReading { t = new DateTime(2014, 5, 23, 10, 0, 0), val1 =12, val2 = 13 };
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

        [Route("api/consumption")]
        public IEnumerable<DailyConsumption> GetWeeklyConsumption() 
        {
            var weeklyData = new List<DailyConsumption>();
            weeklyData.Add(new DailyConsumption() { t = new DateTime(2014, 5, 23), val = 550 });
            weeklyData.Add(new DailyConsumption() { t = new DateTime(2014, 5, 24), val = 590 });
            weeklyData.Add(new DailyConsumption() { t = new DateTime(2014, 5, 25), val = 400 });
            weeklyData.Add(new DailyConsumption() { t = new DateTime(2014, 5, 26), val = 430 });
            weeklyData.Add(new DailyConsumption() { t = new DateTime(2014, 5, 27), val = 600 });
            weeklyData.Add(new DailyConsumption() { t = new DateTime(2014, 5, 28), val = 520 });
            weeklyData.Add(new DailyConsumption() { t = new DateTime(2014, 5, 29), val = 480 });
            
            return weeklyData; 
        }

    }
}
