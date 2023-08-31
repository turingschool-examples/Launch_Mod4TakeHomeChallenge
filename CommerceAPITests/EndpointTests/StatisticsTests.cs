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
    public class StatisticsTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public StatisticsTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async void CountByCategory_ReturnsEachCategoryWithACountOfProducts()
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
            var product3 = new Product { MerchantId = merchant1.Id, Name = "Arizona Iced Tea", Description = "Iced Tea", Category = "Drink", PriceInCents = 99, StockQuantity = 200, ReleaseDate = new DateTime(2000, 1, 1, 0, 0, 0).ToUniversalTime() };
            var product4 = new Product { MerchantId = merchant2.Id, Name = "Slim Jims", Description = "Meat Stick", Category = "Snack", PriceInCents = 99, StockQuantity = 100, ReleaseDate = new DateTime(2000, 1, 1, 0, 0, 0).ToUniversalTime() };
            var product5 = new Product { MerchantId = merchant2.Id, Name = "Sweet Tarts", Description = "Sweet and Sour", Category = "Candy", PriceInCents = 149, StockQuantity = 50, ReleaseDate = new DateTime(2000, 1, 1, 0, 0, 0).ToUniversalTime() };
            var product6 = new Product { MerchantId = merchant2.Id, Name = "Cheese Its", Description = "Made With Real Cheese", Category = "Snack", PriceInCents = 99, StockQuantity = 200, ReleaseDate = new DateTime(2000, 1, 1, 0, 0, 0).ToUniversalTime() };

            List<Product> products = new() { product1, product2, product3, product4, product5, product6 };
            context.Products.AddRange(products);
            context.SaveChanges();

            HttpResponseMessage response = await client.GetAsync($"/api/bycategory");
            string content = await response.Content.ReadAsStringAsync();
            var categoryCount = new Dictionary<string, int>
            {
                {"Candy", 2 },
                {"Snack", 3 },
                {"Drink", 1 }
            };
            string expected = ObjectToJson(categoryCount);

            response.EnsureSuccessStatusCode();
            Assert.Equal(expected, content);
        }

        [Fact]
        public async void AvgPricePerMerchant_ReturnsEachMerchantWithItsAvgPrice()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            var merchant1 = new Merchant { Name = "Circle K", Category = "Convenience Store" };
            var merchant2 = new Merchant { Name = "Biker Jims", Category = "Restaurant" };
            var merchants = new List<Merchant> { merchant1, merchant2 };
            context.Merchants.AddRange(merchants);
            context.SaveChanges();

            var product1 = new Product { MerchantId = merchant1.Id, Name = "Slim Jims", Description = "Meat Stick", Category = "Snack", PriceInCents = 99, StockQuantity = 100, ReleaseDate = new DateTime(2000, 1, 1, 0, 0, 0).ToUniversalTime() };
            var product2 = new Product { MerchantId = merchant1.Id, Name = "Sweet Tarts", Description = "Sweet and Sour", Category = "Candy", PriceInCents = 149, StockQuantity = 50, ReleaseDate = new DateTime(2000, 1, 1, 0, 0, 0).ToUniversalTime() };
            var product3 = new Product { MerchantId = merchant1.Id, Name = "Arizona Iced Tea", Description = "Iced Tea", Category = "Drink", PriceInCents = 99, StockQuantity = 200, ReleaseDate = new DateTime(2000, 1, 1, 0, 0, 0).ToUniversalTime() };
            var product4 = new Product { MerchantId = merchant2.Id, Name = "Slim Jims", Description = "Meat Stick", Category = "Snack", PriceInCents = 99, StockQuantity = 100, ReleaseDate = new DateTime(2000, 1, 1, 0, 0, 0).ToUniversalTime() };
            var product5 = new Product { MerchantId = merchant2.Id, Name = "Sweet Tarts", Description = "Sweet and Sour", Category = "Candy", PriceInCents = 149, StockQuantity = 50, ReleaseDate = new DateTime(2000, 1, 1, 0, 0, 0).ToUniversalTime() };
            var product6 = new Product { MerchantId = merchant2.Id, Name = "Cheese Its", Description = "Made With Real Cheese", Category = "Snack", PriceInCents = 299, StockQuantity = 200, ReleaseDate = new DateTime(2000, 1, 1, 0, 0, 0).ToUniversalTime() };

            List<Product> products = new() { product1, product2, product3, product4, product5, product6 };
            context.Products.AddRange(products);
            context.SaveChanges();

            HttpResponseMessage response = await client.GetAsync($"/api/avgprice");
            string content = await response.Content.ReadAsStringAsync();
            var avgPrices = new Dictionary<string, int>
            {
                {"Circle K", 347/3 },
                {"Biker Jims", 547/3 }
            };
            string expected = ObjectToJson(avgPrices);

            response.EnsureSuccessStatusCode();
            Assert.Equal(expected, content);
        }

        [Fact]
        public async void NewestProductByMerchant_ReturnsEachMerchantAndTheNewestDateOutOfItsProducts()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            var merchant1 = new Merchant { Name = "Circle K", Category = "Convenience Store" };
            var merchant2 = new Merchant { Name = "Biker Jims", Category = "Restaurant" };
            var merchants = new List<Merchant> { merchant1, merchant2 };
            context.Merchants.AddRange(merchants);
            context.SaveChanges();

            var product1 = new Product { MerchantId = merchant1.Id, Name = "Slim Jims", Description = "Meat Stick", Category = "Snack", PriceInCents = 99, StockQuantity = 100, ReleaseDate = new DateTime(2000, 1, 1, 0, 0, 0).ToUniversalTime() };
            var product2 = new Product { MerchantId = merchant1.Id, Name = "Sweet Tarts", Description = "Sweet and Sour", Category = "Candy", PriceInCents = 149, StockQuantity = 50, ReleaseDate = new DateTime(2012, 1, 3, 2, 0, 0).ToUniversalTime() };
            var product3 = new Product { MerchantId = merchant1.Id, Name = "Arizona Iced Tea", Description = "Iced Tea", Category = "Drink", PriceInCents = 99, StockQuantity = 200, ReleaseDate = new DateTime(2000, 1, 1, 0, 0, 0).ToUniversalTime() };
            var product4 = new Product { MerchantId = merchant2.Id, Name = "Slim Jims", Description = "Meat Stick", Category = "Snack", PriceInCents = 99, StockQuantity = 100, ReleaseDate = new DateTime(2020, 8, 8, 3, 0, 0).ToUniversalTime() };
            var product5 = new Product { MerchantId = merchant2.Id, Name = "Sweet Tarts", Description = "Sweet and Sour", Category = "Candy", PriceInCents = 149, StockQuantity = 50, ReleaseDate = new DateTime(2000, 1, 1, 0, 0, 0).ToUniversalTime() };
            var product6 = new Product { MerchantId = merchant2.Id, Name = "Cheese Its", Description = "Made With Real Cheese", Category = "Snack", PriceInCents = 299, StockQuantity = 200, ReleaseDate = new DateTime(2000, 1, 1, 0, 0, 0).ToUniversalTime() };

            List<Product> products = new() { product1, product2, product3, product4, product5, product6 };
            context.Products.AddRange(products);
            context.SaveChanges();

            HttpResponseMessage response = await client.GetAsync($"/api/newestproduct");
            string content = await response.Content.ReadAsStringAsync();
            var mostRecent = new Dictionary<string, DateTime>
            {
                {"Circle K", new DateTime(2012, 1, 3, 2, 0, 0).ToUniversalTime() },
                {"Biker Jims", new DateTime(2020, 8, 8, 3, 0, 0).ToUniversalTime() }
            };
            string expected = ObjectToJson(mostRecent);

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
