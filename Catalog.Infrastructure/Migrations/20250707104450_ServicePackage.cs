using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ServicePackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServicePackage_Service_ServiceId",
                table: "ServicePackage");

            migrationBuilder.DropIndex(
                name: "IX_ServicePackage_ServiceId",
                table: "ServicePackage");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "ServicePackage");

            migrationBuilder.CreateTable(
                name: "ServiceServicePackage",
                columns: table => new
                {
                    ServicePackagesId = table.Column<int>(type: "integer", nullable: false),
                    ServicesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceServicePackage", x => new { x.ServicePackagesId, x.ServicesId });
                    table.ForeignKey(
                        name: "FK_ServiceServicePackage_ServicePackage_ServicePackagesId",
                        column: x => x.ServicePackagesId,
                        principalTable: "ServicePackage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceServicePackage_Service_ServicesId",
                        column: x => x.ServicesId,
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceServicePackage_ServicesId",
                table: "ServiceServicePackage",
                column: "ServicesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceServicePackage");

            migrationBuilder.AddColumn<int>(
                name: "ServiceId",
                table: "ServicePackage",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServicePackage_ServiceId",
                table: "ServicePackage",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServicePackage_Service_ServiceId",
                table: "ServicePackage",
                column: "ServiceId",
                principalTable: "Service",
                principalColumn: "Id");
        }
    }
}
