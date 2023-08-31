using CommerceAPI.DataAccess;
using CommerceAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CommerceAPI.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class MerchantsController : ControllerBase
    {
        private readonly CommerceApiContext _context;

        public MerchantsController(CommerceApiContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Merchant>> GetMerchants()
        {
            return _context.Merchants.Include(e => e.Products).ToList();
        }

        [HttpPost]
        public ActionResult CreateMerchant(Merchant merchant)
        {
            _context.Merchants.Add(merchant);
            _context.SaveChanges();

            return CreatedAtAction("GetMerchants", merchant);
        }
    }
}
