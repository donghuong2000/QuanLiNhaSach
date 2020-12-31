using Microsoft.EntityFrameworkCore.Migrations;

namespace QuanLiNhaSach.Data.Migrations
{
    public partial class DebitDetailArrange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DebitDetail_DebitHeader",
                table: "DebitDetails");

            migrationBuilder.DropTable(
                name: "DebitHeaders");

            migrationBuilder.DropIndex(
                name: "IX_DebitDetails_DebitHeaderId",
                table: "DebitDetails");

            migrationBuilder.DropColumn(
                name: "DebitHeaderId",
                table: "DebitDetails");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "DebitDetails",
                maxLength: 450,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DebitDetails_ApplicationUserId",
                table: "DebitDetails",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_DebitDetail_AppUser",
                table: "DebitDetails",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DebitDetail_AppUser",
                table: "DebitDetails");

            migrationBuilder.DropIndex(
                name: "IX_DebitDetails_ApplicationUserId",
                table: "DebitDetails");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "DebitDetails");

            migrationBuilder.AddColumn<string>(
                name: "DebitHeaderId",
                table: "DebitDetails",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DebitHeaders",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    TotalDebit = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebitHeaders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DebitHeader_APPUSER",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DebitDetails_DebitHeaderId",
                table: "DebitDetails",
                column: "DebitHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_DebitHeaders_ApplicationUserId",
                table: "DebitHeaders",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_DebitDetail_DebitHeader",
                table: "DebitDetails",
                column: "DebitHeaderId",
                principalTable: "DebitHeaders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
