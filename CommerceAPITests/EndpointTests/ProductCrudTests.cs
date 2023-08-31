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
    public class ProductCrudTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ProductCrudTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetReturnsAllProducts()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            var merchant1 = new Merchant { Name = "Biker Jim's", Category = "Restaurant", Products = new List<Product>() };
            var product = new Product { Name = "STEAK", Description = "2 lbs", Category = "Beef", PriceInCents = 2000, StockQuantity = 5, ReleaseDate = DateTime.UtcNow, MerchantId = merchant1.Id };
            merchant1.Products.Add(product);
            var merchants = new List<Merchant> { merchant1 };
            context.Merchants.AddRange(merchants);
            context.SaveChanges();

            var response = await client.GetAsync($"/api/merchants/{merchant1.Id}/products");
            var content = await response.Content.ReadAsStringAsync();

            string expected = ObjectToJson(merchants);

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
                NamingStrategy = new SnakeCaseNamingStrategy()
            };

            string json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                ContractResolver = contractResolver
            });

            return json;
        }
    }
}
