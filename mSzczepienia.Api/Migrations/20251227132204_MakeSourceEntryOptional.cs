using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mSzczepienia.Api.Migrations
{
    /// <inheritdoc />
    public partial class MakeSourceEntryOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_vaccinationRecords_sourceEntryId",
                table: "vaccinationRecords");

            migrationBuilder.AlterColumn<int>(
                name: "sourceEntryId",
                table: "vaccinationRecords",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_vaccinationRecords_sourceEntryId",
                table: "vaccinationRecords",
                column: "sourceEntryId",
                unique: true,
                filter: "[sourceEntryId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_vaccinationRecords_sourceEntryId",
                table: "vaccinationRecords");

            migrationBuilder.AlterColumn<int>(
                name: "sourceEntryId",
                table: "vaccinationRecords",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_vaccinationRecords_sourceEntryId",
                table: "vaccinationRecords",
                column: "sourceEntryId",
                unique: true);
        }
    }
}
