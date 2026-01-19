using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mSzczepienia.Api.Migrations
{
    /// <inheritdoc />
    public partial class VaccinesShortDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "batch",
                table: "vaccinationEntries");

            migrationBuilder.DropColumn(
                name: "batchOcr",
                table: "vaccinationEntries");

            migrationBuilder.AddColumn<string>(
                name: "shortDescriptionEn",
                table: "vaccineDefinitions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "shortDescriptionPl",
                table: "vaccineDefinitions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "shortDescriptionEn",
                table: "vaccineDefinitions");

            migrationBuilder.DropColumn(
                name: "shortDescriptionPl",
                table: "vaccineDefinitions");

            migrationBuilder.AddColumn<string>(
                name: "batch",
                table: "vaccinationEntries",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "batchOcr",
                table: "vaccinationEntries",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");
        }
    }
}
