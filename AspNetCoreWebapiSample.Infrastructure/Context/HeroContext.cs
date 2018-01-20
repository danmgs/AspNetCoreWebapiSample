using AspNetCoreWebapiSample.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreWebapiSample.Infrastructure.Context
{
    public class HeroContext : DbContext
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
