namespace WattApp.data.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using WattApp.data.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<WattApp.data.Models.WattAppContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(WattApp.data.Models.WattAppContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            context.Customers.AddOrUpdate(
              p => p.Name,
              new Customer { Id = 1, Name = "JCI", Guid = "123" },
              new Customer { Id = 2, Name = "ACME Red", Guid = "123" },
              new Customer { Id = 3, Name = "Green Company", Guid = "7iULAhT9vUuLr9A8r2Eb5g" }
            );

            Customer GreenCompany = context.Customers.Single(c => c.Id == 3);

            context.Equipment.AddOrUpdate(
              p => p.Name,
                new Equipment { Name = "Technology Center Consumption Electric Meter", Type = "EletricMeter", Location = "Technology Center", PxGuid = "ynfPDQ_Lj06a-5UM2RvJjw", Customer = GreenCompany },
                new Equipment { Name = "Corporate Building Electric Meter", Type = "EletricMeter", Location = "Corporate Building", PxGuid = "OFSM3USrzkST1vxFUruang", Customer = GreenCompany }
            );
            
        }
    }
}
