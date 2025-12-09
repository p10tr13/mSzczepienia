using Microsoft.EntityFrameworkCore;
using mSzczepienia.Shared.Models;

namespace mSzczepienia.Api
{
    public class AppDbContext: DbContext
    {
        public DbSet<Account> accounts { get; set; }
        public DbSet<Citizen> citizens { get; set; }
        public DbSet<Personnel> personnel { get; set; }
        public DbSet<MedicalFacility> medicalFacilities { get; set; }
        public DbSet<OcrSubmission> ocrSubmissions { get; set; }
        public DbSet<VaccinationEntry> vaccinationEntries { get; set; }
        public DbSet<VaccinationRecord> vaccinationRecords { get; set; }
        public DbSet<VaccineDefinition> vaccineDefinitions { get; set; }
        public DbSet<VaccineDisease> vaccineDiseases { get; set; }
        public DbSet<Disease> diseases { get; set; }


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // ACCOUNT
            modelBuilder.Entity<Account>(static e =>
            {
                // Primary key
                e.HasKey(a => a.accountId);

                // Column properties
                e.Property(a => a.accountId)
                    .IsRequired()
                    .HasDefaultValueSql("NEWID()"); // Generates a new GUID by default
                e.Property(a => a.firebaseUid)
                    .IsRequired()
                    .HasMaxLength(128);
                e.Property(a => a.email)
                    .IsRequired()
                    .HasMaxLength(100);
                e.Property(a => a.role)
                    .IsRequired()
                    .HasConversion<string>() // Store enum as string
                    .HasMaxLength(20);
                e.Property(a => a.name)
                    .IsRequired()
                    .HasMaxLength(50);
                e.Property(a => a.surname)
                    .IsRequired()
                    .HasMaxLength(50);
                e.Property(a => a.creationDate)
                    .IsRequired();
                e.Property(a => a.dateOfBirth)
                    .IsRequired();
            });

            // CITIZEN
            modelBuilder.Entity<Citizen>(static e =>
            {
                // Primary key
                e.HasKey(c => c.citizenId);

                // Column properties
                e.Property(c => c.citizenId)
                    .IsRequired()
                    .ValueGeneratedOnAdd(); // Auto-incremented ID
                e.Property(c => c.accountId)
                    .IsRequired();
                e.Property(c => c.pesel)
                    .IsRequired()
                    .HasMaxLength(20);
                e.Property(c => c.facilityId)
                    .IsRequired();
                e.Property(c => c.submissionStatus)
                    .IsRequired()
                    .HasConversion<string>() // Store enum as string
                    .HasMaxLength(20);

                // Relations
                e.HasOne(c => c.account)
                    .WithOne()
                    .HasForeignKey<Citizen>(c => c.accountId)
                    .OnDelete(DeleteBehavior.Cascade); // When an Account is deleted, the associated Citizen is also deleted

                e.HasOne(c => c.medicalFacility)
                    .WithMany(f => f.citizens)
                    .HasForeignKey(c => c.facilityId)
                    .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of MedicalFacility if Citizens are associated
            });

            // PERSONNEL
            modelBuilder.Entity<Personnel>(static e => 
            {
                // Primary key
                e.HasKey(p => p.personnelId);

                // Column properties
                e.Property(p => p.personnelId)
                    .IsRequired()
                    .ValueGeneratedOnAdd(); // Auto-incremented ID
                e.Property(p => p.accountId)
                    .IsRequired();
                e.Property(p => p.facilityId)
                    .IsRequired();
                e.Property(p => p.pwzNumber)
                    .IsRequired()
                    .HasMaxLength(50);
                e.Property(p => p.position)
                    .IsRequired()
                    .HasConversion<string>() // Store enum as string
                    .HasMaxLength(50);

                // Relations
                e.HasOne(p => p.account)
                    .WithOne()
                    .HasForeignKey<Personnel>(p => p.accountId)
                    .OnDelete(DeleteBehavior.Cascade); // When an Account is deleted, the associated Personnel is also deleted

                e.HasOne(p => p.medicalFacility)
                    .WithMany(f => f.personnels)
                    .HasForeignKey(p => p.facilityId)
                    .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of MedicalFacility if Personnel are associated
            });

            // MEDICAL FACILITY
            modelBuilder.Entity<MedicalFacility>(static e =>
            {
                // Primary key
                e.HasKey(mf => mf.facilityId);

                // Column properties
                e.Property(mf => mf.facilityId)
                    .IsRequired()
                    .ValueGeneratedOnAdd(); // Auto-incremented ID
                e.Property(mf => mf.facilityName)
                    .IsRequired()
                    .HasMaxLength(100);
                e.Property(mf => mf.street)
                    .IsRequired()
                    .HasMaxLength(200);
                e.Property(mf => mf.city)
                    .IsRequired()
                    .HasMaxLength(200);
                e.Property(mf => mf.country)
                    .IsRequired()
                    .HasMaxLength(200);
                e.Property(mf => mf.nip)
                    .IsRequired()
                    .HasMaxLength(20);
                e.Property(mf => mf.creationDate)
                    .IsRequired();
            });

            // OCR SUBMISSION
            modelBuilder.Entity<OcrSubmission>(static e =>
            {
                // Primary key
                e.HasKey(os => os.submissionId);

                // Column properties
                e.Property(os => os.submissionId)
                    .IsRequired()
                    .ValueGeneratedOnAdd(); // Auto-incremented ID
                e.Property(os => os.citizenId)
                    .IsRequired();
                e.Property(os => os.photoUrl)
                    .IsRequired()
                    .HasMaxLength(255);
                e.Property(os => os.status)
                    .IsRequired()
                    .HasConversion<string>() // Store enum as string
                    .HasMaxLength(20);
                e.Property(os => os.submissionDate)
                    .IsRequired();

                // Relations
                e.HasOne(os => os.citizen)
                    .WithMany(c => c.ocrSubmissions)
                    .HasForeignKey(os => os.citizenId)
                    .OnDelete(DeleteBehavior.Cascade); // When a Citizen is deleted, their OCR submissions are also deleted

                e.HasOne(os => os.verifier)
                    .WithMany(p => p.ocrSubmissions)
                    .HasForeignKey(os => os.verifierId)
                    .OnDelete(DeleteBehavior.NoAction); // If Personnel is deleted, set verifierId to null in OcrSubmission
            });

            // VACCINATION ENTRY
            modelBuilder.Entity<VaccinationEntry>(static e =>
            {
                // Primary key
                e.HasKey(ve => ve.vaccinationEntryId);

                // Column properties
                e.Property(ve => ve.vaccinationEntryId)
                    .IsRequired()
                    .ValueGeneratedOnAdd(); // Auto-incremented ID
                e.Property(ve => ve.ocrSubmissionId)
                    .IsRequired();
                e.Property(ve => ve.batchOcr)
                    .IsRequired()
                    .HasMaxLength(40);
                e.Property(ve => ve.vaccineNameOcr)
                    .IsRequired()
                    .HasMaxLength(300);
                e.Property(ve => ve.dateOcr)
                    .IsRequired()
                    .HasMaxLength(100);
                e.Property(ve => ve.batch)
                    .HasMaxLength(100);

                // Relations
                e.HasOne(ve => ve.ocrSubmission)
                    .WithMany(os => os.vaccinationEntries)
                    .HasForeignKey(ve => ve.ocrSubmissionId)
                    .OnDelete(DeleteBehavior.Cascade); // When an OcrSubmission is deleted, its VaccinationEntries are also deleted
                
                e.HasOne(ve => ve.vaccineDefinition)
                    .WithMany(vd => vd.vaccinationEntries)
                    .HasForeignKey(ve => ve.vaccineId)
                    .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of VaccineDefinition if VaccinationEntries are associated

                e.HasOne(ve => ve.vaccinationRecord)
                    .WithOne(vr => vr.sourceEntry)
                    .HasForeignKey<VaccinationRecord>(vr => vr.sourceEntryId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // VACCINATION RECORD
            modelBuilder.Entity<VaccinationRecord>(static e =>
            {
                // Primary key
                e.HasKey(vr => vr.recordId);

                // Column properties
                e.Property(vr => vr.recordId)
                    .IsRequired()
                    .ValueGeneratedOnAdd(); // Auto-incremented ID
                e.Property(vr => vr.citizenId)
                    .IsRequired();
                e.Property(vr => vr.vaccineId)
                    .IsRequired();
                e.Property(vr => vr.vaccinationDate)
                    .IsRequired();
                e.Property(vr => vr.doseNumber)
                    .IsRequired()
                    .HasMaxLength(20);
                e.Property(vr => vr.batch)
                    .IsRequired()
                    .HasMaxLength(50);
                e.Property(vr => vr.sourceEntryId)
                    .IsRequired();
                e.Property(vr => vr.creationDate)
                    .IsRequired();

                // Relations
                e.HasOne(e => e.citizen)
                    .WithMany(c => c.vaccinationRecords)
                    .HasForeignKey(vr => vr.citizenId)
                    .OnDelete(DeleteBehavior.Cascade); // When a Citizen is deleted, their VaccinationRecords are also deleted
                
                e.HasOne(vr => vr.vaccineDefinition)
                    .WithMany(vd => vd.vaccinationRecords)
                    .HasForeignKey(vr => vr.vaccineId)
                    .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of VaccineDefinition if VaccinationRecords are associated
            });

            // VACCINE DEFINITION
            modelBuilder.Entity<VaccineDefinition>(static e =>
            {
                // Primary key
                e.HasKey(vd => vd.vaccineId);

                // Column properties
                e.Property(vd => vd.vaccineId)
                    .IsRequired()
                    .ValueGeneratedOnAdd(); // Auto-incremented ID
                e.Property(vd => vd.vaccineName)
                    .IsRequired()
                    .HasMaxLength(500);
                e.Property(vd => vd.manufacturer)
                    .IsRequired()
                    .HasMaxLength(100);
                e.Property(vd => vd.vaccineDescription)
                    .IsRequired()
                    .HasMaxLength(500);
                e.Property(vd => vd.creationDate)
                    .IsRequired();
            });

            // VACCINE DISEASE
            modelBuilder.Entity<VaccineDisease>(static e =>
            {
                // Primary key
                e.HasKey(vd => new { vd.vaccineId, vd.diseaseId });

                // Column properties
                e.Property(vd => vd.vaccineId)
                    .IsRequired();
                e.Property(vd => vd.diseaseId)
                    .IsRequired();


                // Relations
                e.HasOne(vd => vd.vaccineDefinition)
                    .WithMany(v => v.vaccineDiseases)
                    .HasForeignKey(vd => vd.vaccineId)
                    .OnDelete(DeleteBehavior.Cascade); // When a VaccineDefinition is deleted, its VaccineDiseases are also deleted

                e.HasOne(vd => vd.disease)
                    .WithMany(d => d.vaccineDiseases)
                    .HasForeignKey(vd => vd.diseaseId)
                    .OnDelete(DeleteBehavior.Cascade); // When a Disease is deleted, its VaccineDiseases are also deleted
            });

            // DISEASE
            modelBuilder.Entity<Disease>(static e =>
            {
                // Primary key
                e.HasKey(d => d.diseaseId);

                // Column properties
                e.Property(d => d.diseaseId)
                    .IsRequired()
                    .ValueGeneratedOnAdd(); // Auto-incremented ID
                e.Property(d => d.diseaseNameEn)
                    .IsRequired()
                    .HasMaxLength(500);
                e.Property(d => d.diseaseNamePl)
                    .IsRequired()
                    .HasMaxLength(500);
            });
        }
    }
}
