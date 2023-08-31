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




        [Fact]
        public async void GetProductById_GetsProduct()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            var merchant = new Merchant { Name = "Biker Jim's", Category = "Restaurant" };
            context.Merchants.Add(merchant);
            context.SaveChanges();

            var expectedProduct = new Product { MerchantId = merchant.Id, Name = "Dunkaroos", Description = "Discontinued", Category = "Snack", Price = 5.99M, StockQuantity = 100, ReleaseDate = new DateTime(2023, 1, 1, 0, 0, 0).ToUniversalTime() };
            context.Products.Add(expectedProduct);
            context.SaveChanges();

            HttpResponseMessage response = await client.GetAsync($"/api/merchants/{merchant.Id}/products/{expectedProduct.ProductId}");
            string content = await response.Content.ReadAsStringAsync();

            string expected = ObjectToJson(expectedProduct);

            response.EnsureSuccessStatusCode();
            Assert.Equal(expected, content);

        }




        [Fact]
        public async void UpdateProduct_UpdatesProduct()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            var merchant = new Merchant { Name = "Biker Jim's", Category = "Restaurant" };
            context.Merchants.Add(merchant);
            context.SaveChanges();

            var product = new Product { MerchantId = merchant.Id, Name = "Dunkaroos", Description = "Discontinued", Category = "Snack", Price = 5.99M, StockQuantity = 100, ReleaseDate = new DateTime(2023, 1, 1, 0, 0, 0).ToUniversalTime() };
            context.Products.Add(product);
            context.SaveChanges();

            string jsonString = "{\"Name\": \"Skeletony\", \"Description\": \"Discontinued\", \"Category\": \"Snack\", \"Price\": \"3\", \"StockQuantity\": \"25\"}";
            var requestContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PutAsync($"/api/merchants/{merchant.Id}/products/{product.ProductId}", requestContent); string content = await response.Content.ReadAsStringAsync();


            response.EnsureSuccessStatusCode();

            context.ChangeTracker.Clear();
           
            Assert.Equal("Skeletony", context.Products.Find(1).Name);

        }




        [Fact]
        public async void DeleteProduct_DeletesProduct()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            var merchant = new Merchant { Name = "Biker Jim's", Category = "Restaurant" };
            context.Merchants.Add(merchant);
            context.SaveChanges();

            var product = new Product { MerchantId = merchant.Id, Name = "Dunkaroos", Description = "Discontinued", Category = "Snack", Price = 5.99M, StockQuantity = 100, ReleaseDate = new DateTime(2023, 1, 1, 0, 0, 0).ToUniversalTime() };
            context.Products.Add(product);
            context.SaveChanges();

            HttpResponseMessage response = await client.DeleteAsync($"/api/merchants/{merchant.Id}/products/{product.ProductId}");
            var content = await response.Content.ReadAsStringAsync(); 
            
            


            response.EnsureSuccessStatusCode();

            

            Assert.DoesNotContain("Dunkaroos", content);

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
                // NamingStrategy = new SnakeCaseNamingStrategy()
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
