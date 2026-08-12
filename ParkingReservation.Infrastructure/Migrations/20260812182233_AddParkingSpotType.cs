using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ParkingReservation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddParkingSpotType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "ParkingSpots",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "ParkingSpots");
        }
    }
}
