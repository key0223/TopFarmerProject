using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TopFarmerWebServer.Migrations
{
    /// <inheritdoc />
    public partial class addPlayerhp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Hp",
                table: "Player",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hp",
                table: "Player");
        }
    }
}
