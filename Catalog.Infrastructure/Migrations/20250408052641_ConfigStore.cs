using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConfigStore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BannermageGaller_Banner_BannerId",
                table: "BannermageGaller");

            migrationBuilder.DropForeignKey(
                name: "FK_StoreImageGallery_Store_StoreId",
                table: "StoreImageGallery");

            migrationBuilder.AddForeignKey(
                name: "FK_BannermageGaller_Banner_BannerId",
                table: "BannermageGaller",
                column: "BannerId",
                principalTable: "Banner",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StoreImageGallery_Store_StoreId",
                table: "StoreImageGallery",
                column: "StoreId",
                principalTable: "Store",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BannermageGaller_Banner_BannerId",
                table: "BannermageGaller");

            migrationBuilder.DropForeignKey(
                name: "FK_StoreImageGallery_Store_StoreId",
                table: "StoreImageGallery");

            migrationBuilder.AddForeignKey(
                name: "FK_BannermageGaller_Banner_BannerId",
                table: "BannermageGaller",
                column: "BannerId",
                principalTable: "Banner",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StoreImageGallery_Store_StoreId",
                table: "StoreImageGallery",
                column: "StoreId",
                principalTable: "Store",
                principalColumn: "Id");
        }
    }
}
