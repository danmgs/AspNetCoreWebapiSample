using AspNetCoreWebapiSample.Domain.Entities;
using AutoMapper;

namespace AspNetCoreWebapiSample.Web.Models.Mappings
{
    public class DomainProfile : Profile
    {
        public DomainProfile()
        {
            CreateMap<Hero, HeroModel>();
            CreateMap<SuperPower, SuperPowerModel>();
                
        }
    }
}
