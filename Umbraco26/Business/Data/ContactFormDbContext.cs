using Microsoft.EntityFrameworkCore;
using Umbraco26.Models;

namespace Umbraco26.Business.Data
{
    public class ContactFormDbContext(DbContextOptions<ContactFormDbContext> options) : DbContext(options)
    {
        public DbSet<ContactSubmission> ContactSubmissions => Set<ContactSubmission>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContactSubmission>(submission =>
            {
                submission.ToTable("contactSubmission");
                submission.HasKey(x => x.Id);
                submission.Property(x => x.Id).ValueGeneratedOnAdd();
                submission.Property(x => x.Name).HasMaxLength(ContactSubmission.NameMaxLength).IsRequired();
                submission.Property(x => x.EmailProtected).IsRequired();
                submission.Property(x => x.Comment).HasMaxLength(ContactSubmission.CommentMaxLength).IsRequired();
                submission.Property(x => x.Culture).HasMaxLength(ContactSubmission.CultureMaxLength).IsRequired();

                // SQLite hands dates back without a kind; they are always written as UTC
                submission.Property(x => x.CreatedUtc)
                    .HasConversion(value => value, value => DateTime.SpecifyKind(value, DateTimeKind.Utc));

                submission.HasIndex(x => x.CreatedUtc);
            });
        }
    }
}
