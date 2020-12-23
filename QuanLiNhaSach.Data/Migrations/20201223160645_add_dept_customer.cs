using Microsoft.EntityFrameworkCore.Migrations;

namespace QuanLiNhaSach.Data.Migrations
{
    public partial class add_dept_customer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Dept",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dept",
                table: "AspNetUsers");
        }
    }
}
