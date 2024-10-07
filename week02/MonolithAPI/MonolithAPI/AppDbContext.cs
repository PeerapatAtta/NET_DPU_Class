using Microsoft.EntityFrameworkCore;
using MonolithAPI.Models;

namespace MonolithAPI;

public class AppDbContext : DbContext
{
    public DbSet<ProductModel> Products { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}
