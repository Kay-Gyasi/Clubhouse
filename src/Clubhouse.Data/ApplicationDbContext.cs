using Clubhouse.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Clubhouse.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<BillEntry> BillEntries => Set<BillEntry>();
    public DbSet<Bill> Bill => Set<Bill>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
