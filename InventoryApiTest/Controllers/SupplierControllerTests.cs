using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using InventoryAPI.Controllers;
using InventoryData.IRepo;
using InventoryDto;
using InventoryModel.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;



namespace InventoryApiTest.Controllers
{
    public class SupplierControllerTests
    {
        
        private readonly IMapper _fakeMapper;
        private readonly IUnitOfWork _fakeUnitOfWork;
        private readonly SupplierController _controller;


        public SupplierControllerTests()
        {
            _fakeMapper = A.Fake<IMapper>();
            _fakeUnitOfWork = A.Fake<IUnitOfWork>();
            _controller = new SupplierController(_fakeMapper, _fakeUnitOfWork);
        }


        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            //Arrange
            var suppliers = new List<Supplier>() { new Supplier { Id = 1 }, new Supplier { Id = 2 } };
            var readDto = new List<SupplierReadDto>() { new SupplierReadDto(), new SupplierReadDto() };

            A.CallTo(() => _fakeUnitOfWork.Supplier.GetAllAsync(A<Expression<Func<Supplier, bool>>>._, null))
                .Returns(suppliers);
            A.CallTo(() => _fakeMapper.Map<IEnumerable<SupplierReadDto>>(suppliers)).Returns(readDto);

            
            //Act
            var result = await _controller.GetAll();

            //Assert
            result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(readDto);
        }



        [Fact]
        public async Task GetById_ReturnsOk()
        {
            ////Arrange
            var supplier = new Supplier { Id = 1 };
            var readDto = new SupplierReadDto();

            A.CallTo(() => _fakeUnitOfWork.Supplier.GetOneAsync(A<Expression<Func<Supplier, bool>>>._, null)).Returns(supplier);
            A.CallTo(() => _fakeMapper.Map<SupplierReadDto>(supplier)).Returns(readDto);


            //Act
            var result = await _controller.GetById(1);


            //Assert
            result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(readDto);
        }



        [Fact]
        public async Task GetById_ReturnsNotFound()
        {
            //Arrange
            A.CallTo(() => _fakeUnitOfWork.Supplier.GetOneAsync(A<Expression<Func<Supplier, bool>>>._, null)).Returns((Supplier)null);

            //Act
            var result = await _controller.GetById(999);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }



        [Fact]
        public async Task Create_ReturnsCreated()
        {
            //Arrange
            var writeDto = new SupplierWriteDto();
            var supplier = new Supplier { Id = 10 };
            var readDto = new SupplierReadDto();

            A.CallTo(() => _fakeMapper.Map<Supplier>(writeDto)).Returns(supplier);
            A.CallTo(() => _fakeMapper.Map<SupplierReadDto>(supplier)).Returns(readDto);

            //Act
            var result = await _controller.Create(writeDto);

            //Assert
            A.CallTo(() => _fakeUnitOfWork.Supplier.AddAsync(supplier)).MustHaveHappened();
            A.CallTo(() => _fakeUnitOfWork.SaveAsync()).MustHaveHappened();
            result.Should().BeOfType<CreatedAtActionResult>().Which.Value.Should().Be(readDto);
        }



        [Fact]
        public async Task Create_ReturnsCreated2()
        {
            // Arrange
            var writeDto = new SupplierWriteDto { Name = "test" };
            var supplier = new Supplier { Id = 10 };
            var readDto = new SupplierReadDto();

            A.CallTo(() => _fakeMapper.Map<Supplier>(writeDto)).Returns(supplier);
            A.CallTo(() => _fakeMapper.Map<SupplierReadDto>(supplier)).Returns(readDto);

            // Act
            var result = await _controller.Create(writeDto);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>()
                  .Which.Value.Should().BeOfType<SupplierReadDto>();

            A.CallTo(() => _fakeUnitOfWork.Supplier.AddAsync(supplier)).MustHaveHappened();
            A.CallTo(() => _fakeUnitOfWork.SaveAsync()).MustHaveHappened();
        }




        [Fact]
        public async Task Update_ReturnsNoContent()
        {
            //Arrange
            var writeDto = new SupplierWriteDto();
            var supplier = new Supplier { Id = 1 };

            A.CallTo(() => _fakeUnitOfWork.Supplier.GetOneAsync(A<Expression<Func<Supplier, bool>>>._, null)).Returns(supplier);

            //Act
            var result = await _controller.Update(1, writeDto);

            //Assert
            A.CallTo(() => _fakeMapper.Map(writeDto, supplier)).MustHaveHappened();
            A.CallTo(() => _fakeUnitOfWork.SaveAsync()).MustHaveHappened();
            result.Should().BeOfType<NoContentResult>();
        }



        [Fact]
        public async Task Update_ReturnsNoContent2()
        {
            //Arrange
            var writeDto = new SupplierWriteDto { Name = "test" };
            var supplier = new Supplier { Id = 1 };

            A.CallTo(() => _fakeUnitOfWork.Supplier.GetOneAsync(A<Expression<Func<Supplier, bool>>>._, null)).Returns(supplier);

            //Act
            var result = await _controller.Update(1, writeDto);

            //Assert
            A.CallTo(() => _fakeMapper.Map(writeDto, supplier)).MustHaveHappened();
            A.CallTo(() => _fakeUnitOfWork.Supplier.Update(A<Supplier>._)).MustHaveHappened();
            A.CallTo(() => _fakeUnitOfWork.SaveAsync()).MustHaveHappened();
            result.Should().BeOfType<NoContentResult>();
        }




        [Fact]
        public async Task Update_ReturnsNotFound()
        {
            //Arrange
            A.CallTo(() => _fakeUnitOfWork.Supplier.GetOneAsync(A<Expression<Func<Supplier, bool>>>._, null)).Returns((Supplier)null);

            //Act
            var result = await _controller.Update(1, new SupplierWriteDto());

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }



        [Fact]
        public async Task Delete_ReturnsOk()
        {
            //Arrange
            var supplier = new Supplier { Id = 1 };

            A.CallTo(() => _fakeUnitOfWork.Supplier.GetOneAsync(A<Expression<Func<Supplier, bool>>>._, null)).Returns(supplier);

            //Act
            var result = await _controller.Delete(1);

            //Assert
            A.CallTo(() => _fakeUnitOfWork.Supplier.Remove(supplier)).MustHaveHappened();
            A.CallTo(() => _fakeUnitOfWork.SaveAsync()).MustHaveHappened();
            result.Should().BeOfType<OkObjectResult>();
        }



        [Fact]
        public async Task Delete_ReturnsNotFound()
        {
            //Arrange
            A.CallTo(() => _fakeUnitOfWork.Supplier.GetOneAsync(A<Expression<Func<Supplier, bool>>>._, null)).Returns((Supplier)null);

            //Act
            var result = await _controller.Delete(1);

            //Assert
            result.Should().BeOfType<NotFoundResult>();
        }



        [Fact]
        public async Task DeleteAll_ValidIds_ReturnsOk()
        {
            //Arrange
            var ids = new List<int> { 1, 2 };
            var suppliers = new List<Supplier> { new Supplier { Id = 1 }, new Supplier { Id = 2 } };

            A.CallTo(() => _fakeUnitOfWork.Supplier.GetAllAsync(A<Expression<Func<Supplier, bool>>>._, null))
                .Returns(suppliers);

            //Act
            var result = await _controller.DeleteAll(ids);

            //Assert
            A.CallTo(() => _fakeUnitOfWork.Supplier.RemoveRange(suppliers)).MustHaveHappened();
            A.CallTo(() => _fakeUnitOfWork.SaveAsync()).MustHaveHappened();
            result.Should().BeOfType<NoContentResult>();
        }



        [Fact]
        public async Task DeleteAll_ReturnsBadRequest()
        {
            //Arrange
            var ids = new List<int>();      //empty list

            //Act
            var result = await _controller.DeleteAll(ids);

            //Assert
            result.Should().BeOfType<BadRequestResult>();
        }



       










    }
}
