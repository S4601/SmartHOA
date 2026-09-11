using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartHOA.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFloorAndApartmentCountToBuilding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApartmentCountPerFloor",
                table: "Buildings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FloorCount",
                table: "Buildings",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApartmentCountPerFloor",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "FloorCount",
                table: "Buildings");
        }
    }
}
