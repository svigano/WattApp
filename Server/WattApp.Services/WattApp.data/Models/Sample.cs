using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WattApp.data.Models
{
    public enum SampleType
    {
        Demand = 1,
        PeakDemand = 2,
        DailyPeakDemand = 3,
        WeeklyAverage = 4,
        DailyConsumption = 5
    }

    public class Sample
    {
        public int id { get; set; }
        public DateTime TimeStamp { get; set; }
        public double Value { get; set; }
        public int PointId { get; set; }
        public virtual Point Point { get; set; }
        [Range(1, int.MaxValue)]
        public SampleType SampleType { get; set; }
    }
}
