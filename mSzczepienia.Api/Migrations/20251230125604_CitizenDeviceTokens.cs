using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mSzczepienia.Api.Migrations
{
    /// <inheritdoc />
    public partial class CitizenDeviceTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "citizenDeviceTokens",
                columns: table => new
                {
                    citizenDeviceTokenId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    citizenId = table.Column<int>(type: "int", nullable: false),
                    deviceToken = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    lastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_citizenDeviceTokens", x => x.citizenDeviceTokenId);
                    table.ForeignKey(
                        name: "FK_citizenDeviceTokens_citizens_citizenId",
                        column: x => x.citizenId,
                        principalTable: "citizens",
                        principalColumn: "citizenId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_citizenDeviceTokens_citizenId",
                table: "citizenDeviceTokens",
                column: "citizenId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "citizenDeviceTokens");
        }
    }
}
