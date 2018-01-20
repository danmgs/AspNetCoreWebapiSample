using AspNetCoreWebapiSample.Domain.Entities;
using AspNetCoreWebapiSample.Domain.Interfaces.Repository;
using AspNetCoreWebapiSample.Infrastructure.Context;
using AspNetCoreWebapiSample.Infrastructure.Repository.Common;

namespace AspNetCoreWebapiSample.Infrastructure.Repository
{
    public class HeroRepository : Repository<Hero>, IHeroRepository
    {
        public HeroRepository(HeroContext context) : base(context)
        {
        }    
    }
}
