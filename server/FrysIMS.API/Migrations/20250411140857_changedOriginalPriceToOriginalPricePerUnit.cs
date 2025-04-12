using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FrysIMS.API.Migrations
{
    /// <inheritdoc />
    public partial class changedOriginalPriceToOriginalPricePerUnit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stock_AspNetUsers_CreatedByUserId",
                table: "Stock");

            migrationBuilder.RenameColumn(
                name: "OriginalPrice",
                table: "Stock",
                newName: "OriginalPricePerUnit");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByUserId",
                table: "Stock",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_Stock_AspNetUsers_CreatedByUserId",
                table: "Stock",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stock_AspNetUsers_CreatedByUserId",
                table: "Stock");

            migrationBuilder.RenameColumn(
                name: "OriginalPricePerUnit",
                table: "Stock",
                newName: "OriginalPrice");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByUserId",
                table: "Stock",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Stock_AspNetUsers_CreatedByUserId",
                table: "Stock",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
