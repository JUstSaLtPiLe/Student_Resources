using Microsoft.EntityFrameworkCore.Migrations;

namespace StudentResourcesAPI.Migrations
{
    public partial class StudentResources_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Account");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Account",
                newName: "AccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "Account",
                newName: "Id");

            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "Account",
                nullable: false,
                defaultValue: 0);
        }
    }
}
