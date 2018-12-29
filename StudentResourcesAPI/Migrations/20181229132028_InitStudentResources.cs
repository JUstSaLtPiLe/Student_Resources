using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StudentResourcesAPI.Migrations
{
    public partial class InitStudentResources : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RollNumber = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: false),
                    Salt = table.Column<byte[]>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    Role = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clazz",
                columns: table => new
                {
                    ClazzId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Teacher = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdateAt = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clazz", x => x.ClazzId);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    RoleId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "Subject",
                columns: table => new
                {
                    SubjectId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subject", x => x.SubjectId);
                });

            migrationBuilder.CreateTable(
                name: "GeneralInformation",
                columns: table => new
                {
                    AccountId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Gender = table.Column<int>(nullable: false),
                    Dob = table.Column<DateTime>(nullable: false),
                    Phone = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralInformation", x => x.AccountId);
                    table.ForeignKey(
                        name: "FK_GeneralInformation_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentClazz",
                columns: table => new
                {
                    AccountId = table.Column<int>(nullable: false),
                    ClazzId = table.Column<int>(nullable: false),
                    AccountId1 = table.Column<int>(nullable: true),
                    JoinDate = table.Column<DateTime>(nullable: false),
                    GraduateDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentClazz", x => new { x.AccountId, x.ClazzId });
                    table.UniqueConstraint("AK_StudentClazz_AccountId", x => x.AccountId);
                    table.ForeignKey(
                        name: "FK_StudentClazz_Account_AccountId1",
                        column: x => x.AccountId1,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentClazz_Clazz_ClazzId",
                        column: x => x.ClazzId,
                        principalTable: "Clazz",
                        principalColumn: "ClazzId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleAccount",
                columns: table => new
                {
                    AccountId = table.Column<int>(nullable: false),
                    RoleId = table.Column<int>(nullable: false),
                    GrantDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleAccount", x => new { x.RoleId, x.AccountId });
                    table.ForeignKey(
                        name: "FK_RoleAccount_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleAccount_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClazzSubject",
                columns: table => new
                {
                    ClazzId = table.Column<int>(nullable: false),
                    SubjectId = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClazzSubject", x => new { x.ClazzId, x.SubjectId });
                    table.ForeignKey(
                        name: "FK_ClazzSubject_Clazz_ClazzId",
                        column: x => x.ClazzId,
                        principalTable: "Clazz",
                        principalColumn: "ClazzId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClazzSubject_Subject_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subject",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Grade",
                columns: table => new
                {
                    AccountId = table.Column<int>(nullable: false),
                    SubjectId = table.Column<int>(nullable: false),
                    Mark = table.Column<float>(nullable: false),
                    GradeType = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grade", x => new { x.AccountId, x.SubjectId });
                    table.ForeignKey(
                        name: "FK_Grade_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Grade_Subject_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subject",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClazzSubject_SubjectId",
                table: "ClazzSubject",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Grade_SubjectId",
                table: "Grade",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleAccount_AccountId",
                table: "RoleAccount",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentClazz_AccountId1",
                table: "StudentClazz",
                column: "AccountId1");

            migrationBuilder.CreateIndex(
                name: "IX_StudentClazz_ClazzId",
                table: "StudentClazz",
                column: "ClazzId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClazzSubject");

            migrationBuilder.DropTable(
                name: "GeneralInformation");

            migrationBuilder.DropTable(
                name: "Grade");

            migrationBuilder.DropTable(
                name: "RoleAccount");

            migrationBuilder.DropTable(
                name: "StudentClazz");

            migrationBuilder.DropTable(
                name: "Subject");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "Clazz");
        }
    }
}
