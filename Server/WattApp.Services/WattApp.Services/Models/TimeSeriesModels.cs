using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WattApp.api.Models
{
    public class DoubleReading
    {
        public DateTime t { get; set; }
        public double val1 { get; set; }
        public double val2 { get; set; }
    }

    public class SampleModel
    {
        public DateTime t { get; set; }
        public int val { get; set; }
    }

    public class WeeklyPeaksModel
    { 
        public int YearToDatePeak { get; set; }
        public int AverageWeeklyDemand { get; set; }
        public IEnumerable<SampleModel> WeeklyPeaks { get; set; }
    }
}