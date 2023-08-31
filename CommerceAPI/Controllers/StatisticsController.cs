using CommerceAPI.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpGet]
        public ActionResult GetCountOfProductsByCategory()
        {
            string countResults = string.Empty;

            foreach(var keyValue in CountOfProductsByCategory())
            {
                countResults += keyValue.Key;
                countResults += ": ";
                countResults += keyValue.Value;
                countResults += ", ";
            }

            return Content(countResults);
        }

        public Dictionary<string, int> CountOfProductsByCategory()
        {
            Dictionary<string, int> categoryCount = new Dictionary<string, int>();

            foreach (var merchant in _context.Merchants.Include(m => m.Products))
            {

                var groupedProducts = merchant.Products.GroupBy(p => p.Category);

                foreach (var group in groupedProducts)
                {
                    if(!categoryCount.ContainsKey(group.Key))
                    {
                        categoryCount.Add(group.Key, group.Count());
                    }
                }
            }

            return categoryCount;
        }
    }
}
