using Microsoft.EntityFrameworkCore.Migrations;

namespace QuanLiNhaSach.Data.Migrations
{
    public partial class ModifyDept : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receipt_APPUSER",
                table: "Receipts");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "Receipts",
                maxLength: 450,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450,
                oldNullable: true);

            migrationBuilder.AddColumn<float>(
                name: "new_first_debit",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "new_incurred_debit",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "new_last_debit",
                table: "AspNetUsers",
                nullable: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Receipt_APPUSER",
                table: "Receipts",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receipt_APPUSER",
                table: "Receipts");

            migrationBuilder.DropColumn(
                name: "new_first_debit",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "new_incurred_debit",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "new_last_debit",
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

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "Receipts",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 450);

            migrationBuilder.AddForeignKey(
                name: "FK_Receipt_APPUSER",
                table: "Receipts",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
