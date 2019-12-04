using Microsoft.EntityFrameworkCore.Migrations;

namespace Splitwise.DomainModel.Migrations
{
    public partial class afternamechanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "userId",
                table: "Ledgers",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "expenseId",
                table: "Ledgers",
                newName: "ExpenseId");

            migrationBuilder.RenameColumn(
                name: "debitedAmount",
                table: "Ledgers",
                newName: "DebitedAmount");

            migrationBuilder.RenameColumn(
                name: "creditedAmount",
                table: "Ledgers",
                newName: "CreditedAmount");

            migrationBuilder.RenameColumn(
                name: "simplifyDebts",
                table: "Groups",
                newName: "SimplifyDebts");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Groups",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "isDeleted",
                table: "Groups",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "createdOn",
                table: "Groups",
                newName: "CreatedOn");

            migrationBuilder.RenameColumn(
                name: "addedBy",
                table: "Groups",
                newName: "AddedBy");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "GroupMembers",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "groupId",
                table: "GroupMembers",
                newName: "GroupId");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "Friends",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "friendId",
                table: "Friends",
                newName: "FriendId");

            migrationBuilder.RenameColumn(
                name: "note",
                table: "Expenses",
                newName: "Note");

            migrationBuilder.RenameColumn(
                name: "isDeleted",
                table: "Expenses",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "groupId",
                table: "Expenses",
                newName: "GroupId");

            migrationBuilder.RenameColumn(
                name: "expenseType",
                table: "Expenses",
                newName: "ExpenseType");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "Expenses",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "createdOn",
                table: "Expenses",
                newName: "CreatedOn");

            migrationBuilder.RenameColumn(
                name: "amount",
                table: "Expenses",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "addedBy",
                table: "Expenses",
                newName: "AddedBy");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "Comments",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "expenseId",
                table: "Comments",
                newName: "ExpenseId");

            migrationBuilder.RenameColumn(
                name: "createdOn",
                table: "Comments",
                newName: "CreatedOn");

            migrationBuilder.RenameColumn(
                name: "comment",
                table: "Comments",
                newName: "CommentData");

            migrationBuilder.RenameColumn(
                name: "LoginProvIder",
                table: "AspNetUserTokens",
                newName: "LoginProvider");

            migrationBuilder.RenameColumn(
                name: "lastName",
                table: "AspNetUsers",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "isRegistered",
                table: "AspNetUsers",
                newName: "IsRegistered");

            migrationBuilder.RenameColumn(
                name: "firstName",
                table: "AspNetUsers",
                newName: "FirstName");

            migrationBuilder.RenameColumn(
                name: "currency",
                table: "AspNetUsers",
                newName: "Currency");

            migrationBuilder.RenameColumn(
                name: "ProvIderDisplayName",
                table: "AspNetUserLogins",
                newName: "ProviderDisplayName");

            migrationBuilder.RenameColumn(
                name: "ProvIderKey",
                table: "AspNetUserLogins",
                newName: "ProviderKey");

            migrationBuilder.RenameColumn(
                name: "LoginProvIder",
                table: "AspNetUserLogins",
                newName: "LoginProvider");

            migrationBuilder.RenameColumn(
                name: "log",
                table: "ActivityUsers",
                newName: "Log");

            migrationBuilder.RenameColumn(
                name: "activityUserId",
                table: "ActivityUsers",
                newName: "ActivityUserId");

            migrationBuilder.RenameColumn(
                name: "log",
                table: "Activities",
                newName: "Log");

            migrationBuilder.RenameColumn(
                name: "activityOnId",
                table: "Activities",
                newName: "ActivityOnId");

            migrationBuilder.RenameColumn(
                name: "activityOn",
                table: "Activities",
                newName: "ActivityOn");

            migrationBuilder.AddColumn<string>(
                name: "ActivityId",
                table: "ActivityUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GroupExpenses",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    GroupId = table.Column<string>(nullable: true),
                    ExpenseId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupExpenses", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupExpenses");

            migrationBuilder.DropColumn(
                name: "ActivityId",
                table: "ActivityUsers");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Ledgers",
                newName: "userId");

            migrationBuilder.RenameColumn(
                name: "ExpenseId",
                table: "Ledgers",
                newName: "expenseId");

            migrationBuilder.RenameColumn(
                name: "DebitedAmount",
                table: "Ledgers",
                newName: "debitedAmount");

            migrationBuilder.RenameColumn(
                name: "CreditedAmount",
                table: "Ledgers",
                newName: "creditedAmount");

            migrationBuilder.RenameColumn(
                name: "SimplifyDebts",
                table: "Groups",
                newName: "simplifyDebts");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Groups",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Groups",
                newName: "isDeleted");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "Groups",
                newName: "createdOn");

            migrationBuilder.RenameColumn(
                name: "AddedBy",
                table: "Groups",
                newName: "addedBy");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "GroupMembers",
                newName: "userId");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "GroupMembers",
                newName: "groupId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Friends",
                newName: "userId");

            migrationBuilder.RenameColumn(
                name: "FriendId",
                table: "Friends",
                newName: "friendId");

            migrationBuilder.RenameColumn(
                name: "Note",
                table: "Expenses",
                newName: "note");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Expenses",
                newName: "isDeleted");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "Expenses",
                newName: "groupId");

            migrationBuilder.RenameColumn(
                name: "ExpenseType",
                table: "Expenses",
                newName: "expenseType");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Expenses",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "Expenses",
                newName: "createdOn");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Expenses",
                newName: "amount");

            migrationBuilder.RenameColumn(
                name: "AddedBy",
                table: "Expenses",
                newName: "addedBy");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Comments",
                newName: "userId");

            migrationBuilder.RenameColumn(
                name: "ExpenseId",
                table: "Comments",
                newName: "expenseId");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "Comments",
                newName: "createdOn");

            migrationBuilder.RenameColumn(
                name: "CommentData",
                table: "Comments",
                newName: "comment");

            migrationBuilder.RenameColumn(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                newName: "LoginProvIder");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "AspNetUsers",
                newName: "lastName");

            migrationBuilder.RenameColumn(
                name: "IsRegistered",
                table: "AspNetUsers",
                newName: "isRegistered");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "AspNetUsers",
                newName: "firstName");

            migrationBuilder.RenameColumn(
                name: "Currency",
                table: "AspNetUsers",
                newName: "currency");

            migrationBuilder.RenameColumn(
                name: "ProviderDisplayName",
                table: "AspNetUserLogins",
                newName: "ProvIderDisplayName");

            migrationBuilder.RenameColumn(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                newName: "ProvIderKey");

            migrationBuilder.RenameColumn(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                newName: "LoginProvIder");

            migrationBuilder.RenameColumn(
                name: "Log",
                table: "ActivityUsers",
                newName: "log");

            migrationBuilder.RenameColumn(
                name: "ActivityUserId",
                table: "ActivityUsers",
                newName: "activityUserId");

            migrationBuilder.RenameColumn(
                name: "Log",
                table: "Activities",
                newName: "log");

            migrationBuilder.RenameColumn(
                name: "ActivityOnId",
                table: "Activities",
                newName: "activityOnId");

            migrationBuilder.RenameColumn(
                name: "ActivityOn",
                table: "Activities",
                newName: "activityOn");
        }
    }
}
