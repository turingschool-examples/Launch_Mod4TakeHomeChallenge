using CommerceAPI.DataAccess;
using CommerceAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CommerceAPI.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly CommerceApiContext _context;

        public ProductsController(CommerceApiContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProductAsync(Product product)
        {
            // Check if the associated merchant exists
            var merchant = await _context.Merchants.FindAsync(product.MerchantId);

            if (merchant == null)
            {
                return BadRequest("Merchant does not exist.");
            }

            // Associate the product with the merchant
            product.Merchant = merchant;

            // Add the product to the context and save changes
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            Response.StatusCode = 201;
            return new JsonResult(product);
         //   return CreatedAtAction("GetProduct", new { id = product.ProductId }, product);
        }

        
    }
}
