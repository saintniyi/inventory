using AutoMapper;
using InventoryData.IRepo;
using InventoryModel.Models;
using InventoryDto;
using Microsoft.AspNetCore.Mvc;


namespace InventoryAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;


        public SupplierController(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }


        // GET: api/Supplier
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var suppliersFromDb = await _unitOfWork.Supplier.GetAllAsync();
            var supplierReadDtos = _mapper.Map<IEnumerable<SupplierReadDto>>(suppliersFromDb);
            return Ok(supplierReadDtos);
        }


        // GET: api/Supplier/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var supplierFromDb = await _unitOfWork.Supplier.GetOneAsync(s => s.Id == id);

            if (supplierFromDb == null)
                return NotFound();

            var supplierReadDto = _mapper.Map<SupplierReadDto>(supplierFromDb);
            return Ok(supplierReadDto);
        }


        // POST: api/Supplier
        [HttpPost]
        public async Task<IActionResult> Create ([FromBody] SupplierWriteDto supplierWriteDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var supplier = _mapper.Map<Supplier>(supplierWriteDto);
            await _unitOfWork.Supplier.AddAsync(supplier);
            await _unitOfWork.SaveAsync();

            var supplierReadDto = _mapper.Map<SupplierReadDto>(supplier);

            return CreatedAtAction(nameof(GetById), new { id = supplier.Id }, supplierReadDto);
        }



        // PUT: api/Supplier/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SupplierWriteDto supplierWriteDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var supplierFromDb = await _unitOfWork.Supplier.GetOneAsync(s => s.Id == id);
            if (supplierFromDb == null)
                return NotFound();

            _mapper.Map(supplierWriteDto, supplierFromDb);
            _unitOfWork.Supplier.Update(supplierFromDb);
            await _unitOfWork.SaveAsync();


            return NoContent();
        }



        // DELETE: api/Supplier/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var supplierFromDb = await _unitOfWork.Supplier.GetOneAsync(s => s.Id == id);
            if (supplierFromDb == null)
                return NotFound();

            _unitOfWork.Supplier.Remove(supplierFromDb);
            await _unitOfWork.SaveAsync();

            //return NoContent();
            return Ok(new { success = true, message = "Supplier deleted successfully." });
        }



        // DELETE: api/Supplier
        [HttpDelete]
        public async Task<IActionResult> DeleteAll([FromBody] List<int> ids)
        {
            var suppliersFromDb = await _unitOfWork.Supplier.GetAllAsync(s => ids.Contains(s.Id));
            if (!suppliersFromDb.Any())
                return BadRequest();

            _unitOfWork.Supplier.RemoveRange(suppliersFromDb);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }



    }
}
