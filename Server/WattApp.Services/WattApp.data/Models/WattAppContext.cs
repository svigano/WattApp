using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace WattApp.data.Models
{
    public class WattAppContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx

        public WattAppContext()
            : base("name=WattAppContext")
        {
           // Database.SetInitializer<WattAppContext>(new DropCreateDatabaseIfModelChanges<WattAppContext>());

            // To Enable to go Live
             Database.SetInitializer<WattAppContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // one-to-many
            // Customer -> Equipment
            modelBuilder.Entity<Customer>().HasMany<Equipment>(s => s.EquipmentList)
            .WithRequired(s => s.Customer).HasForeignKey(s => s.CustomerId);

            // one-to-many
            // Equipment -> Point
            modelBuilder.Entity<Equipment>().HasMany<Point>(s => s.PointsList)
            .WithRequired(s => s.Equipment).HasForeignKey(s => s.EquipmentId);

            // one-to-many
            // Point -> Sample
            modelBuilder.Entity<Point>().HasMany<Sample>(s => s.SamplesList)
            .WithRequired(s => s.Point).HasForeignKey(s => s.PointId);
        }

        public System.Data.Entity.DbSet<WattApp.data.Models.Customer> Customers { get; set; }
        public System.Data.Entity.DbSet<WattApp.data.Models.Equipment> Equipment { get; set; }
        public System.Data.Entity.DbSet<WattApp.data.Models.Point> Points { get; set; }
        public System.Data.Entity.DbSet<WattApp.data.Models.Sample> Samples { get; set; }
    }
}
