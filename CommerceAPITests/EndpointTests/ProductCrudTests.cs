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
	[Collection("Commerce API Tests")]
	public class ProductCrudTests : IClassFixture<WebApplicationFactory<Program>>
	{
		private readonly WebApplicationFactory<Program> _factory;

		public ProductCrudTests(WebApplicationFactory<Program> factory)
		{
			_factory = factory;
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

		[Fact]
		public async void GetProducts_ReturnsProductsForAMerchant()
		{
			var merchant = new Merchant { Name = "Acme", Category = "Everything" };
			var product1 = new Product { Name = "Anvil", Category = "Anti-Roadrunner Tech", Description = "A large anvil", PriceInCents = 24999, ReleaseDate = DateTime.Now, StockQuantity = 3 };
			var product2 = new Product { Name = "Coffee Maker", Category = "Home Appliances", Description = "Brews up to 12 cups then breaks", PriceInCents = 11500, ReleaseDate = DateTime.Now, StockQuantity = 20 };
			merchant.Products.Add(product1);
			merchant.Products.Add(product2);

			var context = GetDbContext();
			context.Merchants.Add(merchant);
			context.SaveChanges();

			var client = _factory.CreateClient();
			var response = await client.GetAsync($"/api/merchants/{merchant.Id}/products");
			var content = await response.Content.ReadAsStringAsync();
			var expected = ObjectToJson(merchant.Products);

			response.EnsureSuccessStatusCode();
			Assert.Equal(expected, content);
		}

		[Fact]
		public async void GetProduct_ReturnsSingleProduct()
		{
			var merchant = new Merchant { Name = "Acme", Category = "Everything" };
			var product1 = new Product { Name = "Anvil", Category = "Anti-Roadrunner Tech", Description = "A large anvil", PriceInCents = 24999, ReleaseDate = DateTime.Now, StockQuantity = 3 };
			var product2 = new Product { Name = "Coffee Maker", Category = "Home Appliances", Description = "Brews up to 12 cups then breaks", PriceInCents = 11500, ReleaseDate = DateTime.Now, StockQuantity = 20 };
			merchant.Products.Add(product1);
			merchant.Products.Add(product2);

			var context = GetDbContext();
			context.Merchants.Add(merchant);
			context.SaveChanges();

			var client = _factory.CreateClient();
			var response = await client.GetAsync($"/api/products/{product1.ProductId}");
			var content = await response.Content.ReadAsStringAsync();
			var expected = ObjectToJson(product1);

			response.EnsureSuccessStatusCode();
			Assert.Equal(expected, content);
		}

		[Fact]
		public async void PostProduct_AddsProductToDb()
		{
			var merchant = new Merchant { Name = "Acme", Category = "Everything" };
			var context = GetDbContext();
			var client = _factory.CreateClient();
			context.Merchants.Add(merchant);
			context.SaveChanges();

			string jsonString = "{\"Id\": 1, \"Name\": \"Coffee Maker\", \"Description\": \"Brews up to 12 cups\", \"Category\": \"Home Appliances\", \"Price\": 1100, \"StockQuantity\": 20, \"ReleaseDate\": \"2023-03-01T00:00:00.000Z\"}";
			var requestContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
			var response = await client.PostAsync($"/api/merchants/{merchant.Id}/products" ,requestContent);

			Assert.Equal(201, (int)response.StatusCode);

			var newProduct = context.Products.Last();
			Assert.Equal("Coffee Maker", newProduct.Name);
			Assert.Equal(merchant.Id, newProduct.MerchantId);
		}
	}
}
