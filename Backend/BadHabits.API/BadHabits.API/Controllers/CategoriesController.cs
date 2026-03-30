using BadHabits.API.Data;
using Microsoft.AspNetCore.Mvc;

namespace BadHabits.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        [HttpGet]
        public IActionResult GetAllCategories()
        {
            var categories = _context.Categories
                .Select(c => new
                {
                    id = c.Id,
                    name = c.Name
                })
                .ToList();

            return Ok(new { success = true, data = categories });
        }
    }
}