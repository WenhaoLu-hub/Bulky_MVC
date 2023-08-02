using BulkyWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("User ID=darwin;Host=localhost;Port=5432;Database=bulky;Pooling=true;Connection Lifetime=0;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(
            new Category {CategoryId = 1, Name = "Web Development", DisplayOrder = 1},
            new Category {CategoryId = 2, Name = "Application Development", DisplayOrder = 2},
            new Category {CategoryId = 3, Name = "Graphic Design", DisplayOrder = 3});
    }
}