using Microsoft.EntityFrameworkCore.Migrations;

namespace StudentResourcesAPI.Migrations
{
    public partial class InitStudentResources_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountId",
                table: "Subject",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subject_AccountId",
                table: "Subject",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subject_Account_AccountId",
                table: "Subject",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subject_Account_AccountId",
                table: "Subject");

            migrationBuilder.DropIndex(
                name: "IX_Subject_AccountId",
                table: "Subject");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Subject");
        }
    }
}
