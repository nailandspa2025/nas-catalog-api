using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReviewStoreFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReviewStoreFile",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Url = table.Column<string>(type: "text", nullable: true),
                    ReviewStoreId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewStoreFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewStoreFile_ReviewStore_ReviewStoreId",
                        column: x => x.ReviewStoreId,
                        principalTable: "ReviewStore",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewStoreFile_ReviewStoreId",
                table: "ReviewStoreFile",
                column: "ReviewStoreId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReviewStoreFile");
        }
    }
}
