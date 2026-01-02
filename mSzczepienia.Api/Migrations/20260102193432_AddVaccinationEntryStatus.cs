using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mSzczepienia.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddVaccinationEntryStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "vaccinationEntries",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "vaccinationEntries");
        }
    }
}
