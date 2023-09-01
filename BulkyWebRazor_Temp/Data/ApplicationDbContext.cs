using System;
using BulkyWebRazor_Temp.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyWebRazor_Temp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options) { }

        public DbSet<Category> Categories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(
                "User ID=darwin;Host=localhost;Port=5432;Database=bulky_razor;Connection Lifetime=0;"
            );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Category>()
                .HasData(
                    new Category
                    {
                        CategoryId = 1,
                        Name = "Action",
                        DisplayOrder = 1
                    },
                    new Category
                    {
                        CategoryId = 2,
                        Name = "Controller",
                        DisplayOrder = 2
                    },
                    new Category
                    {
                        CategoryId = 3,
                        Name = "History",
                        DisplayOrder = 3
                    }
                );
        }

        internal void Find(int? id)
        {
            throw new NotImplementedException();
        }
    }
}
