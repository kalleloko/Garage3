using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garage3.Migrations
{
    /// <inheritdoc />
    public partial class ChangeParkingSpotModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ParkingSpots",
                newName: "SpotNUmber");

            migrationBuilder.AddColumn<bool>(
                name: "IsOccupied",
                table: "ParkingSpots",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SpotSize",
                table: "ParkingSpots",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "SSN",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SSN",
                table: "AspNetUsers",
                column: "SSN",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SSN",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsOccupied",
                table: "ParkingSpots");

            migrationBuilder.DropColumn(
                name: "SpotSize",
                table: "ParkingSpots");

            migrationBuilder.RenameColumn(
                name: "SpotNUmber",
                table: "ParkingSpots",
                newName: "Name");

            migrationBuilder.AlterColumn<string>(
                name: "SSN",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
