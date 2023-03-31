using Microsoft.EntityFrameworkCore;

namespace AsyncApi.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<ListingRequest> ListingRequests => Set<ListingRequest>();
}