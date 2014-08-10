using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WattApp.data.Repositories;

namespace WattApp.data.Models
{
    public class DataHelpers
    {
        public const int kDefaultEndDayHour = 23;
        public const int kDefaultEndDayMinute = 59;
        public const int kDefaultEndDaySeconds = 59;

        public static Dictionary<string, List<data.Models.Equipment>> FindEnabledEquipment(IDataRepository dataRep, string customerGuid = null)
        {
            List<data.Models.Equipment> equipList = new List<data.Models.Equipment>();
            var map = new Dictionary<string, List<WattApp.data.Models.Equipment>>();

            var q =
                    from c in dataRep.Equipment
                    join p in dataRep.Customers on c.Customer equals p
                    where c.Customer.Enabled == true
                    select new { Equipment = c, CustomerGuid = p.Guid };

            if (!string.IsNullOrEmpty(customerGuid))
                q = q.Where(c => c.CustomerGuid == customerGuid);

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

        public static void AddDailyPeakDemand(IDataRepository dataRep, int pointId, double value, DateTime day)
        {
            var sample = new Sample() { SampleType = SampleType.DailyPeakDemand, PointId = pointId, Value = value };
            DateTime t = new DateTime(day.Year, day.Month, day.Day, kDefaultEndDayHour, kDefaultEndDayMinute, kDefaultEndDaySeconds);
            sample.TimeStamp = t;
            dataRep.Insert(sample);
        }
        public static void AddDailyConsumption(IDataRepository dataRep, int pointId, double value, DateTime day)
        {
            var sample = new Sample() { SampleType = SampleType.DailyConsumption, PointId = pointId, Value = value };
            DateTime t = new DateTime(day.Year, day.Month, day.Day, kDefaultEndDayHour, kDefaultEndDayMinute, kDefaultEndDaySeconds);
            sample.TimeStamp = t;
            dataRep.Insert(sample);
        }
    }
}
