using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Store");

            migrationBuilder.CreateIndex(
                name: "IX_UserStore_StoreId",
                table: "UserStore",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserStore_Store_StoreId",
                table: "UserStore",
                column: "StoreId",
                principalTable: "Store",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserStore_Store_StoreId",
                table: "UserStore");

            migrationBuilder.DropIndex(
                name: "IX_UserStore_StoreId",
                table: "UserStore");

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Store",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
