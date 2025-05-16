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
    public class ProductControllerTests
    {
        private readonly IUnitOfWork _fakeUnitOfWork;
        private readonly IMapper _fakeMapper;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            _fakeUnitOfWork = A.Fake<IUnitOfWork>();
            _fakeMapper = A.Fake<IMapper>();
            _controller = new ProductController(_fakeUnitOfWork, _fakeMapper);
        }

        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            //Arrange
            var products = new List<Product> { new Product { Id = 1 }, new Product { Id = 2 } };
            var readDto = new List<ProductReadDto> { new ProductReadDto(), new ProductReadDto() };

            A.CallTo(() => _fakeUnitOfWork.Product.GetAllAsync(A<Expression<Func<Product, bool>>>._, "Supplier"))
                .Returns(products);

            A.CallTo(() => _fakeMapper.Map<IEnumerable<ProductReadDto>>(products)).Returns(readDto);


            //Act
            var result = await _controller.GetAll();


            //Assert
            result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(readDto);
        }



        [Fact]
        public async Task GetById_ReturnsOk()
        {
            //Arrange
            var product = new Product { Id = 1 };
            var readDto = new ProductReadDto();

            A.CallTo(() => _fakeUnitOfWork.Product.GetOneAsync(A<Expression<Func<Product, bool>>>._, "Supplier")).Returns(product);
            A.CallTo(() => _fakeMapper.Map<ProductReadDto>(product)).Returns(readDto);


            //Act
            var result = await _controller.GetById(1);


            //Assert
            result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(readDto);
        }



        [Fact]
        public async Task GetById_ReturnsNotFound()
        {
            //Arrange
            A.CallTo(() => _fakeUnitOfWork.Product.GetOneAsync(A<Expression<Func<Product, bool>>>._, "Supplier")).Returns((Product)null);

            //Act
            var result = await _controller.GetById(999);

            //Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }



        [Fact]
        public async Task Create_ReturnsCreated()
        {
            //Arrange
            var writeDto = new ProductWriteDto();
            var product = new Product { Id = 10 };
            var dto = new ProductReadDto();

            A.CallTo(() => _fakeMapper.Map<Product>(writeDto)).Returns(product);
            A.CallTo(() => _fakeMapper.Map<ProductReadDto>(product)).Returns(dto);

            //Act
            var result = await _controller.Create(writeDto);

            //Assert
            A.CallTo(() => _fakeUnitOfWork.Product.AddAsync(product)).MustHaveHappened();
            A.CallTo(() => _fakeUnitOfWork.SaveAsync()).MustHaveHappened();
            result.Should().BeOfType<CreatedAtActionResult>().Which.Value.Should().Be(dto);
        }



        [Fact]
        public async Task Create_ReturnsCreated2()
        {
            // Arrange
            var writeDto = new ProductWriteDto { Name = "test" };
            var product = new Product();
            var readDto = new ProductReadDto();

            A.CallTo(() => _fakeMapper.Map<Product>(writeDto)).Returns(product);
            A.CallTo(() => _fakeMapper.Map<ProductReadDto>(product)).Returns(readDto);

            // Act
            var result = await _controller.Create(writeDto);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>()
                  .Which.Value.Should().BeOfType<ProductReadDto>();

            A.CallTo(() => _fakeUnitOfWork.Product.AddAsync(product)).MustHaveHappened();
            A.CallTo(() => _fakeUnitOfWork.SaveAsync()).MustHaveHappened();
        }





        [Fact]
        public async Task Update_ReturnsNoContent()
        {
            //Arrange
            var writeDto = new ProductWriteDto();
            var product = new Product { Id = 1 };

            A.CallTo(() => _fakeUnitOfWork.Product.GetOneAsync(A<Expression<Func<Product, bool>>>._, null)).Returns(product);

            //Act
            var result = await _controller.Update(1, writeDto);

            //Assert
            A.CallTo(() => _fakeMapper.Map(writeDto, product)).MustHaveHappened();
            A.CallTo(() => _fakeUnitOfWork.SaveAsync()).MustHaveHappened();
            result.Should().BeOfType<NoContentResult>();
        }



        [Fact]
        public async Task Update_ReturnsNoContent2()
        {
            //Arrange
            var writeDto = new ProductWriteDto { Name = "test" };
            var product = new Product { Id = 1 };

            A.CallTo(() => _fakeUnitOfWork.Product.GetOneAsync(A<Expression<Func<Product, bool>>>._, null)).Returns(product);

            //Act
            var result = await _controller.Update(1, writeDto);

            //Assert
            A.CallTo(() => _fakeMapper.Map(writeDto, product)).MustHaveHappened();
            A.CallTo(() => _fakeUnitOfWork.Product.Update(A<Product>._)).MustHaveHappened();
            A.CallTo(() => _fakeUnitOfWork.SaveAsync()).MustHaveHappened();
            result.Should().BeOfType<NoContentResult>();
        }



        [Fact]
        public async Task Update_ReturnsNotFound()
        {
            //Arrange
            A.CallTo(() => _fakeUnitOfWork.Product.GetOneAsync(A<Expression<Func<Product, bool>>>._, null)).Returns((Product)null);

            //Act
            var result = await _controller.Update(1, new ProductWriteDto());

            //Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }



        [Fact]
        public async Task Delete_ReturnsNoContent()
        {
            //Arrange
            var product = new Product { Id = 1 };

            A.CallTo(() => _fakeUnitOfWork.Product.GetOneAsync(A<Expression<Func<Product, bool>>>._, null)).Returns(product);

            //Act
            var result = await _controller.Delete(1);

            //Assert
            A.CallTo(() => _fakeUnitOfWork.Product.Remove(product)).MustHaveHappened();
            A.CallTo(() => _fakeUnitOfWork.SaveAsync()).MustHaveHappened();
            result.Should().BeOfType<NoContentResult>();
        }



        [Fact]
        public async Task Delete_ReturnsNotFound()
        {
            //Arrange
            A.CallTo(() => _fakeUnitOfWork.Product.GetOneAsync(A<Expression<Func<Product, bool>>>._, null)).Returns((Product)null);

            //Act
            var result = await _controller.Delete(1);

            //Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }



        [Fact]
        public async Task DeleteAll_ValidIds_ReturnsOk()
        {
            //Arrange
            var ids = new List<int> { 1, 2 };
            var products = new List<Product> { new Product { Id = 1 }, new Product { Id = 2 } };

            A.CallTo(() => _fakeUnitOfWork.Product.GetAllAsync(A<Expression<Func<Product, bool>>>._, null))
                .Returns(products);

            //Act
            var result = await _controller.DeleteAll(ids);

            //Assert
            A.CallTo(() => _fakeUnitOfWork.Product.RemoveRange(products)).MustHaveHappened();
            A.CallTo(() => _fakeUnitOfWork.SaveAsync()).MustHaveHappened();
            result.Should().BeOfType<OkObjectResult>();
        }



        [Fact]
        public async Task DeleteAll_ReturnsBadRequest()
        {
            //Arrange
            var ids = new List<int>();      //empty list

            //Act
            var result = await _controller.DeleteAll(ids);

            //Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }



        [Fact]
        public async Task DeleteAll_NoMatchingProducts_ReturnsNotFound()
        {
            //Arrange
            A.CallTo(() => _fakeUnitOfWork.Product.GetAllAsync(A<Expression<Func<Product, bool>>>._, null))
                .Returns(new List<Product>());

            //Act
            var result = await _controller.DeleteAll(new List<int> { 999 });

            //Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }






    }



}
