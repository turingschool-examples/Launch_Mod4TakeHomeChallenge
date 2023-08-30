using CommerceAPI.DataAccess;
using Microsoft.AspNetCore.Mvc;
using CommerceAPI.Models;

namespace CommerceAPI.Controllers
{
    [Route("/api")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly CommerceApiContext _context;

        public ProductsController(CommerceApiContext context)
        {
            _context = context;
        }

        [HttpGet("products/{id}")]
        public ActionResult<Product> GetProduct_ReturnsSingleProduct(int id)
        {
            return _context.Products.Find(id);
        }

        [HttpGet("products")]
        public ActionResult<IEnumerable<Product>> GetProducts()
        {
            return _context.Products;
        }
    }
}