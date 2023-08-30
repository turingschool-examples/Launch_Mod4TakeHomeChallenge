using CommerceAPI.DataAccess;
using CommerceAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommerceAPI.Controllers
{
    [Route("/api/merchants/{merchantId:int}/[controller]")]
    [ApiController]
    public class ProductsController
    {
        private readonly CommerceApiContext _context;

        public ProductsController(CommerceApiContext context)
        {
            _context = context;
        }

        //Retrieve a product by its primary key
        [HttpGet("{productId}")]
        public ActionResult<Product> GetProduct(int productId)
        {
            return _context.Products.Find(productId);
        }
        //Create a new product associated with a specific merchant
        
        //Update an existing product

        //Delete a product by its primary key
    }
}
