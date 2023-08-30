using Microsoft.AspNetCore.Mvc;
using CommerceAPI.Models;
using CommerceAPI.DataAccess;
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
        
        public ActionResult GetProducts(int merchantId)
        {
            var merchantWithProducts = _context.Merchants.Include(merch => merch.Products).FirstOrDefault(merch => merch.Id == merchantId);
            
            if (merchantWithProducts == null) 
            {
                return NotFound("Merchant not found");
            }

            return Ok(merchantWithProducts.Products);
        }
    }
}
