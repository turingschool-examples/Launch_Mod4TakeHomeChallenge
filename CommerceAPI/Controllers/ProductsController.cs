using CommerceAPI.DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace CommerceAPI.Controllers
{
    public class ProductsController : Controller
    {
        private readonly CommerceApiContext _context;

        public ProductsController(CommerceApiContext context)
        {
            _context = context;
        }
       
    }
}
