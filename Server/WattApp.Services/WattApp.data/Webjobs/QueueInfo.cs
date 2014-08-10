using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WattApp.data.Webjobs
{
    public class CustomerQueueInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Guid { get; set; }
        public bool Enabled { get; set; }
    }
}
