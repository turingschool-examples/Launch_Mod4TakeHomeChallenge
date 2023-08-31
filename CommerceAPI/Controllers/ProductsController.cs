using CommerceAPI.DataAccess;
using CommerceAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CommerceAPI.Controllers
{
    [Route("/api/merchants/{merchantId:int}/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly CommerceApiContext _context;

        public ProductsController(CommerceApiContext context)
        {
            _context = context;
        }

        [HttpPost]
        
        public ActionResult CreateProduct(Product product, int MerchantId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var merchant = _context.Merchants.FirstOrDefault(m => m.Id == MerchantId);

            if (merchant == null)
            {
                return NotFound();
            }

            product.MerchantId = MerchantId;

            product.ReleaseDate = DateTime.UtcNow;

            _context.Products.Add(product);
            _context.SaveChanges();

            Response.StatusCode = 201;

            return new JsonResult(product);
        }

        
    }
}
