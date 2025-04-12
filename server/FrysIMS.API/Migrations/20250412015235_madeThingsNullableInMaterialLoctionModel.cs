using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FrysIMS.API.Migrations
{
    /// <inheritdoc />
    public partial class madeThingsNullableInMaterialLoctionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialLocations_AspNetUsers_UpdatedByUserId",
                table: "MaterialLocations");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedByUserId",
                table: "MaterialLocations",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialLocations_AspNetUsers_UpdatedByUserId",
                table: "MaterialLocations",
                column: "UpdatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialLocations_AspNetUsers_UpdatedByUserId",
                table: "MaterialLocations");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedByUserId",
                table: "MaterialLocations",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialLocations_AspNetUsers_UpdatedByUserId",
                table: "MaterialLocations",
                column: "UpdatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
