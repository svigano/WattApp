using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WattApp.data.Models
{
    public class Equipment
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
        public string PxGuid { get; set; }
        public Customer Customer { get; set; }
        public virtual List<Point> Points {get; set;}
    }

    public class Point 
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string PxGuid { get; set; }
        public bool Enabled { get; set; }
    }
}
