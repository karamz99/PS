using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PS.Data.Migrations
{
    public partial class M0005 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "DataUsageSent",
                table: "Customers",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "DataUsageRecieved",
                table: "Customers",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.InsertData(
                table: "BlackLists",
                columns: new[] { "Id", "Url" },
                values: new object[] { 1, "msn.com" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DataUsageRecieved", "DataUsageSent" },
                values: new object[] { 0L, 0L });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DataUsageRecieved", "DataUsageSent" },
                values: new object[] { 0L, 0L });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DataUsageRecieved", "DataUsageSent" },
                values: new object[] { 0L, 0L });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DataUsageRecieved", "DataUsageSent" },
                values: new object[] { 0L, 0L });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BlackLists",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "DataUsageSent",
                table: "Customers",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "DataUsageRecieved",
                table: "Customers",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DataUsageRecieved", "DataUsageSent" },
                values: new object[] { 0, 0 });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DataUsageRecieved", "DataUsageSent" },
                values: new object[] { 0, 0 });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DataUsageRecieved", "DataUsageSent" },
                values: new object[] { 0, 0 });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DataUsageRecieved", "DataUsageSent" },
                values: new object[] { 0, 0 });
        }
    }
}
