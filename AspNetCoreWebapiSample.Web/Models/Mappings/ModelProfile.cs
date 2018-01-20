using AspNetCoreWebapiSample.Domain.Entities;
using AutoMapper;
namespace AspNetCoreWebapiSample.Web.Models.Mappings
{
    public class ModelProfile : Profile
    {
        public ModelProfile()
        {
            CreateMap<HeroModel, Hero>()
                .ForMember(dest => dest.SuperPower, opt => opt.Ignore());

            CreateMap<SuperPowerModel, SuperPower>();
        }
    }
}
