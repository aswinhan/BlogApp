using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogApp.Modules.Blog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddArticleCoverImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoverImageUrl",
                schema: "blog",
                table: "Articles",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverImageUrl",
                schema: "blog",
                table: "Articles");
        }
    }
}
