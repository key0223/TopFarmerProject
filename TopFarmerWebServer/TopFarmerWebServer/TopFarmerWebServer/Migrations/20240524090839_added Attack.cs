using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TopFarmerWebServer.Migrations
{
    /// <inheritdoc />
    public partial class addedAttack : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hunger",
                table: "Player");

            migrationBuilder.RenameColumn(
                name: "MaxHunger",
                table: "Player",
                newName: "Attack");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Attack",
                table: "Player",
                newName: "MaxHunger");

            migrationBuilder.AddColumn<int>(
                name: "Hunger",
                table: "Player",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
