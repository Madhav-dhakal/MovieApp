using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieApplication.Migrations
{
    public partial class AddRatingColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Check if the column already exists
            var columnExists = migrationBuilder.Sql(@"
                SELECT COUNT(*) 
                FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_NAME = 'MovieTable' 
                  AND COLUMN_NAME = 'Rating'")
                .ToString() != "0";

            if (!columnExists)
            {
                migrationBuilder.AddColumn<int>(
                    name: "Rating",
                    table: "MovieTable",
                    type: "int",
                    nullable: false,
                    defaultValue: 0);
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Check if the column exists before trying to drop it
            var columnExists = migrationBuilder.Sql(@"
                SELECT COUNT(*) 
                FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_NAME = 'MovieTable' 
                  AND COLUMN_NAME = 'Rating'")
                .ToString() != "0";

            if (columnExists)
            {
                migrationBuilder.DropColumn(
                    name: "Rating",
                    table: "MovieTable");
            }
        }
    }
}
