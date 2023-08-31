using CommerceAPI.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CommerceAPI.Models;

namespace CommerceAPI.Controllers
{
    [Route("api/merchants/{merchantId/[controller]")]
    [ApiController]
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
            var merchant = _context.Merchants.Include(m => m.Products).FirstOrDefault(m => m.Id == merchantId);

            if (merchant == null)
            {
                return NotFound("Product not found");
            }

            return Ok(merchant.Products);
        }
        [HttpGet("{productId}")]
        public ActionResult GetProductsFromId(int merchantId, int productId)
        {
            var merchant = _context.Merchants.FirstOrDefault(m => m.Id == merchantId);
            if (merchant == null)
            {
                return NotFound(" not found");
            }

            var productsOfMerchant = merchant.Products.FirstOrDefault(p => p.Id == productId);
            if (productsOfMerchant == null)
            {
                return NotFound("We do not have this item");
            }

            return new JsonResult(productsOfMerchant);
        }
        [HttpPut("{productId}")]
        public ActionResult UpdateProduct(int merchantId, int productId, Product product)
        {
            var merchantsProducts = _context.Merchants.Include(m => m.Products).FirstOrDefault(m => m.Id == merchantId);
            if (merchantsProducts == null)
            {
                return NotFound("Product not found");
            }

            var productOfMer = merchantsProducts.Products.FirstOrDefault(p => p.Id == productId);
            if (productOfMer == null)
            {
                return NotFound("We do not have this item");
            }

            productOfMer.Name = product.Name;
            productOfMer.Description = product.Description;
            productOfMer.Category = product.Category;
            productOfMer.MerchantId = merchantId;
            productOfMer.Price = product.Price;
            productOfMer.StockQuantity = product.StockQuantity;
            productOfMer.ReleaseDate = product.ReleaseDate;


            _context.Products.Update(productOfMer);
            _context.SaveChanges();

            return Ok(productOfMer);
        }
        [HttpDelete("{productId}")]
        public ActionResult DeleteProduct(int merchantId, int productId)
        {
            var merchantsProducts = _context.Merchants.Include(m => m.Products).FirstOrDefault(m => m.Id == merchantId);
            if (merchantsProducts == null)
            {
                return NotFound("Product not found");
            }

            var productOfMer = merchantsProducts.Products.FirstOrDefault(p => p.Id == productId);
            if (productOfMer == null)
            {
                return NotFound("We do not have this item");
            }

            _context.Products.Remove(productOfMer);
            _context.SaveChanges();

            return Ok(productOfMer);
        }
    }
}
