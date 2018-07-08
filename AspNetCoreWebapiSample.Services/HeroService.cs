using AspNetCoreWebapiSample.Domain.Entities;
using AspNetCoreWebapiSample.Domain.Interfaces.Repository;
using AspNetCoreWebapiSample.Domain.Interfaces.Service;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCoreWebapiSample.Domain.Exceptions;

namespace AspNetCoreWebapiSample.Services
{
    public class HeroService : IHeroService
    {
        private readonly IHeroRepository _heroRepository;
        private readonly ISuperPowerService _superPowerService;

        public HeroService(IHeroRepository heroRepository, ISuperPowerService superPowerService)
        {
            _heroRepository = heroRepository ?? throw new ArgumentNullException(nameof(heroRepository));
            _superPowerService = superPowerService ?? throw new ArgumentNullException(nameof(superPowerService));
        }

        public async Task DeleteAsync(int id)
        {
            var hero = await _heroRepository.GetByIdAsync( id );
            _heroRepository.Delete(hero);
            await _heroRepository.SaveChangesAsync();
        }

        public Task<IEnumerable<Hero>> GetAllAsync()
        {
            return _heroRepository.GetAllAsync();
        }

        public async Task<Hero> GetByIdAsync(int id)
        {
            var obj = await _heroRepository.GetByIdAsync(id);
            if (obj == null)
                throw new CustomNotFoundException("Hero");
            return obj;
        }

        public async Task<Hero> InsertAsync(Hero obj)
        {
            if (await _heroRepository.ExistsAsync(t => t.Code == obj.Code))
                throw new CustomFieldAlreadyExistsException("code");

            if (!await _superPowerService.IsIdExistsAsync(obj.SuperPowerId))
                throw new CustomNotFoundException("Super Power");

            obj.CreateDate = DateTime.Now;
            obj.UpdateDate = DateTime.Now;

            var result = await _heroRepository.InsertAsync(obj);
            await _heroRepository.SaveChangesAsync();
            return result;
        }

        public async Task<bool> IsIdExistsAsync(int id)
        {
            return await _heroRepository.ExistsAsync(t => t.Id == id);            
        }

        public async Task<Hero> UpdateAsync(Hero obj)
        {
            if (await _heroRepository.ExistsAsync(t => t.Code == obj.Code && t.Id != obj.Id))
                throw new CustomFieldAlreadyExistsException("code");

            if (!await _superPowerService.IsIdExistsAsync(obj.SuperPowerId))
                throw new CustomNotFoundException("Super Power");

            obj.UpdateDate = DateTime.Now;

            _heroRepository.Update(obj);
            await _heroRepository.SaveChangesAsync();
            return obj;
        }


    }
}
