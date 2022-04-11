using Commons.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace Commons.Utils.Database
{
    public class RidelyDbContext : DbContext
    {
        public RidelyDbContext(DbContextOptions<RidelyDbContext> options) : base(options)
        {   }

        public DbSet<Visit> Visits { get; set; }
    }
}
