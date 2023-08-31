using CommerceAPI.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CommerceAPI.Controllers
{
    public class ProductsController : Controller
    {
        private readonly CommerceApiContext _context;

        public ProductsController(CommerceApiContext context)
        {
            _context = context;
        }
        [HttpGet]
        public ActionResult GetProducts(int merchantId)
        {
            var merchantProducts = _context.Merchants.Include(m => m.Products).FirstOrDefault(m => m.Id == merchantId);

            if (merchantProducts == null)
            {
                return NotFound("Product not found");
            }

            return Ok(merchantProducts.Products);
        }
    }
}
