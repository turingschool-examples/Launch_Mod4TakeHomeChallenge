using Microsoft.AspNetCore.Mvc;
using CommerceAPI.DataAccess;
using CommerceAPI.Models;

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

        [HttpGet]
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
    }
}
