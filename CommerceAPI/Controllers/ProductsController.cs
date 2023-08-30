using Microsoft.AspNetCore.Mvc;
using CommerceAPI.Models;
using CommerceAPI.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace CommerceAPI.Controllers
{
    [Route("/api/merchants/{merchantId}/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly CommerceApiContext _context;
        public ProductsController(CommerceApiContext context)
        {
            _context = context;
        }
        
        public ActionResult GetProducts(int merchantId)
        {
            var merchantWithProducts = _context.Merchants.Include(merch => merch.Products).FirstOrDefault(merch => merch.Id == merchantId);
            
            if (merchantWithProducts == null) 
            {
                return NotFound("Merchant not found");
            }

            return Ok(merchantWithProducts.Products);
        }

        [HttpGet("{productId}")]
        public ActionResult GetProductViaId(int merchantId, int productId)
        {
            var merchantWithProducts = _context.Merchants.Include(merch => merch.Products).FirstOrDefault(merch => merch.Id == merchantId);

            if (merchantWithProducts == null)
            {
                return NotFound("Merchant not found");
            }

            var productFromMerchant = merchantWithProducts.Products.FirstOrDefault(product => product.Id == productId);

            if (merchantWithProducts == null)
            {
                return NotFound("Product not found in the specified Merchant");
            }

            return Ok(productFromMerchant);
        }

        [HttpPost]
        public ActionResult CreateProduct(int merchantId, Product product) 
        {
            var merchantWithProducts = _context.Merchants.Include(merch => merch.Products).FirstOrDefault(merch => merch.Id == merchantId);

            if (merchantWithProducts == null)
            {
                return NotFound("Merchant not found");
            }

            merchantWithProducts.Products.Add(product);
            _context.SaveChanges();

            return Ok(product);
        }

        [HttpPut("{productId}")]
        public ActionResult UpdateProductViaId(int merchantId, int productId, Product product)
        {
            var merchantWithProducts = _context.Merchants.Include(merch => merch.Products).FirstOrDefault(merch => merch.Id == merchantId);

            if (merchantWithProducts == null)
            {
                return NotFound("Merchant not found");
            }

            var productFromMerchant = merchantWithProducts.Products.FirstOrDefault(product => product.Id == productId);

            if (merchantWithProducts == null)
            {
                return NotFound("Product not found in the specified Merchant");
            }

            productFromMerchant.Name = product.Name;
            productFromMerchant.Description = product.Description;
            productFromMerchant.Category = product.Category;
            productFromMerchant.Price = product.Price;
            productFromMerchant.StockQuanity = product.StockQuanity;
            productFromMerchant.ReleaseDate = product.ReleaseDate;
            productFromMerchant.MerchantId = merchantId;

            _context.Products.Update(productFromMerchant);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{productId}")]
        public ActionResult DeleteProductViaId(int merchantId, int productId)
        {
            var merchantWithProducts = _context.Merchants.Include(merch => merch.Products).FirstOrDefault(merch => merch.Id == merchantId);

            if (merchantWithProducts == null)
            {
                return NotFound("Merchant not found");
            }

            var productFromMerchant = merchantWithProducts.Products.FirstOrDefault(product => product.Id == productId);

            if (merchantWithProducts == null)
            {
                return NotFound("Product not found in the specified Merchant");
            }

            _context.Products.Remove(productFromMerchant);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
