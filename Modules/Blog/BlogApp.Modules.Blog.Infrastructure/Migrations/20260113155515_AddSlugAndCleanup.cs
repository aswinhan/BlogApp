using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogApp.Modules.Blog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSlugAndCleanup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOnUtc",
                schema: "blog",
                table: "Tags",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOnUtc",
                schema: "blog",
                table: "Tags",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOnUtc",
                schema: "blog",
                table: "Articles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "blog",
                table: "Articles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                schema: "blog",
                table: "Articles",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_Slug",
                schema: "blog",
                table: "Articles",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Articles_Slug",
                schema: "blog",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "CreatedOnUtc",
                schema: "blog",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "ModifiedOnUtc",
                schema: "blog",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "DeletedOnUtc",
                schema: "blog",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "blog",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "Slug",
                schema: "blog",
                table: "Articles");
        }
    }
}
