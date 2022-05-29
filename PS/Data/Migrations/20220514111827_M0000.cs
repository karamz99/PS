using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PS.Data.Migrations
{
    public partial class M0000 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataUsageControl = table.Column<bool>(type: "bit", nullable: false),
                    DataUsageSent = table.Column<int>(type: "int", nullable: false),
                    DataUsageRecieved = table.Column<int>(type: "int", nullable: false),
                    TimeUsageControl = table.Column<bool>(type: "bit", nullable: false),
                    TimeUsageStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeUsageEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SpeedUsageControl = table.Column<bool>(type: "bit", nullable: false),
                    SpeedUsage = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customers_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "64178325-cad7-42c0-9474-583800141e2e", 0, "7e75eee3-4987-44a5-8b0e-daad163d167a", "admin@admin.com", true, true, null, "ADMIN@ADMIN.COM", "ADMIN@ADMIN.COM", "AQAAAAEAACcQAAAAEAm10iVbfFso9WXr1xZkHKh/t3m4AKgTOoMP4ES2FMzmgRZshGu1z7/aZ0KjdL8tIw==", null, false, "DWXAEFIDGFEDNRMJRVJNWDCY5TIAGSLH", false, "admin@admin.com" });

            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[] { "Id", "DataUsageControl", "DataUsageRecieved", "DataUsageSent", "Name", "SpeedUsage", "SpeedUsageControl", "TimeUsageControl", "TimeUsageEnd", "TimeUsageStart" },
                values: new object[] { 1, true, 3072, 2048, "Teachers", 0, false, true, new DateTime(1900, 1, 1, 22, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1900, 1, 1, 8, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[] { "Id", "DataUsageControl", "DataUsageRecieved", "DataUsageSent", "Name", "SpeedUsage", "SpeedUsageControl", "TimeUsageControl", "TimeUsageEnd", "TimeUsageStart" },
                values: new object[] { 2, true, 1024, 1024, "Students", 100, true, true, new DateTime(1900, 1, 1, 14, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1900, 1, 1, 8, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "GroupId", "Name", "Password", "Username" },
                values: new object[,]
                {
                    { 1, 1, "Teacher1", "Teacher1", "Teacher1" },
                    { 2, 1, "Teacher2", "Teacher2", "Teacher2" },
                    { 3, 2, "Student1", "Student1", "Student1" },
                    { 4, 2, "Student2", "Student2", "Student2" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_GroupId",
                table: "Customers",
                column: "GroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "64178325-cad7-42c0-9474-583800141e2e");
        }
    }
}
