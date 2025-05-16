using FakeItEasy;
using FluentAssertions;
using InventoryDto;
using InventoryModel.Models;
using InventoryWeb.Controllers;
using InventoryWebTest.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Net;
using System.Text;
using System.Text.Json;




namespace InventoryWebTest.Controllers
{
    public class ProductMvcControllerTest
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private ProductMvcController _controller;

        public ProductMvcControllerTest()
        {
            _httpClientFactory = A.Fake<IHttpClientFactory>();
            _controller = new ProductMvcController(_httpClientFactory);
        }





        #region ClientSideApproach


        [Fact]
        public void Index_ReturnsViewResult()
        {
            //Act
            var result = _controller.Index();

            //Assert
            result.Should().BeOfType<ViewResult>();
        }



        [Fact]
        public async Task GetAll_SuccessStatusCode_ReturnsJsonResult()
        {
            // Arrange
            var products = new List<ProductReadDto>
            {
                new ProductReadDto { Id = 1, Name = "Product 1", Price = 10.0m },
                new ProductReadDto { Id = 2, Name = "Product 2", Price = 20.0m }
            };

            var json = JsonSerializer.Serialize(products);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var client = HttpClientTestHelper.CreateFakeHttpClient(response);
            A.CallTo(() => _httpClientFactory.CreateClient("InventoryAPI")).Returns(client);

            // Act
            var result = await _controller.GetAll() as JsonResult;

            // Assert
            result.Should().NotBeNull();

            // Extract the anonymous object as JSON and parse "data"
            var serialized = JsonSerializer.Serialize(result.Value);
            var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(serialized);

            dict.Should().ContainKey("data");

            var actualProducts = dict["data"].Deserialize<List<ProductReadDto>>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            actualProducts.Should().BeEquivalentTo(products);
        }





        [Fact]
        public async Task GetAll_ReturnsEmptyData()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            var client = HttpClientTestHelper.CreateFakeHttpClient(response);
            A.CallTo(() => _httpClientFactory.CreateClient("InventoryAPI")).Returns(client);

            // Act
            var result = await _controller.GetAll() as JsonResult;

            // Assert
            result.Should().NotBeNull();

            var json = JsonSerializer.Serialize(result.Value);
            var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

            dict.Should().ContainKey("data");

            var dataArray = dict["data"].Deserialize<List<ProductReadDto>>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            dataArray.Should().BeEmpty();
        }



        [Fact]
        public async Task CreateWithClientView_ReturnsViewResult()
        {
            //Arrange
            var suppliers = new List<SupplierReadDto> { new SupplierReadDto { Id = 1, Name = "Supplier1" } };
            var json = JsonSerializer.Serialize(suppliers);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            var client = HttpClientTestHelper.CreateFakeHttpClient(response);
            A.CallTo(() => _httpClientFactory.CreateClient("InventoryAPI")).Returns(client);

            //Act
            var result = await _controller.CreateWithClientView() as ViewResult;

            //Assert
            result.Should().NotBeNull();
            result.Model.Should().BeOfType<ProductWriteDto>();
            ((List<SelectListItem>)_controller.ViewBag.Suppliers).Should().NotBeEmpty();
        }




        [Fact]
        public async Task Save_ReturnsToIndex()
        {
            //Arrange
            var productWriteDto = new ProductWriteDto
            {
                Name = "Test Product",
                Category = ProductCategory.Electronics,
                Price = 9.99m,
                StockQty = 5,
                SupplierId = 1,
                ProductImageFile = null 
            };

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            var client = HttpClientTestHelper.CreateFakeHttpClient(response);

            A.CallTo(() => _httpClientFactory.CreateClient("InventoryAPI")).Returns(client);

            //to ensure TempData is not null since it is not null in calling method
            var tempDataProvider = A.Fake<ITempDataProvider>();
            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), tempDataProvider);

            //Act
            var result = await _controller.Save(productWriteDto, null) as RedirectToActionResult;

            //Assert
            result.Should().NotBeNull();
            result.ActionName.Should().Be("Index");
        }








        [Fact]
        public async Task Edit_ReturnsViewResult()
        {
            //Arrange
            var product = new ProductReadDto { Id = 1, Name = "Product1" };
            var productJson = JsonSerializer.Serialize(product);
            var suppliers = new List<SupplierReadDto> { new SupplierReadDto { Id = 1, Name = "Supplier1" } };
            var suppliersJson = JsonSerializer.Serialize(suppliers);

            var productResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(productJson, Encoding.UTF8, "application/json")
            };
            var supplierResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(suppliersJson, Encoding.UTF8, "application/json")
            };

            var client = HttpClientTestHelper.CreateFakeHttpClient(productResponse, supplierResponse);
            A.CallTo(() => _httpClientFactory.CreateClient("InventoryAPI")).Returns(client);

            //Act
            var result = await _controller.Edit(1) as ViewResult;

            //Assert
            result.Should().NotBeNull();
            result.Model.Should().BeOfType<ProductWriteDto>();
            ((List<SelectListItem>)_controller.ViewBag.Suppliers).Should().NotBeEmpty();
        }




        [Fact]
        public async Task Update_ReturnsToIndex()
        {
            //Arrange
            var productWriteDto = new ProductWriteDto
            {
                Name = "Test Product",
                Category = ProductCategory.Electronics,
                Price = 9.99m,
                StockQty = 5,
                SupplierId = 1,
                ProductImageFile = null
            };

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            var client = HttpClientTestHelper.CreateFakeHttpClient(response);
            A.CallTo(() => _httpClientFactory.CreateClient("InventoryAPI")).Returns(client);

            var tempDataProvider = A.Fake<ITempDataProvider>();
            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), tempDataProvider);

            //Act
            var result = await _controller.Update(1, productWriteDto, null) as RedirectToActionResult;

            //Assert
            result.Should().NotBeNull();
            result.ActionName.Should().Be("Index");
        }



        [Fact]
        public async Task Delete_ReturnsSuccess()
        {
            //Arrange
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            var client = HttpClientTestHelper.CreateFakeHttpClient(response);
            A.CallTo(() => _httpClientFactory.CreateClient("InventoryAPI")).Returns(client);

            //Act
            var result = await _controller.Delete(1) as JsonResult;

            //Assert
            result.Should().NotBeNull();

            var json = JsonSerializer.Serialize(result.Value);
            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

            dict.Should().ContainKey("success");
            dict["success"].ToString().Should().Be("True");

            dict.Should().ContainKey("message");
            dict["message"].ToString().Should().NotBeNullOrEmpty();
        }



        [Fact]
        public async Task DeleteAll_ReturnsSuccess()
        {
            //Arrange
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            var client = HttpClientTestHelper.CreateFakeHttpClient(response);
            A.CallTo(() => _httpClientFactory.CreateClient("InventoryAPI")).Returns(client);

            //Act
            var result = await _controller.DeleteAll() as JsonResult;

            //Assert
            result.Should().NotBeNull();
            var json = JsonSerializer.Serialize(result.Value);
            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

            dict.Should().ContainKey("success");
            dict["success"].ToString().Should().Be("True");

            dict.Should().ContainKey("message");
            dict["message"].ToString().Should().NotBeNullOrEmpty();
        }


        #endregion ClientSideApproach





        #region ServerSideApproach


        [Fact]
        public async Task IndexServer_ReturnsViewResult()
        {
            //Arrange
            var products = new List<ProductReadDto> { new ProductReadDto() };
            var json = JsonSerializer.Serialize(products);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            var client = HttpClientTestHelper.CreateFakeHttpClient(response);
            A.CallTo(() => _httpClientFactory.CreateClient("InventoryAPI")).Returns(client);

            //Act
            var result = await _controller.IndexServer() as ViewResult;

            //Assert
            result.Should().NotBeNull();
            result.Model.Should().BeEquivalentTo(products);
        }



        [Fact]
        public async Task CreateServer_ReturnsViewResult()
        {
            //Arrange
            var suppliers = new List<SupplierReadDto> { new SupplierReadDto { Id = 1, Name = "Supplier1" } };
            var json = JsonSerializer.Serialize(suppliers);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            var client = HttpClientTestHelper.CreateFakeHttpClient(response);
            A.CallTo(() => _httpClientFactory.CreateClient("InventoryAPI")).Returns(client);

            //Act
            var result = await _controller.CreateServer() as ViewResult;

            //Assert
            result.Should().NotBeNull();
            result.Model.Should().BeOfType<ProductWriteDto>();
            ((List<SelectListItem>)_controller.ViewBag.Suppliers).Should().NotBeEmpty();
        }



        [Fact]
        public async Task SaveServer_ReturnsToIndexServer()
        {
            // Create a fake file stream
            var content = "Fake image content";
            var fileName = "test-image.jpg";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            // Create the IFormFile
            var formFile = new FormFile(stream, 0, stream.Length, "ProductImageFile", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };


            //Arrange
            var productWriteDto = new ProductWriteDto
            {
                Name = "Test Product",
                Category = ProductCategory.Apparel, 
                Price = 10.99m,
                ProductImageFile = formFile,
                StockQty = 5,
                SupplierId = 1
            };


            var response = new HttpResponseMessage(HttpStatusCode.OK);
            var client = HttpClientTestHelper.CreateFakeHttpClient(response);
            A.CallTo(() => _httpClientFactory.CreateClient("InventoryAPI")).Returns(client);

            var tempDataProvider = A.Fake<ITempDataProvider>();
            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), tempDataProvider);

            //Act
            var result = await _controller.SaveServer(productWriteDto, null) as RedirectToActionResult;

            //Assert
            result.Should().NotBeNull();
            result.ActionName.Should().Be("IndexServer");
        }



        [Fact]
        public async Task EditServer_ReturnsViewResult()
        {
            //Arrange
            var product = new ProductReadDto { Id = 1, Name = "Product1" };
            var productJson = JsonSerializer.Serialize(product);
            var suppliers = new List<SupplierReadDto> { new SupplierReadDto { Id = 1, Name = "Supplier1" } };
            var suppliersJson = JsonSerializer.Serialize(suppliers);

            var productResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(productJson, Encoding.UTF8, "application/json")
            };
            var supplierResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(suppliersJson, Encoding.UTF8, "application/json")
            };

            var client = HttpClientTestHelper.CreateFakeHttpClient(productResponse, supplierResponse);
            A.CallTo(() => _httpClientFactory.CreateClient("InventoryAPI")).Returns(client);

            //Act
            var result = await _controller.EditServer(1) as ViewResult;

            //Assert
            result.Should().NotBeNull();
            result.Model.Should().BeOfType<ProductWriteDto>();
            ((List<SelectListItem>)_controller.ViewBag.Suppliers).Should().NotBeEmpty();
        }




        [Fact]
        public async Task UpdateServer_ReturnsToIndexServer()
        {
            //Arrange
            var productWriteDto = new ProductWriteDto
            {
                Name = "Test Product",
                Category = ProductCategory.Electronics,
                Price = 9.99m,
                StockQty = 5,
                SupplierId = 1,
                ProductImageFile = null
            };

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            var client = HttpClientTestHelper.CreateFakeHttpClient(response);
            A.CallTo(() => _httpClientFactory.CreateClient("InventoryAPI")).Returns(client);

            var tempDataProvider = A.Fake<ITempDataProvider>();
            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), tempDataProvider);

            //Act
            var result = await _controller.UpdateServer(1, productWriteDto, null) as RedirectToActionResult;

            //Assert
            result.Should().NotBeNull();
            result.ActionName.Should().Be("IndexServer");
        }



        [Fact]
        public async Task DeleteServer_ReturnsViewResult()
        {
            //Arrange
            var product = new ProductReadDto { Id = 1, Name = "Product1" };
            var json = JsonSerializer.Serialize(product);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            var client = HttpClientTestHelper.CreateFakeHttpClient(response);
            A.CallTo(() => _httpClientFactory.CreateClient("InventoryAPI")).Returns(client);

            //Act
            var result = await _controller.DeleteServer(1) as ViewResult;

            //Assert
            result.Should().NotBeNull();
            result.Model.Should().BeOfType<ProductReadDto>();
        }



        [Fact]
        public async Task DeleteServerConfirmed_ReturnsToIndexServer()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Deleted successfully")
            };
            var client = HttpClientTestHelper.CreateFakeHttpClient(response);
            A.CallTo(() => _httpClientFactory.CreateClient("InventoryAPI")).Returns(client);

            _controller = new ProductMvcController(_httpClientFactory)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), A.Fake<ITempDataProvider>())
            };

            // Act
            var result = await _controller.DeleteServerConfirmed(1) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result.ActionName.Should().Be("IndexServer");
        }





        #endregion ServerSideApproach
    }

    //public class JsonResultData
    //{
    //    public IEnumerable<ProductReadDto> data { get; set; }
    //}

    //public class JsonDeleteResult
    //{
    //    public bool success { get; set; }
    //    public string message { get; set; }
    //}
}
