using AspNetCoreWebapiSample.Domain.Entities;
using AspNetCoreWebapiSample.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreWebapiSample.Infrastructure.Context
{
    public class HeroContext : IdentityDbContext<AppUser,AppRole,int>
    {

        public HeroContext(DbContextOptions<HeroContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<SuperPower> SuperPower { get; set; }
        public DbSet<Hero> Hero { get; set; }
        

    }
}
