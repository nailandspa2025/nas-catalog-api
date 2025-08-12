using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeAppDeepLinkIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserStoreDeepLink_AppDeepLink_AppDeepLinkId",
                table: "UserStoreDeepLink");

            migrationBuilder.DropIndex(
                name: "IX_UserStoreDeepLink_AppDeepLinkId",
                table: "UserStoreDeepLink");

            migrationBuilder.DropColumn(
                name: "AppDeepLinkId",
                table: "UserStoreDeepLink");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppDeepLinkId",
                table: "UserStoreDeepLink",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserStoreDeepLink_AppDeepLinkId",
                table: "UserStoreDeepLink",
                column: "AppDeepLinkId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserStoreDeepLink_AppDeepLink_AppDeepLinkId",
                table: "UserStoreDeepLink",
                column: "AppDeepLinkId",
                principalTable: "AppDeepLink",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
