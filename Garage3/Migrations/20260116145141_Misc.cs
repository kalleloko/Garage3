using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garage3.Migrations
{
    /// <inheritdoc />
    public partial class Misc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpotSize",
                table: "ParkingSpots");

            migrationBuilder.RenameColumn(
                name: "SpotNUmber",
                table: "ParkingSpots",
                newName: "SpotNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SpotNumber",
                table: "ParkingSpots",
                newName: "SpotNUmber");

            migrationBuilder.AddColumn<int>(
                name: "SpotSize",
                table: "ParkingSpots",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
