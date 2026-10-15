using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartHOA.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFeeRatesToBuilding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BaseOccupantFee",
                table: "Buildings",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 15.00m);

            migrationBuilder.AddColumn<decimal>(
                name: "PetFee",
                table: "Buildings",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 15.00m);

            migrationBuilder.AddColumn<decimal>(
                name: "RepairFundRatePerIdealPart",
                table: "Buildings",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 1.50m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseOccupantFee",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "PetFee",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "RepairFundRatePerIdealPart",
                table: "Buildings");
        }
    }
}
