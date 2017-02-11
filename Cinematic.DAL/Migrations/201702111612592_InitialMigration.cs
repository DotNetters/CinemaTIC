namespace Cinematic.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Seats",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Row = c.Int(nullable: false),
                        SeatNumber = c.Int(nullable: false),
                        Reserved = c.Boolean(nullable: false),
                        Session_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sessions", t => t.Session_Id)
                .Index(t => t.Session_Id);
            
            CreateTable(
                "dbo.Sessions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TimeAndDate = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Tickets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Price = c.Double(nullable: false),
                        TimeAndDate = c.DateTime(nullable: false),
                        Seat_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Seats", t => t.Seat_Id)
                .Index(t => t.Seat_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tickets", "Seat_Id", "dbo.Seats");
            DropForeignKey("dbo.Seats", "Session_Id", "dbo.Sessions");
            DropIndex("dbo.Tickets", new[] { "Seat_Id" });
            DropIndex("dbo.Seats", new[] { "Session_Id" });
            DropTable("dbo.Tickets");
            DropTable("dbo.Sessions");
            DropTable("dbo.Seats");
        }
    }
}
