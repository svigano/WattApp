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
        }
    }

}
