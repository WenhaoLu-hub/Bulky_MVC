using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BulkyBook.DataAccess.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(
            "host=bulky.co6rikke7jlw.ap-southeast-2.rds.amazonaws.com;Port=5432;Username=darwin;Password=1288jjj.;Database=bulky_demo;"
        );
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
