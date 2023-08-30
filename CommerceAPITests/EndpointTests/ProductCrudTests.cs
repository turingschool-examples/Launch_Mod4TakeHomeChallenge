using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using CommerceAPI.DataAccess;
using CommerceAPI.Models;
using static System.Reflection.Metadata.BlobBuilder;
using System.Text;

namespace CommerceAPITests.EndpointTests
{
    [Collection("Controller Tests")]
    public class ProductCrudTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ProductCrudTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }


        [Fact]
        public async void GetProducts_ReturnsListOfProducts()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            var merchant1 = new Merchant { Name = "Circle K", Category = "Convenience Store" };
            var merchant2 = new Merchant { Name = "Biker Jim's", Category = "Restaurant" };
            var merchants = new List<Merchant> { merchant1, merchant2 };
            context.Merchants.AddRange(merchants);
            context.SaveChanges();

            var product1 = new Product { MerchantId = merchant1.Id, Name = "Slim Jims", Description = "Meat Stick", Category = "Snack", PriceInCents = 99, StockQuantity = 100, ReleaseDate = new DateTime(2000, 1, 1, 0, 0, 0).ToUniversalTime() };
            var product2 = new Product { MerchantId = merchant1.Id, Name = "Sweet Tarts", Description = "Sweet and Sour", Category = "Candy", PriceInCents = 149, StockQuantity = 50, ReleaseDate = new DateTime(2000, 1, 1, 0, 0, 0).ToUniversalTime() };
            var product3 = new Product { MerchantId = merchant1.Id, Name = "Slim Jims", Description = "Meat Stick", Category = "Snack", PriceInCents = 99, StockQuantity = 100, ReleaseDate = new DateTime(2000, 1, 1, 0, 0, 0).ToUniversalTime() };
            List<Product> products = new () { product1, product2, product3 };
            context.Products.AddRange(products);
            context.SaveChanges();

            HttpResponseMessage response = await client.GetAsync($"/api/merchants/{merchant1.Id}/products");
            string content = await response.Content.ReadAsStringAsync();

            string expected = ObjectToJson(products);

            response.EnsureSuccessStatusCode();
            Assert.Equal(expected, content);
        }

        [Fact]
        public async void GetProductById_ReturnsProductWithGivenId()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            var merchant1 = new Merchant { Name = "Circle K", Category = "Convenience Store" };
            var merchant2 = new Merchant { Name = "Biker Jim's", Category = "Restaurant" };
            var merchants = new List<Merchant> { merchant1, merchant2 };
            context.Merchants.AddRange(merchants);
            context.SaveChanges();

            var product1 = new Product { MerchantId = merchant1.Id, Name = "Slim Jims", Description = "Meat Stick", Category = "Snack", PriceInCents = 99, StockQuantity = 100, ReleaseDate = new DateTime(2000, 1, 1, 0, 0, 0).ToUniversalTime() };
            var product2 = new Product { MerchantId = merchant1.Id, Name = "Sweet Tarts", Description = "Sweet and Sour", Category = "Candy", PriceInCents = 149, StockQuantity = 50, ReleaseDate = new DateTime(2000, 1, 1, 0, 0, 0).ToUniversalTime() };
            var product3 = new Product { MerchantId = merchant1.Id, Name = "Slim Jims", Description = "Meat Stick", Category = "Snack", PriceInCents = 99, StockQuantity = 100, ReleaseDate = new DateTime(2000, 1, 1, 0, 0, 0).ToUniversalTime() };
            List<Product> products = new() { product1, product2, product3 };
            context.Products.AddRange(products);
            context.SaveChanges();

            HttpResponseMessage response = await client.GetAsync($"/api/merchants/{merchant1.Id}/products/{product1.Id}");
            string content = await response.Content.ReadAsStringAsync();

            string expected = ObjectToJson(product1);

            response.EnsureSuccessStatusCode();
            Assert.Equal(expected, content);
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
    }
}
