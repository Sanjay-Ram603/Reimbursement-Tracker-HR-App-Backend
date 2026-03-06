using Microsoft.AspNetCore.Mvc;
using ReimbursementTrackerApp.Repositories.Interfaces;

namespace ReimbursementTrackerApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpenseCategoryController : ControllerBase
    {
        private readonly IExpenseCategoryRepository _repository;

        public ExpenseCategoryController(IExpenseCategoryRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _repository.GetAllAsync();
            return Ok(categories);
        }
    }
}
