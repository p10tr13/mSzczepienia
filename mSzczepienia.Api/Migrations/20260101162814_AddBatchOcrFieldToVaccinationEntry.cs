using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mSzczepienia.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddBatchOcrFieldToVaccinationEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "batch",
                table: "vaccinationEntries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "batchOcr",
                table: "vaccinationEntries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "batch",
                table: "vaccinationEntries");

            migrationBuilder.DropColumn(
                name: "batchOcr",
                table: "vaccinationEntries");
        }
    }
}
