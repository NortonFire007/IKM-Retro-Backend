using Microsoft.EntityFrameworkCore;

namespace IKM_Retro.Data
{
    public class RetroDbContext(DbContextOptions<RetroDbContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}