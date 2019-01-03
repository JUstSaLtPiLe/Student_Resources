using Microsoft.EntityFrameworkCore.Migrations;

namespace StudentResourcesAPI.Migrations
{
    public partial class StudentResources_7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mark",
                table: "Grade");

            migrationBuilder.RenameColumn(
                name: "GradeType",
                table: "Grade",
                newName: "TheoricalGradeStatus");

            migrationBuilder.AddColumn<int>(
                name: "AssignmentGrade",
                table: "Grade",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AssignmentGradeStatus",
                table: "Grade",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PraticalGrade",
                table: "Grade",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PraticalGradeStatus",
                table: "Grade",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TheoricalGrade",
                table: "Grade",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignmentGrade",
                table: "Grade");

            migrationBuilder.DropColumn(
                name: "AssignmentGradeStatus",
                table: "Grade");

            migrationBuilder.DropColumn(
                name: "PraticalGrade",
                table: "Grade");

            migrationBuilder.DropColumn(
                name: "PraticalGradeStatus",
                table: "Grade");

            migrationBuilder.DropColumn(
                name: "TheoricalGrade",
                table: "Grade");

            migrationBuilder.RenameColumn(
                name: "TheoricalGradeStatus",
                table: "Grade",
                newName: "GradeType");

            migrationBuilder.AddColumn<float>(
                name: "Mark",
                table: "Grade",
                nullable: false,
                defaultValue: 0f);
        }
    }
}
