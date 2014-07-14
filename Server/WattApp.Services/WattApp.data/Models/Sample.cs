using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WattApp.data.Models
{
    public class Sample
    {
        public int id { get; set; }
        public DateTime TimeStamp { get; set; }
        public double Value { get; set; }
        public int PointId { get; set; }
        public virtual Point Point { get; set; }
    }
}
