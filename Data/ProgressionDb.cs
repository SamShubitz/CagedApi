using Microsoft.EntityFrameworkCore;
using CagedApi.Models;
class ProgressionDb : DbContext
{
    public ProgressionDb(DbContextOptions<ProgressionDb> options)
    : base(options) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasMany(u => u.ProgressionList)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Progression>()
            .HasMany(p => p.ChordList)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
    }
    public DbSet<User> Users => Set<User>();
    public DbSet<Progression> Progressions => Set<Progression>();
    public DbSet<Chord> Chords => Set<Chord>();
}
