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
    public class MerchantCrudTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public MerchantCrudTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async void GetReturnsAllMerchants()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            var merchant1 = new Merchant { Name = "Biker Jim's", Category = "Restaurant" };
            var merchant2 = new Merchant { Name = "REI", Category = "Outdoor" };
            var merchants = new List<Merchant> { merchant1, merchant2 };
            context.Merchants.AddRange(merchants);
            context.SaveChanges();

            var response = await client.GetAsync("/api/merchants");
            var content = await response.Content.ReadAsStringAsync();

            string expected = ObjectToJson(merchants);

            response.EnsureSuccessStatusCode();
            Assert.Equal(expected, content);
        }

        [Fact]
        public async void PostCreatesNewMerchant()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            var jsonString = "{\"Name\":\"Walter's 303\", \"Category\":\"Restaurant\"}";
            var requestContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/merchants", requestContent);

            response.EnsureSuccessStatusCode();

            var newMerchant = context.Merchants.First();

            Assert.Equal("Walter's 303", newMerchant.Name);
            Assert.Equal("Restaurant", newMerchant.Category);
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
