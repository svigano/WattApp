namespace WattApp.data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class t1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Guid = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Equipments",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Type = c.String(),
                        Location = c.String(),
                        PxGuid = c.String(),
                        Customer_Id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Customers", t => t.Customer_Id)
                .Index(t => t.Customer_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Equipments", "Customer_Id", "dbo.Customers");
            DropIndex("dbo.Equipments", new[] { "Customer_Id" });
            DropTable("dbo.Equipments");
            DropTable("dbo.Customers");
        }
    }
}
