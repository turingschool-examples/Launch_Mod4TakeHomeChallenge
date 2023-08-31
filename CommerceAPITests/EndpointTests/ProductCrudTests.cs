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
        public async void GetProducts_ReturnsListOfProducts()
        {
            // Arrange
            Merchant merchant = new Merchant { Name = "McDonalds", Category = "Restaurant" };
            var product1 = new Product
            {
                Id = 1, 
                Name = "Coffee Maker", 
                Description = "Brews up to 12 cups then breaks", 
                Category = "Home Appliances", 
                Price = 1100, 
                StockQuantity = 20, 
                ReleaseDate = "03/05/2023", 
                MerchantId = 1
    };

            var product2 = new Product
            {
              Id = 2, 
                Name = "Electric Guitar", 
                Description = "Solid Guitar", 
                Category = "Musical Instruments",
                Price = 600, 
                StockQuantity = 5, 
                ReleaseDate = "05/25/2023", 
                MerchantId = 1
    };

            var context = GetDbContext();
            context.Merchants.Add(merchant);
            context.Products.AddRange(product1, product2);
            context.SaveChanges();

            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync($"api/merchants/{merchant.Id}/products");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            var expected = ObjectToJson(new List<Product> { product1, product2 });
           Assert.Contains("Coffee Maker",expected);
        }
        [Fact]
        public async void GetProduct_ReturnsProductFromId()
        {
            var context = GetDbContext();
            // Arrange
            Merchant merchant = new Merchant { Name = "McDonalds", Category = "Restaurant" };
            var product1 = new Product
            {
                Id = 1,
                Name = "Coffee Maker",
                Description = "Brews up to 12 cups then breaks",
                Category = "Home Appliances",
                Price = 1100,
                StockQuantity = 20,
                ReleaseDate = "03/05/2023",
                MerchantId = 1
            };

            var product2 = new Product
            {
                Id = 2,
                Name = "Electric Guitar",
                Description = "Solid Guitar",
                Category = "Musical Instruments",
                Price = 600,
                StockQuantity = 5,
                ReleaseDate = "05/25/2023",
                MerchantId = 1
            };

            List<Product> products = new() { product1, product2 };

            
            context.Products.AddRange(products);
            context.SaveChanges();

            HttpClient client = _factory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync($"api/merchants/{merchant.Id}/products/{product1.Id}");
            string content = await response.Content.ReadAsStringAsync();

            // Assert
            string expected = ObjectToJson(product1);

            context.ChangeTracker.Clear();

            response.EnsureSuccessStatusCode();
            Assert.Equal(expected, content);
            Assert.Contains("Coffee Maker", expected);
            Assert.Contains("Home Appliances", expected);
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
