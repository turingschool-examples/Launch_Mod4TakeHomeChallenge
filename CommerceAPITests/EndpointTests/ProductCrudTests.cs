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

namespace CommerceAPITests.EndpointTests
{
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
