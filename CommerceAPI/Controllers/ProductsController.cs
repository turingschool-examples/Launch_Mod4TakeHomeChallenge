using CommerceAPI.DataAccess;
using Microsoft.AspNetCore.Mvc;
using CommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CommerceAPI.Controllers
{
    [Route("/api/merchants/{merchantId}/[controller]")]
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
            //var merchantProducts = _context.Products
            //    .Where(p => p.Merchant.Id == merchantId)
            //    .Include(p => p.Merchant);
            var merchantProducts = _context.Merchants
                .Where(m => m.Id == merchantId)
                .Include(m => m.Products)
                .ToList();


            return new JsonResult(merchantProducts);
        }

        [HttpGet("{productId}")]
        public ActionResult GetProduct(int productId)
        {
            var product = _context.Products.Find(productId);
            return new JsonResult(product);
        }

        [HttpPost]
        public ActionResult CreateProduct(int merchantId, Product product)
        {
            var merchant = _context.Merchants
                .Where(m => m.Id == merchantId)
                .Include(m => m.Products)
                .FirstOrDefault();

            //merchant.Products.Add(product);
            product.MerchantId = merchantId;
            _context.Products.Add(product);
            //_context.Merchants.Update(merchant);
            _context.SaveChanges();

            return new JsonResult(merchant);
        }

        [HttpPut("{productId}")]
        public ActionResult EditProduct(int productId, Product product)
        {
            var currentProduct = _context.Products.Find(productId);

            if (currentProduct != null)
            {
                currentProduct.Name = product.Name;
                currentProduct.Description = product.Description;
                currentProduct.Category = product.Category;
                currentProduct.PriceInCents = product.PriceInCents;
                currentProduct.StockQuantity = product.StockQuantity;
                currentProduct.ReleaseDate = product.ReleaseDate;

                _context.SaveChanges();

                return new JsonResult(currentProduct);
            }

            return NotFound();
        }

        [HttpDelete("{productId}")]
        public ActionResult DeleteProduct(int productId, int merchantId)
        {
            var product = _context.Products.Find(productId);
            var merchant = _context.Merchants
                .Where(m => m.Id == merchantId)
                .Include(m => m.Products)
                .FirstOrDefault();
            _context.Products.Remove(product);
            _context.SaveChanges();
            return new JsonResult(merchant);
        }
    }
}
