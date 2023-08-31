using CommerceAPI.DataAccess;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using CommerceAPI.Models;

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
        public async void GetSpecificProductByPrimaryKey()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            var merchant1 = new Merchant { Name = "Biker Jim's", Category = "Restaurant" };
            var merchant2 = new Merchant { Name = "REI", Category = "Outdoor" };
            var merchants = new List<Merchant> { merchant1, merchant2 };
            context.Merchants.AddRange(merchants);
            var product1 = new Product { Name = "Burger", Category = "Beef", Description = "Tasty", Price = 12.99M, StockQuantity = 4 };
            var product2 = new Product { Name = "Hot Dog", Category = "Mystery", Description = "Good", Price = 8.99M, StockQuantity = 3 };
            var products = new List<Product> { product1, product2 };
            context.Products.AddRange(products);
            merchant1.Products.Add(product1);
            merchant1.Products.Add(product2);
            context.SaveChanges();
            var response = await client.GetAsync($"/api/merchants/{merchant1.Id}/products/{product1.ProductId}");
            var content = await response.Content.ReadAsStringAsync();

            string expected = ObjectToJson(product1);

            response.EnsureSuccessStatusCode();
            Assert.Equal(expected, content);
        }

        [Fact]
        public async void Post_CreatesNewProduct()
        {
            CommerceApiContext context = GetDbContext();

            HttpClient client = _factory.CreateClient();
            string jsonString = "{\"Id\": 1, \"Name\": \"Coffee Maker\", \"Description\": \"Brews up to 12 cups\", \"Category\": \"Home Appliances\", \"Price\": 1100, \"StockQuantity\": 20, \"ReleaseDate\": \"2023-03-01T00:00:00.000Z\", \"MerchantId\": 1}";
            StringContent requestContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var merchant1 = new Merchant { Name = "Biker Jim's", Category = "Restaurant" };
            context.Merchants.Add(merchant1);
            context.SaveChanges();

            HttpResponseMessage response = await client.PostAsync($"/api/merchants/{merchant1.Id}/products", requestContent);

            Assert.Equal("Created", response.StatusCode.ToString());
            Assert.Equal(201, (int)response.StatusCode);

            var newProduct = context.Products.Last();

            Assert.Equal(1100, newProduct.Price);
            Assert.Equal(1, newProduct.MerchantId);
        }

        [Fact]
        public async void DeleteProduct_RemovesProductFromDB()
        {
            //Arrange
            var context = GetDbContext();

            var merchant1 = new Merchant { Name = "Biker Jim's", Category = "Restaurant" };
            var merchant2 = new Merchant { Name = "REI", Category = "Outdoor" };
            var merchants = new List<Merchant> { merchant1, merchant2 };
            context.Merchants.AddRange(merchants);
            var product1 = new Product { Name = "Burger", Category = "Beef", Description = "Tasty", Price = 12.99M, StockQuantity = 4 };
            var product2 = new Product { Name = "Hot Dog", Category = "Mystery", Description = "Good", Price = 8.99M, StockQuantity = 3 };
            var products = new List<Product> { product1, product2 };
            context.Products.AddRange(products);
            merchant1.Products.Add(product1);
            merchant1.Products.Add(product2);
            context.SaveChanges();
            HttpClient client = _factory.CreateClient();

            //Act
            HttpResponseMessage response = await client.DeleteAsync($"/api/merchants/{merchant1.Id}/products/{product1.ProductId}");
            string content = await response.Content.ReadAsStringAsync();

            //Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(1, context.Products.Where(p => p.MerchantId == 1).Count());
            Assert.Equal("Hot Dog", context.Products.First().Name);
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
