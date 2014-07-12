using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WattApp.data.Models
{
    public class WattAppInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<WattAppContext>
    {
        protected override void Seed(WattApp.data.Models.WattAppContext context)
        {
            var customers = new List<Customer>()
            {
              new Customer { Id = 1, Name = "JCI", Guid = "123" },
              new Customer { Id = 2, Name = "ACME Red", Guid = "123" },
              new Customer { Id = 3, Name = "Green Company", Guid = "7iULAhT9vUuLr9A8r2Eb5g" }
            };
            customers.ForEach(s => context.Customers.Add(s));
            context.SaveChanges();

            var GreenCompany = context.Customers.Single(p => p.Guid == "7iULAhT9vUuLr9A8r2Eb5g");
            var equip = new List<Equipment>()
            {
              new Equipment { Name = "Technology Center Consumption Electric Meter", Type = "EletricMeter", Location = "Technology Center", PxGuid = "ynfPDQ_Lj06a-5UM2RvJjw", Customer = GreenCompany },
              new Equipment { Name = "Corporate Building Electric Meter", Type = "EletricMeter", Location = "Corporate Building", PxGuid = "OFSM3USrzkST1vxFUruang", Customer = GreenCompany }
            };
            equip.ForEach(s => context.Equipment.Add(s));
            context.SaveChanges();
        }

    }

}
