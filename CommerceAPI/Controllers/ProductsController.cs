using Microsoft.AspNetCore.Mvc;
using CommerceAPI.DataAccess;
using CommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CommerceAPI.Controllers
{
    [Route("/api/merchants/{merchantId:int}/[controller]")] //base URL
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly CommerceApiContext _context;
        public ProductsController(CommerceApiContext context)
        {
            _context = context;
        }

        [HttpGet] //Always need to indicate what HTTP method this action responds to
        public ActionResult GetProducts(int merchantId)
        {
            var merchant = _context.Merchants.FirstOrDefault(merchant => merchant.Id == merchant.Id);
            if (merchant == null)
            {
                return NotFound();
            }
            var products = _context.Products.Where(product => product.MerchantId == merchantId);
            return new JsonResult(products.OrderBy(product => product.Id));
        }

        [HttpGet("{productId}")]
        public ActionResult GetProductById(int merchantId, int productId)
        {
            var merchant = _context.Merchants.FirstOrDefault(merchant => merchant.Id == merchant.Id);
            if (merchant == null)
            {
                return NotFound();
            }
            var products = _context.Products.Where(product => product.MerchantId == merchantId);
            var product = products.FirstOrDefault(product => product.Id == productId);
            if (product == null)
            {
                return NotFound();
            }
            return new JsonResult(product);
        }

        [HttpPost]
        public ActionResult CreateProduct(int merchantId, Product product)
        {
            if (!ModelState.IsValid)
            {
                //Response.StatusCode = 400;
                return BadRequest();
            }

            var merchant = _context.Merchants.FirstOrDefault(merchant => merchant.Id == merchant.Id);
            merchant.Products.Add(product);
            _context.Products.Add(product);
            _context.SaveChanges(); 

            return new JsonResult(product);
        }

        [HttpPut("{productId}")]
        public ActionResult UpdateProduct(int merchantId, int productId, Product product)
        {
            var merchant = _context.Merchants.Include(merchant => merchant.Products).FirstOrDefault(merchant => merchant.Id == merchantId);

            if (!ModelState.IsValid)
            {
                Response.StatusCode = 400;
            }

            var existingProduct = merchant.Products.FirstOrDefault(product => product.Id == productId);
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Category = product.Category;
            existingProduct.PriceInCents = product.PriceInCents;
            existingProduct.StockQuantity = product.StockQuantity;
            existingProduct.ReleaseDate = product.ReleaseDate;

            _context.Products.Update(existingProduct);
            _context.SaveChanges();

            Response.StatusCode = 204;
            return new JsonResult(existingProduct); ;
        }
                
        [HttpDelete("{productId}")]
        public ActionResult DeleteProductfromId(int merchantId, int productId)
        {
            var merchant = _context.Merchants.FirstOrDefault(merchant => merchant.Id == merchant.Id);

            if (merchant is null)
            {
                return NotFound();
            }

            var products = _context.Products.Where(product => product.MerchantId == merchantId);
            var product = products.FirstOrDefault(product => product.Id == productId);
            _context.Products.Remove(product);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
