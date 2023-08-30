﻿using CommerceAPI.DataAccess;
using CommerceAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            var merchant = _context.Merchants.FirstOrDefault(m => m.Id == merchantId);
            if (merchant == null)
            {
                return NotFound();
            }
            var products = _context.Products.Where(p => p.MerchantId == merchantId);
            return new JsonResult(products.OrderBy(p => p.Id));
        }
    }
}
