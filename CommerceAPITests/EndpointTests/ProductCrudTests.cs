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

        //var product = _context.Products.Where(p => p.MerchantId == merchantId).FirstOrDefault(p => p.Id == productId);
        // THIS IS TO REPLACE merchant.Products


        [Fact]
        public async void PostCreatesNewProduct()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            var merchant1 = new Merchant { Name = "Biker Jim's", Category = "Restaurant" };
            context.Merchants.Add(merchant1);
            context.SaveChanges();

            var jsonString = "{\"Id\": 1, \"Name\": \"Coffee Maker\", \"Description\": \"Brews up to 12 cups\", \"Category\": \"Home Appliances\",  \"Price\": 1100, \"StockQuantity\": 20, \"ReleaseDate\": \"2023-03-01T00:00:00.000Z\", \"MerchantId\": 1}";

            var requestContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync($"/api/merchants/{merchant1.Id}/products", requestContent);

            response.EnsureSuccessStatusCode();

            var newProduct = context.Products.Last();

            Assert.Equal("Coffee Maker", newProduct.Name);
            Assert.Equal("Home Appliances", newProduct.Category);
            Assert.Equal(1, newProduct.MerchantId);

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
