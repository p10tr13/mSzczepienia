using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mSzczepienia.Api.Migrations
{
    /// <inheritdoc />
    public partial class PolishVaccinesDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "vaccineDescription",
                table: "vaccineDefinitions",
                newName: "vaccineDescriptionPl");

            migrationBuilder.AddColumn<string>(
                name: "vaccineDescriptionEn",
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
                name: "vaccineDescriptionEn",
                table: "vaccineDefinitions");

            migrationBuilder.RenameColumn(
                name: "vaccineDescriptionPl",
                table: "vaccineDefinitions",
                newName: "vaccineDescription");
        }
    }
}
