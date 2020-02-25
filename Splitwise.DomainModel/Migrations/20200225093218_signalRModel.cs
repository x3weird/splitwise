using Microsoft.EntityFrameworkCore.Migrations;

namespace Splitwise.DomainModel.Migrations
{
    public partial class signalRModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "NotificationHubs",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ExpenseNotifications",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Payload = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    Detail = table.Column<string>(nullable: true),
                    ConnectionId = table.Column<string>(nullable: true),
                    NotificationOn = table.Column<string>(nullable: true),
                    NotificationOnId = table.Column<string>(nullable: true),
                    Severity = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseNotifications", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpenseNotifications");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "NotificationHubs");
        }
    }
}
