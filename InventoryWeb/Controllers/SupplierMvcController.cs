using InventoryDto;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;


namespace InventoryWeb.Controllers
{
    public class SupplierMvcController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SupplierMvcController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index() => View();



        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var client = _httpClientFactory.CreateClient("InventoryAPI");
            var response = await client.GetAsync("Supplier");

            if (!response.IsSuccessStatusCode)
                return Json(new { data = new List<SupplierReadDto>() });

            var json = await response.Content.ReadAsStringAsync();
            var suppliers = JsonSerializer.Deserialize<IEnumerable<SupplierReadDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return Json(new { data = suppliers });
        }



        // GET: Supplier/Upsert or Supplier/Upsert?id=5
        public async Task<IActionResult> Upsert(int? id)
        {
            if (id == null || id == 0)
            {
                // Create new supplier
                return View(new SupplierWriteDto());
            }

            var client = _httpClientFactory.CreateClient("InventoryAPI");
            var response = await client.GetAsync($"Supplier/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return View("Error");
            }

            var json = await response.Content.ReadAsStringAsync();
            var supplier = JsonSerializer.Deserialize<SupplierReadDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Map ReadDto to WriteDto for editing
            var editDto = new SupplierWriteDto
            {
                Name = supplier.Name,
                Email = supplier.Email,
                Phone = supplier.Phone
            };

            return View(editDto);
        }



        // POST: Supplier/Upsert 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(int? id, SupplierWriteDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var client = _httpClientFactory.CreateClient("InventoryAPI");
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");

            HttpResponseMessage response;

            if (id == null || id == 0)
            {
                // Create
                response = await client.PostAsync("Supplier", content);
                TempData["success"] = "Supplier created successfully";
            }
            else
            {
                // Update
                response = await client.PutAsync($"Supplier/{id}", content);
                TempData["success"] = "Supplier updated successfully";
            }

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            return View("Error");
        }


        
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("InventoryAPI");
            var response = await client.DeleteAsync($"Supplier/{id}");

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Supplier deleted successfully." });
            }

            return Json(new { success = false, message = "Failed to delete supplier." });
        }



    }







}
