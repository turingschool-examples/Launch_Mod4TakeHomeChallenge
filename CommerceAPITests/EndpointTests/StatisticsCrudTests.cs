using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommerceAPI.Models;
using CommerceAPI.DataAccess;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CommerceAPITests.EndpointTests
{
    public class StatisticsCrudTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        public StatisticsCrudTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async void Test_GetCountOfProductsPerCategory()
        {
            var TimeDateToUse = DateTime.Now;
            Merchant merchant1 = new Merchant { Name = "Merchant Man", Category = "Online" };

            Product product1 = new Product { Name = "Product1", Category = "Product", Description = "a product", Price = 100, ReleaseDate = TimeDateToUse, StockQuanity = 10, MerchantId = 1 };
            Product product2 = new Product { Name = "Product2", Category = "Product", Description = "a second product", Price = 100, ReleaseDate = TimeDateToUse, StockQuanity = 10, MerchantId = 1 };

            var context = GetDbContext();

            context.Merchants.Add(merchant1);
            context.Products.AddRange(product1, product2);
            context.SaveChanges();

            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/Statistics");
            var content = await response.Content.ReadAsStringAsync();

            Assert.Contains("Product: 2", content);
        }

        private string ObjectToJson(object obj)
        {
            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            string json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                ContractResolver = contractResolver
            });

            return json;
        }

        private CommerceApiContext GetDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<CommerceApiContext>();
            optionsBuilder.UseInMemoryDatabase("TestDatabase");

            var context = new CommerceApiContext(optionsBuilder.Options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            return context;
        }
    }
}
