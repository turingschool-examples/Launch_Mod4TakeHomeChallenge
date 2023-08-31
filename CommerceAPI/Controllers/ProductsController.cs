using CommerceAPI.DataAccess;
using CommerceAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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




        [HttpGet("{productId}")]
        public ActionResult GetProductById(int productId, int MerchantId)
        {
            
            var merchant = _context.Merchants.FirstOrDefault(m => m.Id == MerchantId);

            if (merchant == null)
            {
                return NotFound();
            }

            var product = _context.Products.Where(p => p.MerchantId == MerchantId).FirstOrDefault(p => p.ProductId == productId);

            return new JsonResult(product);
        }




        [HttpPut("{productId}")]
        public ActionResult UpdateProduct(int productId, int MerchantId, Product updatedProduct)
        {

            var merchant = _context.Merchants.FirstOrDefault(m => m.Id == MerchantId);

            if (merchant == null)
            {
                return NotFound();
            }

            var product = _context.Products.Where(p => p.MerchantId == MerchantId).FirstOrDefault(p => p.ProductId == productId);
            if (product == null)
            {
                return NotFound();
            }

            product.Name = updatedProduct.Name;
            product.Description = updatedProduct.Description;
            product.Category = updatedProduct.Category;
            product.Price = updatedProduct.Price;
            product.StockQuantity = updatedProduct.StockQuantity;
            _context.Products.Update(product);
            _context.SaveChanges();
            Response.StatusCode = 204;

            return new JsonResult(product);
        }




    }
}
