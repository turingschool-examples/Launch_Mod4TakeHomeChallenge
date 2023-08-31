using CommerceAPI.DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace CommerceAPI.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly CommerceApiContext _context;

        public StatisticsController(CommerceApiContext context)
        {
            _context = context;
        }

        [Route("/api/bycategory")]
        [HttpGet]
        public ActionResult CountByCategory()
        {
            var countByCategory = new Dictionary<string, int>();
            foreach(var product in _context.Products)
            {
                if (countByCategory.ContainsKey(product.Category))
                {
                    countByCategory[product.Category]++;
                }
                else
                {
                    countByCategory.Add(product.Category, 1);
                }
            }
            return new JsonResult(countByCategory);
        }

        [Route("/api/avgprice")]
        [HttpGet]
        public ActionResult AvgPricePerMerchant()
        {
            var avgPricePerMerchant = new Dictionary<string, double>();
            foreach(var merchant in _context.Merchants)
            {
                int merchantPriceSum = _context.Products.Where(p => p.MerchantId == merchant.Id).Select(p => p.PriceInCents).Sum();
                int merchantCount = _context.Products.Where(p => p.MerchantId == merchant.Id).Count();
                avgPricePerMerchant.Add(merchant.Name, (merchantPriceSum / merchantCount));
            }
            return new JsonResult(avgPricePerMerchant);
        }

        [Route("/api/newestproduct")]
        [HttpGet]
        public ActionResult NewestProductByMerchant()
        {
            var newestReleaseDate = new Dictionary<string, DateTime>();
            foreach (var merchant in _context.Merchants)
            {
                var productDates = _context.Products.Where(p => p.MerchantId == merchant.Id).Select(p => p.ReleaseDate);
                var latestDate = productDates.OrderByDescending(d => d).First();
                newestReleaseDate.Add(merchant.Name, latestDate);
            }
            return new JsonResult(newestReleaseDate);
        }
    }
}
