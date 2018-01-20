using AspNetCoreWebapiSample.Domain.Entities;
using AspNetCoreWebapiSample.Domain.Interfaces.Repository;
using AspNetCoreWebapiSample.Infrastructure.Repository.Common;
using AspNetCoreWebapiSample.Infrastructure.Context;

namespace AspNetCoreWebapiSample.Infrastructure.Repository
{
    public class SuperPowerRepository : Repository<SuperPower>, ISuperPowerRepository
    {
        public SuperPowerRepository(HeroContext context) : base(context)
        {
        }
    }
}
