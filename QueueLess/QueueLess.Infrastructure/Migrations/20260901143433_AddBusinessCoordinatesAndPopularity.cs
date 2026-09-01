using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QueueLess.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBusinessCoordinatesAndPopularity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "Tag",
                table: "Businesses");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Businesses",
                type: "float(9)",
                precision: 9,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Businesses",
                type: "float(9)",
                precision: 9,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PopularityScore",
                table: "Businesses",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "PopularityScore",
                table: "Businesses");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Businesses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tag",
                table: "Businesses",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
