namespace WattApp.Services.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using WattApp.Services.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<WattApp.Services.Models.WattAppContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(WattApp.Services.Models.WattAppContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            context.Customers.AddOrUpdate(
              p => p.Name,
              new Customer { Id = 1, Name = "JCI", Guid ="123" },
              new Customer { Id = 2, Name = "ACME Red", Guid = "123" },
              new Customer { Id = 3, Name = "Yellow Pages", Guid = "123" }
            );
            
        }
    }
}
