using api.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api.Data;

// IdentityDbContext<AppUser> brings in all the Identity tables
// (AspNetUsers, AspNetRoles, AspNetUserClaims, ...) configured for our
// AppUser. Add your own DbSet<T> properties below for trade entities later.
public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // public DbSet<Trade> Trades { get; set; }
}
