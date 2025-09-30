using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentRewview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "ReviewTechnician",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "ReviewService",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "ReviewTechnician");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "ReviewService");
        }
    }
}
