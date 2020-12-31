using Microsoft.EntityFrameworkCore.Migrations;

namespace QuanLiNhaSach.Data.Migrations
{
    public partial class addIsDebitForBill : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDebit",
                table: "Bills",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDebit",
                table: "Bills");
        }
    }
}
