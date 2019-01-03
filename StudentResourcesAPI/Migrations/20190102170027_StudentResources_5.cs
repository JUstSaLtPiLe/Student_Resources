using Microsoft.EntityFrameworkCore.Migrations;

namespace StudentResourcesAPI.Migrations
{
    public partial class StudentResources_5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Grade",
                table: "Grade");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Grade_AccountId_SubjectId",
                table: "Grade",
                columns: new[] { "AccountId", "SubjectId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Grade",
                table: "Grade",
                columns: new[] { "AccountId", "SubjectId", "GradeType" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Grade_AccountId_SubjectId",
                table: "Grade");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Grade",
                table: "Grade");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Grade",
                table: "Grade",
                columns: new[] { "AccountId", "SubjectId" });
        }
    }
}
