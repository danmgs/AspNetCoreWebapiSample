using AspNetCoreWebapiSample.Domain.Interfaces.Service;
using System.Collections.Generic;
using AspNetCoreWebapiSample.Domain.Entities;
using System.Threading.Tasks;
using AspNetCoreWebapiSample.Domain.Interfaces.Repository;
using AspNetCoreWebapiSample.Domain.Exceptions;
using System;

namespace AspNetCoreWebapiSample.Services
{
    public class SuperPowerService : ISuperPowerService
    {
        ISuperPowerRepository _superPowerRepository;

        public SuperPowerService(ISuperPowerRepository superPowerRepository)
        {
            _superPowerRepository = superPowerRepository;
        }

        public async Task DeleteAsync(int id)
        {
            var obj = await _superPowerRepository.GetByIdAsync(id);
            _superPowerRepository.Delete(obj);
            await _superPowerRepository.SaveChangesAsync();
        }

        public Task<IEnumerable<SuperPower>> GetAllAsync()
        {
            return _superPowerRepository.GetAllAsync();
        }

        public async Task<SuperPower> GetByIdAsync(int id)
        {
            var obj = await _superPowerRepository.GetByIdAsync(id);
            if (obj == null)
                throw new CustomNotFoundException("Super Power");
            return obj;
        }

        public async Task<SuperPower> InsertAsync(SuperPower obj)
        {
            if (await _superPowerRepository.ExistsAsync(t => t.Name == obj.Name))
                throw new CustomFieldAlreadyExistsException("name");

            obj.CreateDate = DateTime.Now;
            obj.UpdateDate = DateTime.Now;

            var result = await _superPowerRepository.InsertAsync(obj);
            await _superPowerRepository.SaveChangesAsync();
            return result;
        }

        public async Task<bool> IsIdExistsAsync(int id)
        {            
            return await _superPowerRepository.ExistsAsync(t => t.Id == id);            
        }

        public async Task<SuperPower> UpdateAsync(SuperPower obj)
        {
            if (await _superPowerRepository.ExistsAsync(t => t.Name == obj.Name && t.Id != obj.Id))
                throw new CustomFieldAlreadyExistsException("name");

            obj.UpdateDate = DateTime.Now;

            _superPowerRepository.Update(obj);
            await _superPowerRepository.SaveChangesAsync();
            return obj;
        }
    }
}
