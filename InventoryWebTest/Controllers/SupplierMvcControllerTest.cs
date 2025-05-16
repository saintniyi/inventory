using FakeItEasy;
using FluentAssertions;
using InventoryDto;
using InventoryWeb.Controllers;
using InventoryWebTest.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Net;
using System.Text;
using System.Text.Json;



namespace InventoryWebTest.Controllers
{
    public class SupplierMvcControllerTest
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SupplierMvcController _controller;

        public SupplierMvcControllerTest()
        {
            _httpClientFactory = A.Fake<IHttpClientFactory>();

            var tempDataProvider = A.Fake<ITempDataProvider>();
            _controller = new SupplierMvcController(_httpClientFactory)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), tempDataProvider)
            };
        }



        [Fact]
        public void Index_ReturnsViewResult()
        {
            // Act
            var result = _controller.Index();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }




        [Fact]
        public async Task GetAll_ReturnsSuppliersList()
        {
            // Arrange
            var suppliers = new List<SupplierReadDto> { new SupplierReadDto { Name = "Test Supplier" } };
            var json = JsonSerializer.Serialize(suppliers);
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

            var actualSuppliers = dict["data"].Deserialize<List<SupplierReadDto>>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            actualSuppliers.Should().BeEquivalentTo(suppliers);
            actualSuppliers.Should().HaveCount(1);
        }



        [Fact]
        public async Task GetAll_ReturnsEmptyList()
        {
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            var client = HttpClientTestHelper.CreateFakeHttpClient(response);
            A.CallTo(() => _httpClientFactory.CreateClient("InventoryAPI")).Returns(client);

            var result = await _controller.GetAll() as JsonResult;

            result.Should().NotBeNull();

            var json = JsonSerializer.Serialize(result.Value);
            var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

            dict.Should().ContainKey("data");

            var dataArray = dict["data"].Deserialize<List<SupplierReadDto>>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            dataArray.Should().BeEmpty();
        }




        [Fact]
        public async Task Upsert_Get_ReturnsView_WhenIdIsValid()
        {
            var supplier = new SupplierReadDto
            {
                Name = "Test Supplier",
                Email = "test@example.com",
                Phone = "1234567890"
            };
            var json = JsonSerializer.Serialize(supplier);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var client = HttpClientTestHelper.CreateFakeHttpClient(response);
            A.CallTo(() => _httpClientFactory.CreateClient("InventoryAPI")).Returns(client);

            var result = await _controller.Upsert(1) as ViewResult;

            result.Should().NotBeNull();
            result.Model.Should().BeOfType<SupplierWriteDto>();
        }



        [Fact]
        public async Task Upsert_Get_ReturnsEmpty_WhenIdIsNull()
        {
            var result = await _controller.Upsert(null) as ViewResult;

            result.Should().NotBeNull();
            result.Model.Should().BeOfType<SupplierWriteDto>();
        }



        [Fact]
        public async Task Upsert_Post_CreatesSupplier_WhenModelIsValid()
        {
            var dto = new SupplierWriteDto { Name = "New Supplier", Email = "new@supplier.com", Phone = "000123" };
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            var client = HttpClientTestHelper.CreateFakeHttpClient(response);

            A.CallTo(() => _httpClientFactory.CreateClient("InventoryAPI")).Returns(client);

            var result = await _controller.Upsert(null, dto) as RedirectToActionResult;

            result.Should().NotBeNull();
            result.ActionName.Should().Be("Index");
        }



        [Fact]
        public async Task Upsert_Post_ReturnsView_WhenModelIsInvalid()
        {
            _controller.ModelState.AddModelError("Name", "Required");

            var dto = new SupplierWriteDto();

            var result = await _controller.Upsert(null, dto) as ViewResult;

            result.Should().NotBeNull();
            result.Model.Should().Be(dto);
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
                       
            var serialized = JsonSerializer.Serialize(result.Value);
            var parsed = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(serialized);

            parsed.Should().ContainKey("success");
            parsed["success"].GetBoolean().Should().BeTrue();

            parsed.Should().ContainKey("message");
            parsed["message"].GetString().Should().Be("Supplier deleted successfully.");
        }



        [Fact]
        public async Task Delete_ReturnsFailure()
        {
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            var client = HttpClientTestHelper.CreateFakeHttpClient(response);

            A.CallTo(() => _httpClientFactory.CreateClient("InventoryAPI")).Returns(client);

            var result = await _controller.Delete(1) as JsonResult;

            //Assert
            result.Should().NotBeNull();

            var serialized = JsonSerializer.Serialize(result.Value);
            var parsed = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(serialized);

            parsed.Should().ContainKey("success");
            parsed["success"].GetBoolean().Should().BeFalse();

            parsed.Should().ContainKey("message");
            parsed["message"].GetString().Should().Be("Failed to delete supplier.");
        }



        


    }


}
