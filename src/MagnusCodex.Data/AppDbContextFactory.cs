using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MagnusCodex.Data;

/// <summary>
/// Usada apenas pelo CLI do EF Core (dotnet ef migrations add ...).
/// Em runtime o app injeta o contexto via DbContextOptionsBuilder.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Data Source=dev_design.db")
            .Options;

        return new AppDbContext(options);
    }
}
