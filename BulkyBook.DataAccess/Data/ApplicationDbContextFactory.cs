using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BulkyBook.DataAccess.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(
            "User ID=postgres;Host=localhost;Port=5432;Database=bulky;Pooling=true;Connection Lifetime=0;"
        );
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
