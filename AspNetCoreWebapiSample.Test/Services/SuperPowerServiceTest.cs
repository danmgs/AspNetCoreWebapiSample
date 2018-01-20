using AspNetCoreWebapiSample.Domain.Entities;
using AspNetCoreWebapiSample.Domain.Exceptions;
using AspNetCoreWebapiSample.Domain.Interfaces.Repository;
using AspNetCoreWebapiSample.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;

namespace AspNetCoreWebapiSample.Test.Services
{
    public class SuperPowerServiceTest
    {
        private readonly SuperPowerService _serviceTest;
        public Mock<ISuperPowerRepository> _superPowerRepositoryMock { get; set; }

        public SuperPowerServiceTest()
        {
            _superPowerRepositoryMock = new Mock<ISuperPowerRepository>();
            _serviceTest = new SuperPowerService(_superPowerRepositoryMock.Object);
        }

        [Fact]
        public async void GetAll_ShouldReturnAllSuperPowers()
        {
            IEnumerable<SuperPower> expected = new List<SuperPower>() {
                    new SuperPower(){Name="xxx"},
                    new SuperPower(){Name="yyy"},
                };

            _superPowerRepositoryMock.Setup(t => t.GetAllAsync())
                .ReturnsAsync(expected).Verifiable();

            var result = await _serviceTest.GetAllAsync();
            Assert.Same(expected, result);
            _superPowerRepositoryMock.Verify(t => t.GetAllAsync(), Times.Once);
        }
        [Fact]
        public async void GetById_ShouldReturnSuperPower()
        {
            int id = 1;
            var expected = new SuperPower();
            _superPowerRepositoryMock.Setup(t => t.GetByIdAsync(id))
                .ReturnsAsync(expected).Verifiable();

            var result = await _serviceTest.GetByIdAsync(id);

            Assert.Equal(expected, result);
            _superPowerRepositoryMock.Verify(t => t.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async void GetById_ShouldReturnNotFoundException()
        {
            int id = 1;
            SuperPower expected = null;
            _superPowerRepositoryMock.Setup(t => t.GetByIdAsync(id))
                .ReturnsAsync(expected)
                .Verifiable();

            await Assert.ThrowsAsync<CustomNotFoundException>(() => _serviceTest.GetByIdAsync(id));
            _superPowerRepositoryMock.Verify(t => t.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async void Create_ShouldCreateAndReturnSuperPowerCreated()
        {
            var expected = new SuperPower();
            _superPowerRepositoryMock.Setup(t => t.InsertAsync(expected))
                .ReturnsAsync(expected)
                .Verifiable();

            var result = await _serviceTest.InsertAsync(expected);

            Assert.Same(expected, result);

            _superPowerRepositoryMock.Verify(t => t.InsertAsync(expected), Times.Once);
            _superPowerRepositoryMock.Verify(t => t.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async void Create_ShouldReturnFieldExistsException()
        {
            var obj = new SuperPower();
            _superPowerRepositoryMock.Setup(t => t.IsItExistsAsync(It.IsAny<Expression<Func<SuperPower, bool>>>()))
                .ReturnsAsync(true)
                .Verifiable();

            await Assert.ThrowsAsync<CustomFieldAlreadyExistsException>(() => _serviceTest.InsertAsync(obj));

            _superPowerRepositoryMock.Verify(t => t.InsertAsync(obj), Times.Never);
            _superPowerRepositoryMock.Verify(t => t.IsItExistsAsync(It.IsAny<Expression<Func<SuperPower, bool>>>()), Times.Once);
            _superPowerRepositoryMock.Verify(t => t.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async void Update_ShouldUpdateAndReturnSuperPowerUpdated()
        {
            var expected = new SuperPower();
            _superPowerRepositoryMock.Setup(t => t.Update(expected))
                .Verifiable();

            var result = await _serviceTest.UpdateAsync(expected);

            Assert.Same(expected, result);

            _superPowerRepositoryMock.Verify(t => t.Update(expected), Times.Once);
            _superPowerRepositoryMock.Verify(t => t.SaveChangesAsync(), Times.Once);
        }


        [Fact]
        public async void Update_ShouldReturnFieldExistsException()
        {
            var expected = new SuperPower();
            _superPowerRepositoryMock.Setup(t => t.Update(expected))
                .Verifiable();

            _superPowerRepositoryMock.Setup(t => t.IsItExistsAsync(It.IsAny<Expression<Func<SuperPower, bool>>>()))
                .ReturnsAsync(true)
                .Verifiable();

            await Assert.ThrowsAsync<CustomFieldAlreadyExistsException>(() => _serviceTest.UpdateAsync(expected));

            _superPowerRepositoryMock.Verify(t => t.IsItExistsAsync(It.IsAny<Expression<Func<SuperPower, bool>>>()), Times.Once);
            _superPowerRepositoryMock.Verify(t => t.Update(expected), Times.Never);
            _superPowerRepositoryMock.Verify(t => t.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async void Delete_ShouldDeleteSuperPower()
        {
            _superPowerRepositoryMock.Setup(t => t.Delete(It.IsAny<SuperPower>()))
                .Verifiable();

            await _serviceTest.DeleteAsync(1);

            _superPowerRepositoryMock.Verify(t => t.Delete(It.IsAny<SuperPower>()), Times.Once);
            _superPowerRepositoryMock.Verify(t => t.SaveChangesAsync(), Times.Once);
        }


    }
}
