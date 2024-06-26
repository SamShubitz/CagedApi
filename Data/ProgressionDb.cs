using Microsoft.EntityFrameworkCore;
using CagedApi.Models;
class ProgressionDb : DbContext
{
    public ProgressionDb(DbContextOptions<ProgressionDb> options)
        : base(options) { }

    public DbSet<Progression> Progressions => Set<Progression>();

    //  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    => optionsBuilder.UseNpgsql("Host=localhost:5432;Database=CagedApiDb");
}
