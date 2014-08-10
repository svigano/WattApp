using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WattApp.api.Models
{
    public class CustomerModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Guid { get; set; }
        public bool Enabled { get; set; }
    }

    public class EquipmentModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string PxGuid { get; set; }
    }
}