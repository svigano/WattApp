using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WattApp.data.Models
{
    public class WattAppDEVInitializerDropCreateDatabaseIfModelChanges : System.Data.Entity.DropCreateDatabaseIfModelChanges<WattAppContext>
    {
        protected override void Seed(WattApp.data.Models.WattAppContext context)
        {
            var customers = new List<Customer>()
            {
              new Customer { Name = "Green Company", Guid = "7iULAhT9vUuLr9A8r2Eb5g", Enabled = true }
            };
            var equip = new List<Equipment>()
            {
              new Equipment { Name = "Main Meter", Type = "EletricMeter", Location = "Technology Center", PxGuid = "ynfPDQ_Lj06a-5UM2RvJjw"},
              new Equipment { Name = "Corporate Electric Meter", Type = "EletricMeter", Location = "Corporate Building", PxGuid = "OFSM3USrzkST1vxFUruang"}
            };
            var points = new List<Point>()
            {
                new Point { Name = "IntervalDemand", Type = "IntervalDemand", Enabled = true, PxGuid = "Ghk1guf-IUqoZmS3C87UkA" },
                new Point { Name = "IntervalDemand", Type = "IntervalDemand", Enabled = true, PxGuid = "c7Iw7gKzuUCqlaIwewVRPg" }
            };
            customers[0].EquipmentList.Add(equip[0]);
            customers[0].EquipmentList.Add(equip[1]);
            equip[0].PointsList.Add(points[0]);
            equip[1].PointsList.Add(points[1]);

            customers.ForEach(s => context.Customers.Add(s));
            context.SaveChanges();
        }
    }

}
