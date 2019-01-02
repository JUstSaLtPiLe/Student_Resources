using Microsoft.EntityFrameworkCore.Migrations;

namespace StudentResourcesAPI.Migrations
{
    public partial class StudentResources_4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentClazz_Account_AccountId1",
                table: "StudentClazz");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_StudentClazz_AccountId",
                table: "StudentClazz");

            migrationBuilder.DropIndex(
                name: "IX_StudentClazz_AccountId1",
                table: "StudentClazz");

            migrationBuilder.DropColumn(
                name: "AccountId1",
                table: "StudentClazz");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentClazz_Account_AccountId",
                table: "StudentClazz",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentClazz_Account_AccountId",
                table: "StudentClazz");

            migrationBuilder.AddColumn<int>(
                name: "AccountId1",
                table: "StudentClazz",
                nullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_StudentClazz_AccountId",
                table: "StudentClazz",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentClazz_AccountId1",
                table: "StudentClazz",
                column: "AccountId1");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentClazz_Account_AccountId1",
                table: "StudentClazz",
                column: "AccountId1",
                principalTable: "Account",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
