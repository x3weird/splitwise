using Microsoft.EntityFrameworkCore.Migrations;

namespace Splitwise.DomainModel.Migrations
{
    public partial class signalRNew : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ConnectionId",
                table: "ExpenseNotifications",
                newName: "Email");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Email",
                table: "ExpenseNotifications",
                newName: "ConnectionId");
        }
    }
}
