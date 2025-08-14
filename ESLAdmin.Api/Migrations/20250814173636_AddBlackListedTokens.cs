using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ESLAdmin.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddBlackListedTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlacklistedTokens",
                columns: table => new
                {
                    Id = table.Column<string>(type: "VARCHAR(256)", nullable: false),
                    Token = table.Column<string>(type: "VARCHAR(2048)", maxLength: 2048, nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    UserId = table.Column<string>(type: "VARCHAR(450)", maxLength: 450, nullable: false),
                    BlacklistedOn = table.Column<DateTime>(type: "TIMESTAMP", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlacklistedTokens", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlacklistedTokens_ExpiryDate",
                table: "BlacklistedTokens",
                column: "ExpiryDate");

            migrationBuilder.CreateIndex(
                name: "IX_BlacklistedTokens_UserId",
                table: "BlacklistedTokens",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlacklistedTokens");
        }
    }
}
