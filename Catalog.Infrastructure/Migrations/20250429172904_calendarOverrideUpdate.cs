using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class calendarOverrideUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CalendarTypeId",
                table: "CalendarOverride",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "CalendarOverride",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "StoreId",
                table: "CalendarOverride",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TechnicianId",
                table: "CalendarOverride",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "CalendarOverride",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarOverride_CalendarTypeId",
                table: "CalendarOverride",
                column: "CalendarTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarOverride_StoreId",
                table: "CalendarOverride",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarOverride_CalendarType_CalendarTypeId",
                table: "CalendarOverride",
                column: "CalendarTypeId",
                principalTable: "CalendarType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CalendarOverride_Store_StoreId",
                table: "CalendarOverride",
                column: "StoreId",
                principalTable: "Store",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalendarOverride_CalendarType_CalendarTypeId",
                table: "CalendarOverride");

            migrationBuilder.DropForeignKey(
                name: "FK_CalendarOverride_Store_StoreId",
                table: "CalendarOverride");

            migrationBuilder.DropIndex(
                name: "IX_CalendarOverride_CalendarTypeId",
                table: "CalendarOverride");

            migrationBuilder.DropIndex(
                name: "IX_CalendarOverride_StoreId",
                table: "CalendarOverride");

            migrationBuilder.DropColumn(
                name: "CalendarTypeId",
                table: "CalendarOverride");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "CalendarOverride");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "CalendarOverride");

            migrationBuilder.DropColumn(
                name: "TechnicianId",
                table: "CalendarOverride");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "CalendarOverride");
        }
    }
}
