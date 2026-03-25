using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReimbursementTrackerApp.Services.Interfaces;

namespace ReimbursementTrackerApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
   
    public class ExpenseCategoryController : ControllerBase
    {
        private readonly IExpenseCategoryService _service;

        public ExpenseCategoryController(IExpenseCategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string categoryName)
        {
            var id = await _service.CreateAsync(categoryName);
            return Ok(id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, string categoryName)
        {
            await _service.UpdateAsync(id, categoryName);
            return Ok("Updated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);

            if (!result)
                return NotFound();

            return Ok("Deleted successfully");
        }
    }
}
