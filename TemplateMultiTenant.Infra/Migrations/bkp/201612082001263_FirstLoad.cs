namespace TemplateMultiTenant.Infra.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FirstLoad : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 10, storeType: "nvarchar"),
                        Name = c.String(nullable: false, maxLength: 10, storeType: "nvarchar"),
                        Price = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Products");
        }
    }
}
