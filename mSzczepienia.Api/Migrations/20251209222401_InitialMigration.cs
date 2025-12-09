using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mSzczepienia.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "accounts",
                columns: table => new
                {
                    accountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    firebaseUid = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    surname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    creationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    dateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounts", x => x.accountId);
                });

            migrationBuilder.CreateTable(
                name: "diseases",
                columns: table => new
                {
                    diseaseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    diseaseNameEn = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    diseaseNamePl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_diseases", x => x.diseaseId);
                });

            migrationBuilder.CreateTable(
                name: "medicalFacilities",
                columns: table => new
                {
                    facilityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    facilityName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    city = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    street = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    country = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    nip = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    creationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_medicalFacilities", x => x.facilityId);
                });

            migrationBuilder.CreateTable(
                name: "vaccineDefinitions",
                columns: table => new
                {
                    vaccineId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vaccineName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    manufacturer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    vaccineDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    creationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vaccineDefinitions", x => x.vaccineId);
                });

            migrationBuilder.CreateTable(
                name: "citizens",
                columns: table => new
                {
                    citizenId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    accountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    pesel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    facilityId = table.Column<int>(type: "int", nullable: false),
                    submissionStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_citizens", x => x.citizenId);
                    table.ForeignKey(
                        name: "FK_citizens_accounts_accountId",
                        column: x => x.accountId,
                        principalTable: "accounts",
                        principalColumn: "accountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_citizens_medicalFacilities_facilityId",
                        column: x => x.facilityId,
                        principalTable: "medicalFacilities",
                        principalColumn: "facilityId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "personnel",
                columns: table => new
                {
                    personnelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    accountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    facilityId = table.Column<int>(type: "int", nullable: false),
                    pwzNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    position = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_personnel", x => x.personnelId);
                    table.ForeignKey(
                        name: "FK_personnel_accounts_accountId",
                        column: x => x.accountId,
                        principalTable: "accounts",
                        principalColumn: "accountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_personnel_medicalFacilities_facilityId",
                        column: x => x.facilityId,
                        principalTable: "medicalFacilities",
                        principalColumn: "facilityId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "vaccineDiseases",
                columns: table => new
                {
                    vaccineId = table.Column<int>(type: "int", nullable: false),
                    diseaseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vaccineDiseases", x => new { x.vaccineId, x.diseaseId });
                    table.ForeignKey(
                        name: "FK_vaccineDiseases_diseases_diseaseId",
                        column: x => x.diseaseId,
                        principalTable: "diseases",
                        principalColumn: "diseaseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_vaccineDiseases_vaccineDefinitions_vaccineId",
                        column: x => x.vaccineId,
                        principalTable: "vaccineDefinitions",
                        principalColumn: "vaccineId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ocrSubmissions",
                columns: table => new
                {
                    submissionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    citizenId = table.Column<int>(type: "int", nullable: false),
                    photoUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    verifierId = table.Column<int>(type: "int", nullable: true),
                    submissionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    verifierNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    verificationDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ocrSubmissions", x => x.submissionId);
                    table.ForeignKey(
                        name: "FK_ocrSubmissions_citizens_citizenId",
                        column: x => x.citizenId,
                        principalTable: "citizens",
                        principalColumn: "citizenId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ocrSubmissions_personnel_verifierId",
                        column: x => x.verifierId,
                        principalTable: "personnel",
                        principalColumn: "personnelId");
                });

            migrationBuilder.CreateTable(
                name: "vaccinationEntries",
                columns: table => new
                {
                    vaccinationEntryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ocrSubmissionId = table.Column<int>(type: "int", nullable: false),
                    batchOcr = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    vaccineNameOcr = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    dateOcr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    vaccineId = table.Column<int>(type: "int", nullable: true),
                    batch = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vaccinationEntries", x => x.vaccinationEntryId);
                    table.ForeignKey(
                        name: "FK_vaccinationEntries_ocrSubmissions_ocrSubmissionId",
                        column: x => x.ocrSubmissionId,
                        principalTable: "ocrSubmissions",
                        principalColumn: "submissionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_vaccinationEntries_vaccineDefinitions_vaccineId",
                        column: x => x.vaccineId,
                        principalTable: "vaccineDefinitions",
                        principalColumn: "vaccineId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "vaccinationRecords",
                columns: table => new
                {
                    recordId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    citizenId = table.Column<int>(type: "int", nullable: false),
                    vaccineId = table.Column<int>(type: "int", nullable: false),
                    vaccinationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    doseNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    nextVaccinationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    batch = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    sourceEntryId = table.Column<int>(type: "int", nullable: false),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    creationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vaccinationRecords", x => x.recordId);
                    table.ForeignKey(
                        name: "FK_vaccinationRecords_citizens_citizenId",
                        column: x => x.citizenId,
                        principalTable: "citizens",
                        principalColumn: "citizenId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_vaccinationRecords_vaccinationEntries_sourceEntryId",
                        column: x => x.sourceEntryId,
                        principalTable: "vaccinationEntries",
                        principalColumn: "vaccinationEntryId");
                    table.ForeignKey(
                        name: "FK_vaccinationRecords_vaccineDefinitions_vaccineId",
                        column: x => x.vaccineId,
                        principalTable: "vaccineDefinitions",
                        principalColumn: "vaccineId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_citizens_accountId",
                table: "citizens",
                column: "accountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_citizens_facilityId",
                table: "citizens",
                column: "facilityId");

            migrationBuilder.CreateIndex(
                name: "IX_ocrSubmissions_citizenId",
                table: "ocrSubmissions",
                column: "citizenId");

            migrationBuilder.CreateIndex(
                name: "IX_ocrSubmissions_verifierId",
                table: "ocrSubmissions",
                column: "verifierId");

            migrationBuilder.CreateIndex(
                name: "IX_personnel_accountId",
                table: "personnel",
                column: "accountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_personnel_facilityId",
                table: "personnel",
                column: "facilityId");

            migrationBuilder.CreateIndex(
                name: "IX_vaccinationEntries_ocrSubmissionId",
                table: "vaccinationEntries",
                column: "ocrSubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_vaccinationEntries_vaccineId",
                table: "vaccinationEntries",
                column: "vaccineId");

            migrationBuilder.CreateIndex(
                name: "IX_vaccinationRecords_citizenId",
                table: "vaccinationRecords",
                column: "citizenId");

            migrationBuilder.CreateIndex(
                name: "IX_vaccinationRecords_sourceEntryId",
                table: "vaccinationRecords",
                column: "sourceEntryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vaccinationRecords_vaccineId",
                table: "vaccinationRecords",
                column: "vaccineId");

            migrationBuilder.CreateIndex(
                name: "IX_vaccineDiseases_diseaseId",
                table: "vaccineDiseases",
                column: "diseaseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "vaccinationRecords");

            migrationBuilder.DropTable(
                name: "vaccineDiseases");

            migrationBuilder.DropTable(
                name: "vaccinationEntries");

            migrationBuilder.DropTable(
                name: "diseases");

            migrationBuilder.DropTable(
                name: "ocrSubmissions");

            migrationBuilder.DropTable(
                name: "vaccineDefinitions");

            migrationBuilder.DropTable(
                name: "citizens");

            migrationBuilder.DropTable(
                name: "personnel");

            migrationBuilder.DropTable(
                name: "accounts");

            migrationBuilder.DropTable(
                name: "medicalFacilities");
        }
    }
}
