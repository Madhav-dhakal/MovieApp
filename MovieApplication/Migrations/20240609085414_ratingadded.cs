using Microsoft.EntityFrameworkCore.Migrations;

namespace MovieApplication.Migrations
{
    public partial class ratingadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_MovieTable_MovieModelId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_MovieModelId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "MovieModelId",
                table: "Reviews");

            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_MovieId",
                table: "Reviews",
                column: "MovieId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_MovieTable_MovieId",
                table: "Reviews",
                column: "MovieId",
                principalTable: "MovieTable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_MovieTable_MovieId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_MovieId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Reviews");

            migrationBuilder.AddColumn<int>(
                name: "MovieModelId",
                table: "Reviews",
                type: "int",
                nullable: true);

            migrationBuilder.Sql("UPDATE Reviews SET MovieModelId = (SELECT Id FROM MovieTable WHERE MovieTable.Id = Reviews.MovieId)");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_MovieModelId",
                table: "Reviews",
                column: "MovieModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_MovieTable_MovieModelId",
                table: "Reviews",
                column: "MovieModelId",
                principalTable: "MovieTable",
                principalColumn: "Id");
        }
    }
}
