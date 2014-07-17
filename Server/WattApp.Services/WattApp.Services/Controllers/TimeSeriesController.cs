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

        //[Route("api/customers/{customerGuid}/Dashboard/{itemId}/DemandVsYesterday")]
        [Route("api/DemandVsYesterday")]
        public IEnumerable<DoubleReading> GetTodayVsYesterday()
        {
            var readings = new List<DoubleReading>();
            var startTime = DateTime.UtcNow.Subtract(TimeSpan.FromHours(24));
            var todaySamples = _dataRep.Samples.Where(s => s.PointId == 1 && s.TimeStamp > startTime && s.TimeStamp.Minute == 0).ToList();
            startTime = DateTime.UtcNow.Subtract(TimeSpan.FromHours(48));
            var endTime = startTime.AddHours(24);
            var yesterdaySamples = _dataRep.Samples.Where(s => s.PointId == 1 && s.TimeStamp > startTime && s.TimeStamp < endTime && s.TimeStamp.Minute == 0).ToList();

            // # of samples line up
            // Simply combine the value sequentially
            if (todaySamples.Count == yesterdaySamples.Count)
                for (int i = 0; i < todaySamples.Count(); i++)
                    readings.Add(new DoubleReading() { t = todaySamples[i].TimeStamp, val1 = todaySamples[i].Value, val2 = yesterdaySamples[i].Value });
            else // gaps in data
            {
                _logger.Warn(string.Format("GetTodayVsYesterday -> different # of samples: Today {0} yesterday {1}", todaySamples.Count, yesterdaySamples.Count));
                // TO be improved
                // Temporary return only one time series
                for (int i = 0; i < todaySamples.Count(); i++)
                    readings.Add(new DoubleReading() { t = todaySamples[i].TimeStamp, val1 = todaySamples[i].Value, val2 = todaySamples[i].Value });
            }
            return readings;
        }

    }
}
