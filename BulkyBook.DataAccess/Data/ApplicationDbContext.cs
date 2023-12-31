using BulkyBook.Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.DataAccess.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions options)
        : base(options) { }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public DbSet<OrderHeader> OrderHeaders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseNpgsql(
            "host=bulky.co6rikke7jlw.ap-southeast-2.rds.amazonaws.com;Port=5432;Username=darwin;Password=1288jjj.;Database=bulky_demo;"
        );

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
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
                    Name = "Novel",
                    DisplayOrder = 2
                },
                new Category
                {
                    CategoryId = 3,
                    Name = "Science Fiction",
                    DisplayOrder = 3
                }
            );

        modelBuilder
            .Entity<Product>()
            .HasData(
                new Product
                {
                    ProductId = 1,
                    Title = "The Moon and SixPence",
                    Author = "William Somerset Maugham",
                    Description =
                        "The Moon and Sixpence is a novel by W. Somerset Maugham, first published on 15 April 1919. It is told in episodic form by a first-person narrator providing a series of glimpses into the mind and soul of the central character, Charles Strickland, a middle-aged English stockbroker, who abandons his wife and children abruptly to pursue his desire to become an artist. The story is, in part, based on the life of the painter Paul Gauguin.",
                    ISBN = "9781598185218",
                    ListPrice = 5.99,
                    ListPrice50 = 5.29,
                    ListPrice100 = 4.99,
                    CategoryId = 1,
                    ImageUrl = ""
                },
                new Product
                {
                    ProductId = 2,
                    Title = "The Old Man and the Sea",
                    Author = "Ernest Hemingway",
                    Description =
                        "The Old Man and the Sea is a novella written by the American author Ernest Hemingway in 1951 in Cayo Blanco (Cuba), and published in 1952.[1] It was the last major work of fiction written by Hemingway that was published during his lifetime. One of his most famous works, it tells the story of Santiago, an aging Cuban fisherman who struggles with a giant marlin far out in the Gulf Stream off the coast of Cuba.",
                    ISBN = "9780684830490",
                    ListPrice = 5.99,
                    ListPrice50 = 5.29,
                    ListPrice100 = 4.99,
                    CategoryId = 1,
                    ImageUrl = ""
                },
                new Product
                {
                    ProductId = 3,
                    Title = "The Great Gatsby",
                    Author = "F. Scott Fitzgerald",
                    Description =
                        "The Great Gatsby is a 1925 novel by American writer F. Scott Fitzgerald. Set in the Jazz Age on Long Island, near New York City, the novel depicts first-person narrator Nick Carraway's interactions with mysterious millionaire Jay Gatsby and Gatsby's obsession to reunite with his former lover, Daisy Buchanan.",
                    ISBN = "0743273567",
                    ListPrice = 5.99,
                    ListPrice50 = 5.29,
                    ListPrice100 = 4.99,
                    CategoryId = 1,
                    ImageUrl = ""
                }
            );

        modelBuilder
            .Entity<Company>()
            .HasData(
                new Company
                {
                    CompanyId = 1,
                    Name = "Company1",
                    StreetAddress = "123 Main St",
                    City = "New York",
                    State = "NY",
                    PostalCode = "12345",
                    PhoneNumber = "1234567890"
                }
            );
    }
}
