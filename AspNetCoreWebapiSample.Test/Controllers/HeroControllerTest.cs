using AspNetCoreWebapiSample.Domain.Entities;
using AspNetCoreWebapiSample.Domain.Exceptions;
using AspNetCoreWebapiSample.Domain.Interfaces.Service;
using AspNetCoreWebapiSample.Web.Controllers;
using AspNetCoreWebapiSample.Web.Models;
using AspNetCoreWebapiSample.Web.Models.Mappings;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace AspNetCoreWebapiSample.Test.Controllers
{
    public class HeroControllerTest
    {
        private IMapper _mapper;
        private Mock<IHeroService> _heroServiceMock { get; }
        private HeroController _controllerTest { get; }

        public HeroControllerTest()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DomainProfile());
                cfg.AddProfile(new ModelProfile());
            });
            _mapper = config.CreateMapper();

            _heroServiceMock = new Mock<IHeroService>();
            _controllerTest = new HeroController(_heroServiceMock.Object, _mapper);
        }

        [Fact]
        public async void GetAll_ShouldReturnOkObjectResult()
        {
            var expected = new List<Hero> {
                new Hero{Name="Batman"},
                new Hero{Name="Superman"},
            };

            var expectedFromController = _mapper.Map<List<HeroModel>>(expected);

            _heroServiceMock.Setup(t => t.GetAllAsync())
                .ReturnsAsync(expected);

            var result = await _controllerTest.GetAll();
            var objResult = Assert.IsType<OkObjectResult>(result).Value as List<HeroModel>;

            Assert.Equal(expectedFromController.Count, objResult.Count);
            Assert.Equal(expectedFromController[1].Name, objResult[1].Name);
        }

        [Fact]
        public async void GetById_ShouldReturnOkObjectResult()
        {
            var expected = new Hero { Id = 1, Name = "batman" };
            _heroServiceMock.Setup(t => t.GetByIdAsync(1))
                .ReturnsAsync(expected);

            var result = await _controllerTest.GetById(1);
            var objResult = Assert.IsType<OkObjectResult>(result).Value as HeroModel;

            Assert.Equal(1, objResult.Id);
            Assert.Equal("batman", objResult.Name);
        }

        [Fact]
        public async void GetById_ShouldReturnNotFound()
        {

            _heroServiceMock.Setup(t => t.GetByIdAsync(1))
                .ThrowsAsync(new CustomNotFoundException("Hero"));
            var result = await _controllerTest.GetById(1);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void Create_ShouldReturnCreatedAtActionResult()
        {
            var expectedId = 1;
            var expectedModel = new HeroModel { Name = "batman" };
            var hero = _mapper.Map<Hero>(expectedModel);

            var expectedActionName = nameof(_controllerTest.GetById);

            _heroServiceMock.Setup(t => t.InsertAsync(It.IsAny<Hero>()))
                .ReturnsAsync(() =>
                {
                    hero.Id = expectedId;
                    return hero;
                });

            var result = await _controllerTest.Create(expectedModel);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(expectedId, ((HeroModel)createdResult.Value).Id);
            Assert.Equal(expectedActionName, createdResult.ActionName);
            _heroServiceMock.Verify(t => t.InsertAsync(It.IsAny<Hero>()), Times.Once);
        }

        [Fact]
        public async void Create_ShouldReturnBadRequest()
        {
            var expected = new HeroModel { Name = "batman" };
            _controllerTest.ModelState.AddModelError("k", "v");

            var result = await _controllerTest.Create(expected);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
            _heroServiceMock.Verify(t => t.InsertAsync(It.IsAny<Hero>()), Times.Never);
        }

        [Fact]
        public async void Create_ShouldReturnBadRequestCustomFieldAlreadyExistsException()
        {
            var expected = new HeroModel { Name = "batman" };

            _heroServiceMock.Setup(t => t.InsertAsync(It.IsAny<Hero>()))
                .ThrowsAsync(new CustomFieldAlreadyExistsException(""));

            var result = await _controllerTest.Create(expected);
            Assert.IsType<BadRequestObjectResult>(result);
            
        }
        
        [Fact]
        public async void Update_ShouldReturnOkObjectResult()
        {
            var expectedModel = new HeroModel { Name = "batman" };
            var hero = _mapper.Map<Hero>(expectedModel);

            _heroServiceMock.Setup(t => t.GetByIdAsync(1))
                .ReturnsAsync(hero)
                .Verifiable();

            _heroServiceMock.Setup(t => t.UpdateAsync(hero))
                .ReturnsAsync(hero)
                .Verifiable();

            var result = await _controllerTest.Update(1, expectedModel);

            var objectResult = Assert.IsType<OkObjectResult>(result).Value as HeroModel;
            Assert.Equal(expectedModel.Id, objectResult.Id);
            _heroServiceMock.Verify(t => t.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _heroServiceMock.Verify(t => t.UpdateAsync(It.IsAny<Hero>()), Times.Once);
        }

        [Fact]
        public async void Update_ShouldReturnNotFound()
        {
            var model = new HeroModel();
            var hero = new Hero();

            _heroServiceMock.Setup(t => t.GetByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new CustomNotFoundException("Hero"));

            var result = await _controllerTest.Update(1, model);
            Assert.IsType<NotFoundResult>(result);
            _heroServiceMock.Verify(t => t.GetByIdAsync(It.IsAny<int>()), Times.Once);
        }
        
        [Fact]
        public async void Update_ShouldReturnBadRequest()
        {
            var obj = new HeroModel();

            _controllerTest.ModelState.AddModelError("k", "v");

            var result = await _controllerTest.Update(1,obj);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
            _heroServiceMock.Verify(t => t.GetByIdAsync(It.IsAny<int>()), Times.Never);
            _heroServiceMock.Verify(t => t.UpdateAsync(It.IsAny<Hero>()), Times.Never);
        }
        
        [Fact]
        public async void Delete_ShouldReturnOkObjectResult()
        {
            _heroServiceMock.Setup(t => t.DeleteAsync(It.IsAny<int>()))
               .Returns(System.Threading.Tasks.Task.FromResult(true))
               .Verifiable();

            var result = await _controllerTest.Delete(1);
            Assert.IsType(typeof(OkResult), result);
            _heroServiceMock.Verify(t => t.DeleteAsync(It.IsAny<int>()), Times.Once);
        }
    }
}
