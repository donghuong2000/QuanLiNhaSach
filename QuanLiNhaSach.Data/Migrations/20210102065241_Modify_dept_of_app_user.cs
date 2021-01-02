using Microsoft.EntityFrameworkCore.Migrations;

namespace QuanLiNhaSach.Data.Migrations
{
    public partial class Modify_dept_of_app_user : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dept",
                table: "AspNetUsers");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Dept",
                table: "AspNetUsers",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "old_first_debit",
                table: "AspNetUsers",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "old_incurred_debit",
                table: "AspNetUsers",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "old_last_debit",
                table: "AspNetUsers",
                type: "real",
                nullable: true);
        }
    }
}
