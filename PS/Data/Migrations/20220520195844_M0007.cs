using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PS.Data.Migrations
{
    public partial class M0007 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DataUsageUnit",
                table: "Groups",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataUsageUnit",
                table: "Groups");
        }
    }
}
