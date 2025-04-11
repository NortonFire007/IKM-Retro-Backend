using Microsoft.EntityFrameworkCore;
using IKM_Retro.Data;

namespace IKM_Retro.Extensions
{
    public static class MigrationExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();
            using RetroDbContext dbContext = scope.ServiceProvider.GetRequiredService<RetroDbContext>();
            dbContext.Database.Migrate();
        }
    }
}