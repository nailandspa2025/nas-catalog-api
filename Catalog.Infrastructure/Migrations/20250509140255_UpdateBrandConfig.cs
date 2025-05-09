using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBrandConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Store_Brand_BrandId",
                table: "Store");

            migrationBuilder.DropIndex(
                name: "IX_Store_BrandId",
                table: "Store");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Store_BrandId",
                table: "Store",
                column: "BrandId");

            migrationBuilder.AddForeignKey(
                name: "FK_Store_Brand_BrandId",
                table: "Store",
                column: "BrandId",
                principalTable: "Brand",
                principalColumn: "Id");
        }
    }
}
