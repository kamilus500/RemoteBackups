using Microsoft.EntityFrameworkCore;
using RemoteBackups.Api.Entities;

namespace RemoteBackups.Api.Persistance
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<FileMetaData> FileMetaDatas { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Files)
                .WithOne(f => f.User);
        }
    }
}
