using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WattApp.data.Models
{
    public class Equipment
    {
        public Equipment()
        {
            PointsList = new List<Point>();
        }
        public int id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
        public string PxGuid { get; set; }
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual IList<Point> PointsList {get; set;}
    }

    public class Point 
    {
        public Point()
        {
            SamplesList = new List<Sample>();
        }
        public int id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string PxGuid { get; set; }
        public bool Enabled { get; set; }
        public int EquipmentId { get; set; }
        public virtual Equipment Equipment { get; set; }
        public virtual IList<Sample> SamplesList { get; set; }
    }
}
