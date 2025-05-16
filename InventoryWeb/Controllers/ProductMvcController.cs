using InventoryDto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;
using System.Text.Json;


namespace InventoryWeb.Controllers
{
    public class ProductMvcController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;



        public ProductMvcController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }



        private async Task LoadSuppliers()
        {
            var client = _httpClientFactory.CreateClient("InventoryAPI");
            var response = await client.GetAsync("Supplier");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Suppliers = new List<SelectListItem>();
                return;
            }

            var json = await response.Content.ReadAsStringAsync();
            var suppliers = JsonSerializer.Deserialize<IEnumerable<SupplierReadDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            ViewBag.Suppliers = suppliers.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name
            }).ToList();
        }




        #region ClientSideApproach


        public IActionResult Index() => View();



        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var client = _httpClientFactory.CreateClient("InventoryAPI");
            var response = await client.GetAsync("Product");

            if (!response.IsSuccessStatusCode)
                return Json(new { data = new List<ProductReadDto>() });

            var json = await response.Content.ReadAsStringAsync();
            var products = JsonSerializer.Deserialize<IEnumerable<ProductReadDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return Json(new { data = products });
        }



        [HttpGet]
        public async Task<IActionResult> CreateWithClientView()
        {
            await LoadSuppliers();
            return View(new ProductWriteDto());
        }



        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Save(ProductWriteDto prodWriteDto, IFormFile? productImage)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        await LoadSuppliers();
        //        return View("Create", prodWriteDto);
        //    }

        //    // Assign image file
        //    if (productImage != null && productImage.Length > 0)
        //    {
        //        prodWriteDto.ProductImageFile = productImage;
        //    }

        //    var client = _httpClientFactory.CreateClient("InventoryAPI");

        //    using var content = new MultipartFormDataContent();
        //    content.Add(new StringContent(prodWriteDto.Name ?? string.Empty), "Name");
        //    content.Add(new StringContent(((int?)prodWriteDto.Category)?.ToString() ?? string.Empty), "Category");
        //    content.Add(new StringContent(prodWriteDto.Price.ToString() ?? "0"), "Price");
        //    content.Add(new StringContent(prodWriteDto.StockQty.ToString() ?? "0"), "StockQty");
        //    content.Add(new StringContent(prodWriteDto.SupplierId.ToString() ?? "0"), "SupplierId");

        //    if (prodWriteDto.ProductImageFile != null && prodWriteDto.ProductImageFile.Length > 0)
        //    {
        //        var streamContent = new StreamContent(prodWriteDto.ProductImageFile.OpenReadStream());
        //        streamContent.Headers.ContentType = new MediaTypeHeaderValue(prodWriteDto.ProductImageFile.ContentType);
        //        content.Add(streamContent, "ProductImageFile", prodWriteDto.ProductImageFile.FileName);
        //    }

        //    var response = await client.PostAsync("Product", content);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        TempData["success"] = "Product created successfully";
        //        return RedirectToAction(nameof(Index));
        //    }

        //    return View("Error");
        //}




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(ProductWriteDto prodWriteDto, IFormFile? productImage)
        {
            if (!ModelState.IsValid)
            {
                await LoadSuppliers();
                return View("Create", prodWriteDto);
            }

            // Assign uploaded file if provided
            prodWriteDto.ProductImageFile = productImage;

            var client = _httpClientFactory.CreateClient("InventoryAPI");

            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(prodWriteDto.Name ?? string.Empty), "Name");
            content.Add(new StringContent(((int?)prodWriteDto.Category)?.ToString() ?? string.Empty), "Category");
            content.Add(new StringContent(prodWriteDto.Price.ToString() ?? "0"), "Price");
            content.Add(new StringContent(prodWriteDto.StockQty.ToString() ?? "0"), "StockQty");
            content.Add(new StringContent(prodWriteDto.SupplierId.ToString() ?? "0"), "SupplierId");

            if (prodWriteDto.ProductImageFile != null)
            {
                try
                {
                    var stream = prodWriteDto.ProductImageFile.OpenReadStream();

                    if (stream != null && prodWriteDto.ProductImageFile.Length > 0)
                    {
                        var streamContent = new StreamContent(stream);
                        streamContent.Headers.ContentType = new MediaTypeHeaderValue(prodWriteDto.ProductImageFile.ContentType);
                        content.Add(streamContent, "ProductImageFile", prodWriteDto.ProductImageFile.FileName);
                    }
                }
                catch (Exception ex)
                {
                    // Optional: log and skip image if file is invalid (esp. in tests)
                    Console.WriteLine("Warning: Invalid ProductImageFile - " + ex.Message);
                }
            }

            var response = await client.PostAsync("Product", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["success"] = "Product created successfully";
                return RedirectToAction(nameof(Index));
            }

            return View("Error");
        }






        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var client = _httpClientFactory.CreateClient("InventoryAPI");

            // Fetch product details
            var response = await client.GetAsync($"Product/{id}");
            if (!response.IsSuccessStatusCode)
                return View("Error");

            var json = await response.Content.ReadAsStringAsync();
            var product = JsonSerializer.Deserialize<ProductReadDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Map ProductReadDto to ProductWriteDto
            var dto = new ProductWriteDto
            {
                Name = product.Name,
                Category = product.Category,
                Price = product.Price,
                StockQty = product.StockQty,
                SupplierId = product.Supplier?.Id ?? 0,
                // ProductWriteDto does not have ProductImage, so we don't map it here
                // But we do map the ProductImageBase64 to display an image preview
            };

            // Fetch suppliers for the dropdown
            var supplierResponse = await client.GetAsync("Supplier");
            if (supplierResponse.IsSuccessStatusCode)
            {
                var supplierJson = await supplierResponse.Content.ReadAsStringAsync();
                var suppliers = JsonSerializer.Deserialize<IEnumerable<SupplierReadDto>>(supplierJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                ViewBag.Suppliers = suppliers.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                }).ToList();
            }

            // Pass base64 image for preview (for display in the view)
            ViewBag.ImageBase64 = product.ProductImageBase64;  // For displaying the existing image in the view
            ViewBag.ProductId = id;

            return View(dto);
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id, ProductWriteDto dto, IFormFile? productImage)
        {
            if (id == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                await LoadSuppliers();
                ViewBag.ProductId = id;
                return View(dto);
            }

            if (productImage != null && productImage.Length > 0)
                dto.ProductImageFile = productImage;

            var client = _httpClientFactory.CreateClient("InventoryAPI");

            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(dto.Name), "Name");
            content.Add(new StringContent(((int)dto.Category).ToString()), "Category");
            content.Add(new StringContent(dto.Price.ToString()), "Price");
            content.Add(new StringContent(dto.StockQty.ToString()), "StockQty");
            content.Add(new StringContent(dto.SupplierId.ToString()), "SupplierId");

            if (dto.ProductImageFile != null)
            {
                var streamContent = new StreamContent(dto.ProductImageFile.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(dto.ProductImageFile.ContentType);
                content.Add(streamContent, "ProductImageFile", dto.ProductImageFile.FileName);
            }

            var response = await client.PutAsync($"Product/{id}", content);
            if (response.IsSuccessStatusCode)
            {
                TempData["success"] = "Product updated successfully";
                return RedirectToAction(nameof(Index));
            }

            return View("Error");
        }


        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("InventoryAPI");
            var response = await client.DeleteAsync($"Product/{id}");

            if (response.IsSuccessStatusCode)
                return Json(new { success = true, message = "Product deleted successfully." });

            return Json(new { success = false, message = "Failed to delete product." });
        }




        [HttpDelete]
        public async Task<IActionResult> DeleteAll()
        {
            var client = _httpClientFactory.CreateClient("InventoryAPI");
            var response = await client.DeleteAsync("Product/DeleteAll");

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "All products have been deleted successfully." });
            }

            return Json(new { success = false, message = "Failed to delete all products." });
        }




        #endregion ClientSideApproach



        //************************************************************************//
        //************************************************************************//




        #region ServerSideApproach



        public async Task<IActionResult> IndexServer()
        {
            var client = _httpClientFactory.CreateClient("InventoryAPI");
            var response = await client.GetAsync("Product");

            if (!response.IsSuccessStatusCode)
                return View("Error");

            var json = await response.Content.ReadAsStringAsync();
            var products = JsonSerializer.Deserialize<IEnumerable<ProductReadDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(products);
        }



        [HttpGet]
        public async Task<IActionResult> CreateServer()
        {
            await LoadSuppliers();
            return View(new ProductWriteDto());
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveServer(ProductWriteDto prodWriteDto, IFormFile? productImage)
        {
            if (!ModelState.IsValid)
            {
                await LoadSuppliers();
                return View("Create", prodWriteDto);
            }

            // Assign image file
            if (productImage != null && productImage.Length > 0)
            {
                prodWriteDto.ProductImageFile = productImage;
            }

            var client = _httpClientFactory.CreateClient("InventoryAPI");

            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(prodWriteDto.Name), "Name");
            content.Add(new StringContent(((int)prodWriteDto.Category).ToString()), "Category");
            content.Add(new StringContent(prodWriteDto.Price.ToString()), "Price");
            content.Add(new StringContent(prodWriteDto.StockQty.ToString()), "StockQty");
            content.Add(new StringContent(prodWriteDto.SupplierId.ToString()), "SupplierId");

            if (prodWriteDto.ProductImageFile != null)
            {
                var streamContent = new StreamContent(prodWriteDto.ProductImageFile.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(prodWriteDto.ProductImageFile.ContentType);
                content.Add(streamContent, "ProductImageFile", prodWriteDto.ProductImageFile.FileName);
            }

            var response = await client.PostAsync("Product", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["success"] = "Product created successfully";
                return RedirectToAction(nameof(IndexServer));
            }

            return View("Error");
        }




        public async Task<IActionResult> EditServer(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var client = _httpClientFactory.CreateClient("InventoryAPI");

            // Fetch product details
            var response = await client.GetAsync($"Product/{id}");
            if (!response.IsSuccessStatusCode)
                return View("Error");

            var json = await response.Content.ReadAsStringAsync();
            var product = JsonSerializer.Deserialize<ProductReadDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Map ProductReadDto to ProductWriteDto
            var dto = new ProductWriteDto
            {
                Name = product.Name,
                Category = product.Category,
                Price = product.Price,
                StockQty = product.StockQty,
                SupplierId = product.Supplier?.Id ?? 0,
                // ProductWriteDto does not have ProductImage, so we don't map it here
                // But we do map the ProductImageBase64 to display an image preview
            };

            // Fetch suppliers for the dropdown
            var supplierResponse = await client.GetAsync("Supplier");
            if (supplierResponse.IsSuccessStatusCode)
            {
                var supplierJson = await supplierResponse.Content.ReadAsStringAsync();
                var suppliers = JsonSerializer.Deserialize<IEnumerable<SupplierReadDto>>(supplierJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                ViewBag.Suppliers = suppliers.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                }).ToList();
            }

            // Pass base64 image for preview (for display in the view)
            ViewBag.ImageBase64 = product.ProductImageBase64;  // For displaying the existing image in the view
            ViewBag.ProductId = id;

            return View(dto);
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateServer(int? id, ProductWriteDto dto, IFormFile? productImage)
        {
            if (id == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                await LoadSuppliers();
                ViewBag.ProductId = id;
                return View(dto);
            }

            if (productImage != null && productImage.Length > 0)
                dto.ProductImageFile = productImage;

            var client = _httpClientFactory.CreateClient("InventoryAPI");

            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(dto.Name), "Name");
            content.Add(new StringContent(((int)dto.Category).ToString()), "Category");
            content.Add(new StringContent(dto.Price.ToString()), "Price");
            content.Add(new StringContent(dto.StockQty.ToString()), "StockQty");
            content.Add(new StringContent(dto.SupplierId.ToString()), "SupplierId");

            if (dto.ProductImageFile != null)
            {
                var streamContent = new StreamContent(dto.ProductImageFile.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(dto.ProductImageFile.ContentType);
                content.Add(streamContent, "ProductImageFile", dto.ProductImageFile.FileName);
            }

            var response = await client.PutAsync($"Product/{id}", content);
            if (response.IsSuccessStatusCode)
            {
                TempData["success"] = "Product updated successfully";
                return RedirectToAction(nameof(IndexServer));
            }

            return View("Error");
        }



        // GET: Display the product for deletion
        public async Task<IActionResult> DeleteServer(int id)
        {
            var client = _httpClientFactory.CreateClient("InventoryAPI");
            var response = await client.GetAsync($"Product/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var product = JsonSerializer.Deserialize<ProductReadDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteServerConfirmed(int id)
        {
            var client = _httpClientFactory.CreateClient("InventoryAPI");
            var response = await client.DeleteAsync($"Product/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["success"] = "Product deleted successfully!";
                return RedirectToAction("IndexServer", "ProductMvc");  
            }

            TempData["error"] = "Failed to delete the product.";
            return RedirectToAction("IndexServer", "ProductMvc");  
        }






        [HttpGet]
        public async Task<IActionResult> DeleteFromServerCallClient(int id)
        {
            var client = _httpClientFactory.CreateClient("InventoryAPI");

            var response = await client.GetAsync($"Product/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var product = JsonSerializer.Deserialize<ProductReadDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(product); 
        }





        #endregion ServerSideApproach





    }
}
