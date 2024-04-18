using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TopFarmerWebServer.Migrations
{
    /// <inheritdoc />
    public partial class PlayerCoin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Coin",
                table: "Player",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Coin",
                table: "Player");
        }
    }
}
