using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class calendarUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Recurrence",
                table: "Calendar",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RecurrenceEndDate",
                table: "Calendar",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecurrenceInterval",
                table: "Calendar",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReminderMinutesBefore",
                table: "Calendar",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "StoreId",
                table: "Calendar",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TechnicianId",
                table: "Calendar",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Calendar_StoreId",
                table: "Calendar",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Calendar_Store_StoreId",
                table: "Calendar",
                column: "StoreId",
                principalTable: "Store",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Calendar_Store_StoreId",
                table: "Calendar");

            migrationBuilder.DropIndex(
                name: "IX_Calendar_StoreId",
                table: "Calendar");

            migrationBuilder.DropColumn(
                name: "Recurrence",
                table: "Calendar");

            migrationBuilder.DropColumn(
                name: "RecurrenceEndDate",
                table: "Calendar");

            migrationBuilder.DropColumn(
                name: "RecurrenceInterval",
                table: "Calendar");

            migrationBuilder.DropColumn(
                name: "ReminderMinutesBefore",
                table: "Calendar");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Calendar");

            migrationBuilder.DropColumn(
                name: "TechnicianId",
                table: "Calendar");
        }
    }
}
