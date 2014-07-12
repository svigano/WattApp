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

            context.Customers.AddOrUpdate(
  p => p.Name,
  new Customer { Name = "ACME Red", Guid = "123", Enabled = false }
);

            Customer GreenCompany = new Customer { Name = "Green Company", Guid = "7iULAhT9vUuLr9A8r2Eb5g", Enabled = true };
            Customer JCI = new Customer { Name = "JCI", Guid = "123", Enabled = false };
            Point pt1 = new Point { Name = "IntervalDemand", Type = "IntervalDemand", Enabled = true, PxGuid = "Ghk1guf-IUqoZmS3C87UkA" };
            Point pt2 = new Point { Name = "IntervalDemand", Type = "IntervalDemand", Enabled = true, PxGuid = "c7Iw7gKzuUCqlaIwewVRPg" };

            context.Equipment.AddOrUpdate(
              p => p.Name,
                new Equipment { Name = "Consumption Electric Meter", Type = "EletricMeter", Location = "Technology Center", PxGuid = "ynfPDQ_Lj06a-5UM2RvJjw", Customer = GreenCompany, Points = new List<Point> { pt1 } },
                new Equipment { Name = "Test JCIT", Type = "EletricMeter", Location = "JCI", PxGuid = "123dewfefwfew", Customer = JCI },
                new Equipment { Name = "Electric Meter", Type = "EletricMeter", Location = "Corporate Building", PxGuid = "OFSM3USrzkST1vxFUruang", Customer = GreenCompany, Points = new List<Point> { pt2 } }
            );

            context.Samples.Add(
                new Sample { Point = pt1, TimeStamp = DateTime.Now, Value = 230 }
            );

        }
    }
}
