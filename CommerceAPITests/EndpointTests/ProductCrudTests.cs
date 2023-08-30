using Microsoft.AspNetCore.Mvc.Testing;
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

namespace CommerceAPITests.EndpointTests
{
    public class ProductCrudTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ProductCrudTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async void Teat_GetAllProductsFromMerchant()
        {
            var TimeDateToUse = DateTime.Now;
            Merchant merchant1 = new Merchant {Name = "Merchant Man", Category = "Online"};

            Product product1 = new Product {Name = "Product1", Category = "Product", Description = "a product", Price = 100, ReleaseDate = TimeDateToUse, StockQuanity = 10, MerchantId = 1 };
            Product product2 = new Product { Name = "Product2", Category = "Product", Description = "a second product", Price = 100, ReleaseDate = TimeDateToUse, StockQuanity = 10, MerchantId = 1 };

            var context = GetDbContext();

            context.Merchants.Add(merchant1);
            context.Products.AddRange(product1, product2);
            context.SaveChanges();

            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/merchants/{merchant1.Id}/products");
            var content = await response.Content.ReadAsStringAsync();

            var expected = ObjectToJson(new List<Product> { product1, product2 });

            context.ChangeTracker.Clear();

            Assert.Equal(expected, content);
        }

        [Fact]
        public async void Test_GetProductFromMerchantViaId()
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

            var response = await client.GetAsync($"/api/merchants/{merchant1.Id}/products/{product1.Id}");
            var content = await response.Content.ReadAsStringAsync();

            var expected = ObjectToJson(product1);

            context.ChangeTracker.Clear();

            Assert.Equal(expected, content);
        }

        [Fact]
        public async void Test_CreatesProductinDB()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            Merchant merchant1 = new Merchant { Name = "Merchant Man", Category = "Online" };
            context.Merchants.Add(merchant1);
            context.SaveChanges();

            var jsonString = "{\"Name\": \"Coffee Maker\", \"Description\": \"Brews up to 12 cups\", \"Category\": \"Home Appliances\", \"Price\": 1100, \"StockQuantity\": 20, \"ReleaseDate\": \"2023-03-01T00:00:00.000Z\", \"MerchantId\": 1}";
            var requestContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"/api/merchants/{merchant1.Id}/products", requestContent);

            var newProduct = context.Products.First();

            Assert.Equal("Coffee Maker", newProduct.Name);
        }

        [Fact]
        public async Task Test_UpdatesProductinDB()
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

            var jsonString = "{\"Name\": \"Coffee Maker\", \"Description\": \"Brews up to 12 cups\", \"Category\": \"Home Appliances\", \"Price\": 1100, \"StockQuantity\": 20, \"ReleaseDate\": \"2023-03-01T00:00:00.000Z\", \"MerchantId\": 1}";
            var requestContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"/api/merchants/{merchant1.Id}/products/{product1.Id}", requestContent);

            Assert.Equal("Coffee Maker", context.Products.FirstOrDefault(p => p.Id == product1.Id).Name);
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
