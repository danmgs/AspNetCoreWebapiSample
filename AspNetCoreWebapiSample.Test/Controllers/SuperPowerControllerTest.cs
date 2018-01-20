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
    public class SuperPowerControllerTest
    {
        private IMapper _mapper { get; }
        private Mock<ISuperPowerService> _superPowerServiceMock { get; }
        private SuperPowerController _controllerTest { get; }

        public SuperPowerControllerTest()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DomainProfile());
            });
            _mapper = config.CreateMapper();

            _superPowerServiceMock = new Mock<ISuperPowerService>();
            _controllerTest = new SuperPowerController(_superPowerServiceMock.Object, _mapper);
        }
        

        [Fact]
        public async void GetAll_ShouldReturnOkObjectResult()
        {
            var expected = new List<SuperPower> {
                    new SuperPower{Name="Power X"},
                    new SuperPower{Name="Power Y"},
                };

            var expectedFromController = _mapper.Map<List<SuperPowerModel>>(expected);


            _superPowerServiceMock.Setup(t => t.GetAllAsync())
                .ReturnsAsync(expected);

            var result = await _controllerTest.GetAll();

            var objResult = Assert.IsType<OkObjectResult>(result).Value as List<SuperPowerModel>;

            Assert.Equal(expectedFromController.Count, objResult.Count);
            Assert.Equal(expectedFromController[1].Name, objResult[1].Name);
        }


        [Fact]
        public async void GetById_ShouldReturnOkObjectResult()
        {
            var expected = new SuperPower() { Name = "zzzzzz", Id = 1 };

            _superPowerServiceMock.Setup(t => t.GetByIdAsync(1))
                .ReturnsAsync(expected);

            var result = await _controllerTest.GetById(1);

            var objResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal(1, ((SuperPowerModel)objResult.Value).Id);
            Assert.Equal("zzzzzz", ((SuperPowerModel)objResult.Value).Name);
        }

        [Fact]
        public async void GetById_ShouldReturnNotFound()
        {
            var expected = new SuperPower();
            _superPowerServiceMock.Setup(t => t.GetByIdAsync(0))
                .ThrowsAsync(new CustomNotFoundException("Super Power"));

            var result = await _controllerTest.GetById(0);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async void Create_ShouldReturnCreatedAtActionResult()
        {
            var expectedId = 1;
            var expectedModel = new SuperPowerModel { Name = "power" };
            var superPower = _mapper.Map<SuperPower>(expectedModel);

            var expectedActionName = nameof(_controllerTest.GetById);

            _superPowerServiceMock.Setup(t => t.InsertAsync(It.IsAny< SuperPower>()))
                .ReturnsAsync(() =>
                {
                    superPower.Id = expectedId;
                    return superPower;
                });


            var result = await _controllerTest.Create(expectedModel);
            
            
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);            
            Assert.Equal(expectedId, ((SuperPowerModel)createdResult.Value).Id);
            Assert.Equal(expectedActionName, createdResult.ActionName);

            _superPowerServiceMock.Verify(t => t.InsertAsync(It.IsAny<SuperPower>()), Times.Once);
        }

        [Fact]
        public async void Create_ShouldReturnBadRequest()
        {
            var expected = new SuperPowerModel { Name = "power" };
            _controllerTest.ModelState.AddModelError("k", "v");

            var result = await _controllerTest.Create(expected);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
            _superPowerServiceMock.Verify(t => t.InsertAsync(It.IsAny<SuperPower>()), Times.Never);
        }
        
        [Fact]
        public async void Create_ShouldReturnBadRequestCustomFieldAlreadyExistsException()
        {
            var expected = new SuperPowerModel() ;

            _superPowerServiceMock.Setup(t => t.InsertAsync(It.IsAny<SuperPower>()))
                .ThrowsAsync(new CustomFieldAlreadyExistsException(""));

            var result = await _controllerTest.Create(expected);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async void Update_ShouldReturnOkObjectResult()
        {
            var expectedModel = new SuperPowerModel { Name = "power" };
            SuperPower superPower = _mapper.Map<SuperPower>(expectedModel);

            _superPowerServiceMock.Setup(t => t.GetByIdAsync(1))
                .ReturnsAsync(superPower)
                .Verifiable();

            _superPowerServiceMock.Setup(t => t.UpdateAsync(superPower))
                .ReturnsAsync(superPower)
                .Verifiable();

            var result = await _controllerTest.Update(1,expectedModel);
            
            var objectResult = Assert.IsType<OkObjectResult>(result).Value as SuperPowerModel;            
            Assert.Equal(expectedModel.Id, objectResult.Id);
            _superPowerServiceMock.Verify(t => t.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _superPowerServiceMock.Verify(t => t.UpdateAsync(It.IsAny<SuperPower>()), Times.Once);
        }

        [Fact]
        public async void Update_ShouldReturnNotFound()
        {
            var model = new SuperPowerModel();
            var superPower = new SuperPower();


            _superPowerServiceMock.Setup(t => t.GetByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new CustomNotFoundException("Super Power"));

            var result = await _controllerTest.Update(1,model);
            Assert.IsType<NotFoundResult>(result);
            _superPowerServiceMock.Verify(t => t.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _superPowerServiceMock.Verify(t => t.UpdateAsync(It.IsAny<SuperPower>()), Times.Never);
        }

        [Fact]
        public async void Update_ShouldReturnBadRequest()
        {
            var obj = new SuperPowerModel();

            _controllerTest.ModelState.AddModelError("k", "v");

            var result = await _controllerTest.Update(1,obj);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
            _superPowerServiceMock.Verify(t => t.GetByIdAsync(It.IsAny<int>()), Times.Never);
            _superPowerServiceMock.Verify(t => t.UpdateAsync(It.IsAny<SuperPower>()), Times.Never);
        }

        [Fact]
        public async void Delete_ShouldReturnOkObjectResult()
        {
            _superPowerServiceMock.Setup(t => t.DeleteAsync(It.IsAny<int>()))
                .Returns(System.Threading.Tasks.Task.FromResult(true))
                .Verifiable();

            var result = await _controllerTest.Delete(1);
            Assert.IsType(typeof(OkResult), result);
            _superPowerServiceMock.Verify(t => t.DeleteAsync(It.IsAny<int>()), Times.Once);
        }
    }
}
