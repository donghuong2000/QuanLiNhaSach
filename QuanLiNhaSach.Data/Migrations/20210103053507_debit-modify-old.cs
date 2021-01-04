using Microsoft.EntityFrameworkCore.Migrations;

namespace QuanLiNhaSach.Data.Migrations
{
    public partial class debitmodifyold : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "old_first_debit",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "old_incurred_debit",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "old_last_debit",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "old_first_debit",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "old_incurred_debit",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "old_last_debit",
                table: "AspNetUsers");
        }
    }
}
