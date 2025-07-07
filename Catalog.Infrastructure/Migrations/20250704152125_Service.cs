using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Service : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Merchant_ServicePackageId",
                table: "Merchant");

            migrationBuilder.AddColumn<int>(
                name: "ServicePackageId",
                table: "Store",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServiceId",
                table: "ServicePackage",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Service",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    UrlImage = table.Column<string>(type: "text", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Service", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Store_ServicePackageId",
                table: "Store",
                column: "ServicePackageId");

            migrationBuilder.CreateIndex(
                name: "IX_ServicePackage_ServiceId",
                table: "ServicePackage",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Merchant_ServicePackageId",
                table: "Merchant",
                column: "ServicePackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServicePackage_Service_ServiceId",
                table: "ServicePackage",
                column: "ServiceId",
                principalTable: "Service",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Store_ServicePackage_ServicePackageId",
                table: "Store",
                column: "ServicePackageId",
                principalTable: "ServicePackage",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServicePackage_Service_ServiceId",
                table: "ServicePackage");

            migrationBuilder.DropForeignKey(
                name: "FK_Store_ServicePackage_ServicePackageId",
                table: "Store");

            migrationBuilder.DropTable(
                name: "Service");

            migrationBuilder.DropIndex(
                name: "IX_Store_ServicePackageId",
                table: "Store");

            migrationBuilder.DropIndex(
                name: "IX_ServicePackage_ServiceId",
                table: "ServicePackage");

            migrationBuilder.DropIndex(
                name: "IX_Merchant_ServicePackageId",
                table: "Merchant");

            migrationBuilder.DropColumn(
                name: "ServicePackageId",
                table: "Store");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "ServicePackage");

            migrationBuilder.CreateIndex(
                name: "IX_Merchant_ServicePackageId",
                table: "Merchant",
                column: "ServicePackageId",
                unique: true);
        }
    }
}
