using Microsoft.EntityFrameworkCore.Migrations;

namespace Splitwise.DomainModel.Migrations
{
    public partial class signalRModelChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpenseNotifications");

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Payload = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    Detail = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    NotificationOn = table.Column<string>(nullable: true),
                    NotificationOnId = table.Column<string>(nullable: true),
                    Severity = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.CreateTable(
                name: "ExpenseNotifications",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Detail = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    NotificationOn = table.Column<string>(nullable: true),
                    NotificationOnId = table.Column<string>(nullable: true),
                    Payload = table.Column<string>(nullable: true),
                    Severity = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseNotifications", x => x.Id);
                });
        }
    }
}
