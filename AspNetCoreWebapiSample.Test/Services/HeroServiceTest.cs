using AspNetCoreWebapiSample.Domain.Entities;
using AspNetCoreWebapiSample.Domain.Exceptions;
using AspNetCoreWebapiSample.Domain.Interfaces.Repository;
using AspNetCoreWebapiSample.Domain.Interfaces.Service;
using AspNetCoreWebapiSample.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;

namespace AspNetCoreWebapiSample.Test.Services
{
    public class HeroServiceTest
    {
        private readonly HeroService _serviceTest;
        private Mock<IHeroRepository> _heroRepositoryMock { get; }
        private Mock<ISuperPowerService> _superPowerServiceMock { get; }


        public HeroServiceTest()
        {
            _heroRepositoryMock = new Mock<IHeroRepository>();
            _superPowerServiceMock = new Mock<ISuperPowerService>();
            _serviceTest = new HeroService(_heroRepositoryMock.Object, _superPowerServiceMock.Object);
        }

        [Fact]
        public async void GetAll_ShouldReturnAllHeroes()
        {
            IEnumerable<Hero> expected = new List<Hero> {
                new Hero{Name="Batman"},
                new Hero{Name="Jaspion"},
                new Hero{Name="Spiderman"},
            };

            _heroRepositoryMock.Setup(t => t.GetAllAsync())
                .ReturnsAsync(expected);

            var result = await _serviceTest.GetAllAsync();
            Assert.Same(expected, result);
            _heroRepositoryMock.Verify(t => t.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async void GetById_ShouldReturnHero()
        {
            int id = 1;
            var expected = new Hero { Id = 1 };
            _heroRepositoryMock.Setup(t => t.GetByIdAsync(id))
                .ReturnsAsync(expected)
                .Verifiable();

            var result = await _serviceTest.GetByIdAsync(id);

            Assert.Equal(expected, result);
            _heroRepositoryMock.Verify(t => t.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async void GetById_ShouldReturnNotFoundException()
        {
            int id = 1;
            Hero expected = null;
            _heroRepositoryMock.Setup(t => t.GetByIdAsync(id))
                .ReturnsAsync(expected)
                .Verifiable();

            await Assert.ThrowsAsync<CustomNotFoundException>(() => _serviceTest.GetByIdAsync(id));
            _heroRepositoryMock.Verify(t => t.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async void Create_ShouldCreateAndReturnHeroCreated()
        {
            var expected = new Hero();

            _heroRepositoryMock.Setup(t => t.InsertAsync(expected))
                .ReturnsAsync(expected)
                .Verifiable();

            _superPowerServiceMock.Setup(t => t.IsIdExistsAsync(It.IsAny<int>()))
                .ReturnsAsync(true)
                .Verifiable();

            var result = await _serviceTest.InsertAsync(expected);

            Assert.Same(expected, result);
            _heroRepositoryMock.Verify(t => t.InsertAsync(expected), Times.Once);
            _superPowerServiceMock.Verify(t => t.IsIdExistsAsync(It.IsAny<int>()), Times.Once);
            _heroRepositoryMock.Verify(t => t.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async void Create_ShouldReturnFieldExistsException()
        {
            var obj = new Hero();

            _heroRepositoryMock.Setup(t => t.IsItExistsAsync(It.IsAny<Expression<Func<Hero, bool>>>()))
                .ReturnsAsync(true)
                .Verifiable();

            await Assert.ThrowsAsync<CustomFieldAlreadyExistsException>(() => _serviceTest.InsertAsync(obj));
            _heroRepositoryMock.Verify(t => t.IsItExistsAsync(It.IsAny<Expression<Func<Hero, bool>>>()), Times.Once);
            _heroRepositoryMock.Verify(t => t.InsertAsync(obj), Times.Never);
            _heroRepositoryMock.Verify(t => t.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async void Update_ShouldUpdateAndReturnHeroUpdated()
        {
            var expected = new Hero();
            expected.Id = 1;

            _heroRepositoryMock.Setup(t => t.Update(expected))
                .Verifiable();

            _superPowerServiceMock.Setup(t => t.IsIdExistsAsync(It.IsAny<int>()))
                .ReturnsAsync(true)
                .Verifiable();

            var result = await _serviceTest.UpdateAsync(expected);
            Assert.Same(expected, result);

            _heroRepositoryMock.Verify(t => t.Update(expected), Times.Once);
            _heroRepositoryMock.Verify(t => t.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async void Update_ShouldReturnFieldException()
        {
            var expected = new Hero();

            _heroRepositoryMock.Setup(t => t.IsItExistsAsync(It.IsAny<Expression<Func<Hero, bool>>>()))
                .ReturnsAsync(true)
                .Verifiable();

            await Assert.ThrowsAsync<CustomFieldAlreadyExistsException>(() => _serviceTest.UpdateAsync(expected));
            _heroRepositoryMock.Verify(t => t.Update(expected), Times.Never);
            _heroRepositoryMock.Verify(t => t.IsItExistsAsync(It.IsAny<Expression<Func<Hero, bool>>>()), Times.Once);
            _heroRepositoryMock.Verify(t => t.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async void Delete_ShouldDeleteHero()
        {
            int heroId = 1;
            _heroRepositoryMock.Setup(t => t.Delete(It.IsAny<Hero>()))
                .Verifiable();

            await _serviceTest.DeleteAsync(heroId);

            _heroRepositoryMock.Verify(t => t.Delete(It.IsAny<Hero>()), Times.Once);
            _heroRepositoryMock.Verify(t => t.SaveChangesAsync(), Times.Once);
        }
      
    }
}
