using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieApplication.Migrations
{
    /// <inheritdoc />
    public partial class movieUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MovieUrl",
                table: "MovieTable",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MovieUrl",
                table: "MovieTable");
        }
    }
}
