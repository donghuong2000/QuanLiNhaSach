using Microsoft.EntityFrameworkCore.Migrations;

namespace QuanLiNhaSach.Data.Migrations
{
    public partial class fixcol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookExistDetail_BookExistHeader",
                table: "BookExistDetails");

            migrationBuilder.DropTable(
                name: "BookExistHeaders");

            migrationBuilder.DropIndex(
                name: "IX_BookExistDetails_BookExistHeaderId",
                table: "BookExistDetails");

            migrationBuilder.DropColumn(
                name: "BookExistHeaderId",
                table: "BookExistDetails");

            migrationBuilder.AddColumn<string>(
                name: "BookId",
                table: "BookExistDetails",
                maxLength: 450,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookExistDetails_BookId",
                table: "BookExistDetails",
                column: "BookId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookExistDetail_Book",
                table: "BookExistDetails",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookExistDetail_Book",
                table: "BookExistDetails");

            migrationBuilder.DropIndex(
                name: "IX_BookExistDetails_BookId",
                table: "BookExistDetails");

            migrationBuilder.DropColumn(
                name: "BookId",
                table: "BookExistDetails");

            migrationBuilder.AddColumn<string>(
                name: "BookExistHeaderId",
                table: "BookExistDetails",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BookExistHeaders",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BookId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    TotalExist = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookExistHeaders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookExistHeader_Book",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookExistDetails_BookExistHeaderId",
                table: "BookExistDetails",
                column: "BookExistHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_BookExistHeaders_BookId",
                table: "BookExistHeaders",
                column: "BookId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookExistDetail_BookExistHeader",
                table: "BookExistDetails",
                column: "BookExistHeaderId",
                principalTable: "BookExistHeaders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
