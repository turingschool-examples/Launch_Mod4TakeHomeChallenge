using CommerceAPI.DataAccess;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public async void GetSingleProduct_ReturnsSingleProductById()//need json string instead of product objects.
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            Product product1 = new() { Name = "product 1", Description="description 1", Category="category 1", PriceInCents=400, ReleaseDate = DateTime.Now.ToUniversalTime(), StockQuantity=1 };
            Product product2 = new() { Name = "product 2", Description = "description 2", Category = "category 2", PriceInCents = 500, ReleaseDate = DateTime.Now.ToUniversalTime(), StockQuantity = 5 };
            List<Product> products = new() { product1, product2 };
            context.Products.AddRange(products);
            context.SaveChanges();

            var response = await client.GetAsync("/api/products/1");
            var content = await response.Content.ReadAsStringAsync();

            string expected = ObjectToJson(product1);

            response.EnsureSuccessStatusCode();
            Assert.Equal(expected, content);
        }

        [Fact]
        public async void CreateProduct_CreatesNewProduct_AssignsToMerchant()
        {

        }

        [Fact]
        public async void UpdateProduct_UpdatesCorrectData()
        {

        }

        [Fact]
        public async void DeleteProduct_RemovesProductFromDb()
        {

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