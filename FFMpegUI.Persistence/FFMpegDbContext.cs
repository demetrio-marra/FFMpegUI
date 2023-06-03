using FFMpegUI.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace FFMpegUI.Persistence
{
    public class FFMpegDbContext : DbContext
    {
        public FFMpegDbContext(DbContextOptions<FFMpegDbContext> options)
         : base(options)
        { }

        public DbSet<FFMpegPersistedProcess> Processes { get; set; }
        public DbSet<FFMpegPersistedProcessItem> ProcessItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            

            modelBuilder.Entity<FFMpegPersistedProcess>()
                .HasMany(p => p.Items)
                .WithOne(i => i.Process)
                .HasForeignKey(i => i.ProcessId);

            modelBuilder.Entity<FFMpegPersistedProcess>().HasKey(p => p.Id);
            modelBuilder.Entity<FFMpegPersistedProcessItem>().HasKey(p => p.Id);
        }
    }
}
