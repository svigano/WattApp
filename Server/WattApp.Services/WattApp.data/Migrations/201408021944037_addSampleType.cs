namespace WattApp.data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addSampleType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Samples", "SampleType", c => c.Int(nullable: false, defaultValue: 1));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Samples", "SampleType");
        }
    }
}
