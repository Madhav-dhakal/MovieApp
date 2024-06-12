using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieApplication.Migrations
{
    /// <inheritdoc />
    public partial class second : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MovieUrl",
                table: "MovieTable",
                newName: "Movie");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Movie",
                table: "MovieTable",
                newName: "MovieUrl");
        }
    }
}
