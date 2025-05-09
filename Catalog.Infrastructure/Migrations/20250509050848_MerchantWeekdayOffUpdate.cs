using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MerchantWeekdayOffUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Merchant_ServicePackage_ServicePackageId",
                table: "Merchant");

            migrationBuilder.AlterColumn<int>(
                name: "ServicePackageId",
                table: "Merchant",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Merchant_ServicePackage_ServicePackageId",
                table: "Merchant",
                column: "ServicePackageId",
                principalTable: "ServicePackage",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Merchant_ServicePackage_ServicePackageId",
                table: "Merchant");

            migrationBuilder.AlterColumn<int>(
                name: "ServicePackageId",
                table: "Merchant",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Merchant_ServicePackage_ServicePackageId",
                table: "Merchant",
                column: "ServicePackageId",
                principalTable: "ServicePackage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
