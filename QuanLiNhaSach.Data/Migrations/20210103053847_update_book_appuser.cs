using Microsoft.EntityFrameworkCore.Migrations;

namespace QuanLiNhaSach.Data.Migrations
{
    public partial class update_book_appuser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "old_Quantity",
                table: "Books",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "old_first_exist",
                table: "Books",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "old_incurred_exist",
                table: "Books",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "old_Quantity",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "old_first_exist",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "old_incurred_exist",
                table: "Books");
        }
    }
}
