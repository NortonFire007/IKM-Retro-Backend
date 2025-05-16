using IKM_Retro.Models;
using IKM_Retro.Models.Retro;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IKM_Retro.Data;

public class RetroDbContext(DbContextOptions<RetroDbContext> options) : IdentityDbContext<User>(options)
{
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Retrospective> Retrospectives { get; set; }
    public DbSet<RetrospectiveToUser> RetrospectiveToUser { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupItem> GroupItems { get; set; }
    public DbSet<GroupItemVote> GroupItemVotes { get; set; }
    public DbSet<GroupItemComment> GroupItemComments { get; set; }
    public DbSet<RetrospectiveInvite> RetrospectiveInvites { get; set; }
    public DbSet<ActionItem> ActionItems { get; set; }
    public DbSet<ActionItemComment> ActionItemComments { get; set; }
    public object RetrospectiveToUsers { get; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
        
}