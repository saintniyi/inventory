using AutoMapper;
using InventoryData.IRepo;
using InventoryModel.Models;
using InventoryDto;
using Microsoft.AspNetCore.Mvc;



namespace InventoryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public ProductController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }



        // GET: api/Product
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _unitOfWork.Product.GetAllAsync(includeOthers: "Supplier");
            var dto = _mapper.Map<IEnumerable<ProductReadDto>>(products);
            return Ok(dto);
        }



        // GET: api/Product/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _unitOfWork.Product.GetOneAsync(p => p.Id == id, "Supplier");
            if (product == null)
                return NotFound(new { Message = $"Product with ID {id} not found." });

            var dto = _mapper.Map<ProductReadDto>(product);
            return Ok(dto);
        }



        // POST: api/Product
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductWriteDto prodWriteDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = _mapper.Map<Product>(prodWriteDto);

            if (prodWriteDto.ProductImageFile != null)
            {
                using var ms = new MemoryStream();
                await prodWriteDto.ProductImageFile.CopyToAsync(ms);
                product.ProductImage = ms.ToArray();
            }

            await _unitOfWork.Product.AddAsync(product);
            await _unitOfWork.SaveAsync();

            var resultDto = _mapper.Map<ProductReadDto>(product);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, resultDto);
        }



        // PUT: api/Product/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] ProductWriteDto prodWriteDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var productFromDb = await _unitOfWork.Product.GetOneAsync(p => p.Id == id);
            if (productFromDb == null)
                return NotFound(new { Message = $"Product with ID {id} not found." });

            _mapper.Map(prodWriteDto, productFromDb);

            if (prodWriteDto.ProductImageFile != null)
            {
                using var ms = new MemoryStream();
                await prodWriteDto.ProductImageFile.CopyToAsync(ms);
                productFromDb.ProductImage = ms.ToArray();
            }

            _unitOfWork.Product.Update(productFromDb);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }



        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _unitOfWork.Product.GetOneAsync(p => p.Id == id);
            if (product == null)
                return NotFound(new { Message = $"Product with ID {id} not found." });

            _unitOfWork.Product.Remove(product);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }



        // DELETE: api/Product
        [HttpDelete]
        public async Task<IActionResult> DeleteAll([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest(new { Message = "No product IDs were provided." });

            var products = await _unitOfWork.Product.GetAllAsync(p => ids.Contains(p.Id));

            if (!products.Any())
                return NotFound(new { Message = "No matching products found for deletion." });

            _unitOfWork.Product.RemoveRange(products);
            await _unitOfWork.SaveAsync();

            return Ok(new { Message = $"{products.Count()} products deleted." });
        }








    }
}
