using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WattApp.data.Models
{
    public class Customer
    {
        public Customer()
        {
            EquipmentList = new List<Equipment>();
        }
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Guid { get; set; }
        public bool Enabled { get; set; }
        public virtual IList<Equipment> EquipmentList { get; set; }
    }
}