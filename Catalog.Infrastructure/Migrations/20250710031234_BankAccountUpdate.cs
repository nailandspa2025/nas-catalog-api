using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BankAccountUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankAccountStore_BankAccount_bankAccountsId",
                table: "BankAccountStore");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BankAccountStore",
                table: "BankAccountStore");

            migrationBuilder.DropIndex(
                name: "IX_BankAccountStore_bankAccountsId",
                table: "BankAccountStore");

            migrationBuilder.RenameColumn(
                name: "bankAccountsId",
                table: "BankAccountStore",
                newName: "BankAccountsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BankAccountStore",
                table: "BankAccountStore",
                columns: new[] { "BankAccountsId", "StoresId" });

            migrationBuilder.CreateIndex(
                name: "IX_BankAccountStore_StoresId",
                table: "BankAccountStore",
                column: "StoresId");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccountStore_BankAccount_BankAccountsId",
                table: "BankAccountStore",
                column: "BankAccountsId",
                principalTable: "BankAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankAccountStore_BankAccount_BankAccountsId",
                table: "BankAccountStore");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BankAccountStore",
                table: "BankAccountStore");

            migrationBuilder.DropIndex(
                name: "IX_BankAccountStore_StoresId",
                table: "BankAccountStore");

            migrationBuilder.RenameColumn(
                name: "BankAccountsId",
                table: "BankAccountStore",
                newName: "bankAccountsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BankAccountStore",
                table: "BankAccountStore",
                columns: new[] { "StoresId", "bankAccountsId" });

            migrationBuilder.CreateIndex(
                name: "IX_BankAccountStore_bankAccountsId",
                table: "BankAccountStore",
                column: "bankAccountsId");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccountStore_BankAccount_bankAccountsId",
                table: "BankAccountStore",
                column: "bankAccountsId",
                principalTable: "BankAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
