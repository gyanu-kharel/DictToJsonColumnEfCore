using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace store.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users => Set<User>();
    public virtual DbSet<Company> Companies => Set<Company>();
    public virtual DbSet<CompanyProp> CompanyProps => Set<CompanyProp>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<CompanyProp>()
            .Property(p => p.Values)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions) null),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v,(JsonSerializerOptions) null));
    }
}