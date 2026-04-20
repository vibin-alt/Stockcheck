using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<LoginResponse> LoginResponses { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LoginResponse>()
                    .HasNoKey()
                    .Ignore(x => x.Data);     // Ignore navigation issue

        base.OnModelCreating(modelBuilder);
    }
}