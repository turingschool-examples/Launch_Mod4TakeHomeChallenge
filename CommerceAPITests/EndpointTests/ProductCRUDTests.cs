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
        public async void GetReturnsAllProductsbyMerchantId()
        {
            var context = GetDbContext();
            HttpClient client = _factory.CreateClient();

            var merchant1 = new Merchant { Name = "Biker Jim's", Category = "Restaurant" };
            var merchant2 = new Merchant { Name = "REI", Category = "Outdoor" };
            var merchants = new List<Merchant> { merchant1, merchant2 };
            context.Merchants.AddRange(merchants);
            context.SaveChanges();

            var product1 = new Product { Name = "The Hunger Games", Description = "Young Adult", Category = "Books", PriceInCents = 1000, StockQuantity = 10 };
            var product2 = new Product { Name = "Green Eggs & Ham", Description = "Kids", Category = "Books", PriceInCents = 500, StockQuantity = 1 };
            var product3 = new Product { Name = "Sneakers", Description = "Run faster", Category = "Shoes", PriceInCents = 10000, StockQuantity = 100 };
            var products = new List<Product> { product1, product2, product3 };
            context.Products.AddRange(products);
            merchant1.Products.Add(product1);
            merchant1.Products.Add(product2);
            merchant1.Products.Add(product3);
            context.SaveChanges();

            HttpResponseMessage response = await client.GetAsync($"/api/merchants/{merchant1.Id}/products");
            var content = await response.Content.ReadAsStringAsync();

            string expected = ObjectToJson(products);

            response.EnsureSuccessStatusCode();
            Assert.Equal(expected, content);
        }

        [Fact]
        public async void GetReturnsSingleProductById()
        {
            var context = GetDbContext();
            HttpClient client = _factory.CreateClient();

            var merchant1 = new Merchant { Name = "Biker Jim's", Category = "Restaurant" };
            var merchant2 = new Merchant { Name = "REI", Category = "Outdoor" };
            var merchants = new List<Merchant> { merchant1, merchant2 };
            context.Merchants.AddRange(merchants);
            context.SaveChanges();

            var product1 = new Product { Name = "The Hunger Games", Description = "Young Adult", Category = "Books", PriceInCents = 1000, StockQuantity = 10 };
            var product2 = new Product { Name = "Green Eggs & Ham", Description = "Kids", Category = "Books", PriceInCents = 500, StockQuantity = 1 };
            var product3 = new Product { Name = "Sneakers", Description = "Run faster", Category = "Shoes", PriceInCents = 10000, StockQuantity = 100 };
            var products = new List<Product> { product1, product2, product3 };
            context.Products.AddRange(products);
            merchant1.Products.Add(product1);
            merchant1.Products.Add(product2);
            merchant1.Products.Add(product3);
            context.SaveChanges();

            HttpResponseMessage response = await client.GetAsync($"/api/merchants/{merchant1.Id}/products/{product1.Id}");
            var content = await response.Content.ReadAsStringAsync();

            string expected = ObjectToJson(product1);

            response.EnsureSuccessStatusCode();
            Assert.Equal(expected, content);
        }

        [Fact]
        public async void PostCreatesProduct()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            var merchant1 = new Merchant { Name = "REI", Category = "Outdoor" };
            context.Merchants.Add(merchant1);
            context.SaveChanges();

            var jsonString = "{\"Id\": 1, \"Name\": \"Coffee Maker\", \"Description\": \"Brews up to 12 cups\", \"Category\": \"Home Appliances\", \"PriceInCents\": 1100, \"StockQuantity\": 20, \"ReleaseDate\": \"2023-03-01T00:00:00.000Z\", \"MerchantId\": 1}";
            var requestContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"/api/merchants/{merchant1.Id}/products", requestContent);

            response.EnsureSuccessStatusCode();

            var newProduct = context.Products.Last();

            Assert.Equal("Coffee Maker", newProduct.Name);
            Assert.Equal(1100, newProduct.PriceInCents);
        }

        [Fact]
        public async void UpdateProduct_UpdatesExistingProduct()
        {
            CommerceApiContext context = GetDbContext();
            HttpClient client = _factory.CreateClient();

            var merchant1 = new Merchant { Name = "REI", Category = "Outdoor" };
            context.Merchants.Add(merchant1);
            context.SaveChanges();

            var product1 = new Product { Name = "The Hunger Games", Description = "Young Adult", Category = "Books", PriceInCents = 1000, StockQuantity = 10 };
            merchant1.Products.Add(product1);
            context.Products.Add(product1);
            context.SaveChanges();

            var jsonString = "{\"Id\": 1, \"Name\": \"Coffee Maker\", \"Description\": \"Brews up to 12 cups\", \"Category\": \"Home Appliances\", \"PriceInCents\": 1100, \"StockQuantity\": 20, \"ReleaseDate\": \"2023-03-01T00:00:00.000Z\", \"MerchantId\": 1}";
            var requestContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"/api/merchants/{merchant1.Id}/products/{product1.Id}", requestContent);

            context.ChangeTracker.Clear();

            Assert.Equal(204, (int)response.StatusCode);
            Assert.Equal("Coffee Maker", context.Products.Find(1).Name);
            Assert.Equal("Brews up to 12 cups", context.Products.Find(1).Description);
        }

        [Fact]
        public async void DeleteProduct_RemovesExistingProduct()
        {
            CommerceApiContext context = GetDbContext();
            HttpClient client = _factory.CreateClient();

            var merchant1 = new Merchant { Name = "REI", Category = "Outdoor" };
            context.Merchants.Add(merchant1);
            context.SaveChanges();

            var product1 = new Product { Name = "The Hunger Games", Description = "Young Adult", Category = "Books", PriceInCents = 1000, StockQuantity = 10 };
            merchant1.Products.Add(product1);
            context.SaveChanges();

            var response = await client.DeleteAsync($"/api/merchants/{merchant1.Id}/products/{product1.Id}");
            var content = await response.Content.ReadAsStringAsync();

            Assert.DoesNotContain("The Hunger Games", content);
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
