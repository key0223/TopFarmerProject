using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TopFarmerWebServer.Migrations
{
    /// <inheritdoc />
    public partial class deleteLoginAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoginAccount");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Account",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Account");

            migrationBuilder.CreateTable(
                name: "LoginAccount",
                columns: table => new
                {
                    LoginAccountDbId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoginAccountName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginAccount", x => x.LoginAccountDbId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoginAccount_LoginAccountName",
                table: "LoginAccount",
                column: "LoginAccountName",
                unique: true);
        }
    }
}
