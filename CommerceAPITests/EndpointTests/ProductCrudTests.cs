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
        public async void GetSingleProduct_ReturnsSingleProductById()
        {
            //arrange
            var context = GetDbContext();
            var client = _factory.CreateClient();

            Product product1 = new() { Name = "Coffee Maker", Description= "Brews up to 12 cups", Category= "Home Appliances", PriceInCents= 1100, ReleaseDate = DateTimeOffset.Parse("2023 - 03 - 01T00:00:00.000Z").UtcDateTime, StockQuantity = 20, MerchantId = 1};
            Product product2 = new() { Name = "product 2", Description = "description 2", Category = "category 2", PriceInCents = 500, ReleaseDate = DateTime.Now.ToUniversalTime(), StockQuantity = 5 };
            List<Product> products = new() { product1, product2 };

            //act
            context.Products.AddRange(products);
            context.SaveChanges();

            var response = await client.GetAsync("/api/products/1");
            var content = await response.Content.ReadAsStringAsync();

            string expected = "{\"id\":1,\"name\":\"Coffee Maker\",\"description\":\"Brews up to 12 cups\",\"category\":\"Home Appliances\",\"priceInCents\":1100,\"stockQuantity\":20,\"releaseDate\":\"2023-03-01T00:00:00Z\",\"merchantId\":1}";

            //assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(expected, content);
        }

        [Fact]
        public async void CreateProduct_CreatesNewProduct_AssignsToMerchant()
        {
            //arrange
            var context = GetDbContext();
            HttpClient client = _factory.CreateClient();

            var merchant1 = new Merchant { Name = "Biker Jim's", Category = "Restaurant" };

            //act
            context.Merchants.Add(merchant1);
            context.SaveChanges();

            string jsonString = "{\"Id\":1,\"Name\":\"Coffee Maker\",\"Description\":\"Brews up to 12 cups\",\"Category\":\"Home Appliances\",\"Price\":1100,\"StockQuantity\":20,\"ReleaseDate\":\"2023-03-01T00:00:00.000Z\",\"MerchantId\":1}";
            StringContent requestContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/merchants/1/products", requestContent);

            //assert
            var newProduct = context.Products.Last();
            response.EnsureSuccessStatusCode();
            Assert.Equal("Coffee Maker", newProduct.Name);
        }

        [Fact]
        public async void UpdateProduct_UpdatesCorrectData()
        {
            //arrange
            Product product1 = new() { Name = "Coffee Maker", Description = "Brews up to 12 cups", Category = "Home Appliances", PriceInCents = 1100, ReleaseDate = DateTimeOffset.Parse("2023 - 03 - 01T00:00:00.000Z").UtcDateTime, StockQuantity = 20, MerchantId = 1 };
            var context = GetDbContext();
            var client = _factory.CreateClient();

            //act
            context.Products.Add(product1);
            context.SaveChanges();

            var jsonString = "{\"id\":1,\"name\":\"Coffee Maker\",\"description\":\"Brews up to 12 cups\",\"category\":\"Home Appliances\",\"priceInCents\":1100,\"stockQuantity\":20,\"releaseDate\":\"2023-03-01T00:00:00Z\",\"merchantId\":1}";
            var requestContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await client.PutAsync("/api/merchants/1/products/1", requestContent);

            //assert
            context.ChangeTracker.Clear();
            Assert.Equal("Coffee Maker", context.Products.Find(1).Name);
        }

        [Fact]
        public async void DeleteProduct_RemovesProductFromDb()
        {
            //arrange
            Product product1 = new() { Name = "Coffee Maker", Description = "Brews up to 12 cups", Category = "Home Appliances", PriceInCents = 1100, ReleaseDate = DateTimeOffset.Parse("2023 - 03 - 01T00:00:00.000Z").UtcDateTime, StockQuantity = 20, MerchantId = 1 };
            var context = GetDbContext(); 
            HttpClient client = _factory.CreateClient();

            //act
            context.Products.Add(product1);
            context.SaveChanges();

            HttpResponseMessage response = await client.DeleteAsync("/api/products/1");

            //assert
            Assert.DoesNotContain(product1, context.Products);
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