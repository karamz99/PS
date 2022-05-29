using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PS.Data.Migrations
{
    public partial class M0006 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataUsageRecieved",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "DataUsageSent",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "SpeedUsage",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "SpeedUsageControl",
                table: "Groups");

            migrationBuilder.AddColumn<long>(
                name: "DataUsage",
                table: "Groups",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.UpdateData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: 1,
                column: "DataUsage",
                value: 2048L);

            migrationBuilder.UpdateData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: 2,
                column: "DataUsage",
                value: 1024L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataUsage",
                table: "Groups");

            migrationBuilder.AddColumn<int>(
                name: "DataUsageRecieved",
                table: "Groups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DataUsageSent",
                table: "Groups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SpeedUsage",
                table: "Groups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SpeedUsageControl",
                table: "Groups",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DataUsageRecieved", "DataUsageSent" },
                values: new object[] { 3072, 2048 });

            migrationBuilder.UpdateData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DataUsageRecieved", "DataUsageSent", "SpeedUsage", "SpeedUsageControl" },
                values: new object[] { 1024, 1024, 100, true });
        }
    }
}
