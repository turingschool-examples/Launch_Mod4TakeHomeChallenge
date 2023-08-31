using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
			var merchant = _context.Merchants
				.Where(m => m.Id == merchantId)
				.Include(m => m.Products)
				.FirstOrDefault();

			return new JsonResult(merchant.Products);
		}

		[HttpGet]
		[Route("/api/products/{productId:int}")]
		public ActionResult GetProduct(int productId)
		{
			var product = _context.Products.Find(productId);
			return new JsonResult(product);
		}

		[HttpPost]
		public ActionResult CreateProduct(int merchantId, Product product)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}

			var merchant = _context.Merchants
				.Where(m => m.Id == merchantId)
				.Include(m => m.Products)
				.FirstOrDefault();

			merchant.Products.Add(product);
			_context.SaveChanges();
			Response.StatusCode = 201;
			return new JsonResult(product);
		}

		[HttpPut]
		[Route("/api/products/{productId:int}")]
		public ActionResult UpdateProduct(int productId, Product product)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}
			product.ProductId = productId;
			_context.Products.Update(product);
			_context.SaveChanges();
			Response.StatusCode = 204;
			return new JsonResult(product);
		}
	}
}
