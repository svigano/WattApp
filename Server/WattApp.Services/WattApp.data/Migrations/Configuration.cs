namespace WattApp.data.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using WattApp.data.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<WattApp.data.Models.WattAppContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(WattApp.data.Models.WattAppContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
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
