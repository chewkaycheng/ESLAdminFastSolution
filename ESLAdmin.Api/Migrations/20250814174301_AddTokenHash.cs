using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ESLAdmin.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTokenHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TokenHash",
                table: "BlacklistedTokens",
                type: "VARCHAR(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_BlacklistedTokens_TokenHash",
                table: "BlacklistedTokens",
                column: "TokenHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BlacklistedTokens_TokenHash",
                table: "BlacklistedTokens");

            migrationBuilder.DropColumn(
                name: "TokenHash",
                table: "BlacklistedTokens");
        }
    }
}
