using CommerceAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommerceAPITests.ModelTests
{
	public class ProductTests
	{
		[Fact]
		public void Product_IsCreatedWithNoConstructor()
		{
			Product product = new() { Name = "Shirt", Category = "Clothing", Description = "A pretty cool shirt", PriceInCents = 2499, ReleaseDate = DateTime.Now, StockQuantity = 5 };

			Assert.Equal("Shirt", product.Name);
			Assert.Equal(2499, product.PriceInCents);
		}

		[Fact]
		public void Product_PriceInDollars_ReturnsConvertedPrice()
		{
			Product product = new() { Name = "Shirt", Category = "Clothing", Description = "A pretty cool shirt", PriceInCents = 2499, ReleaseDate = DateTime.Now, StockQuantity = 5 };

			Assert.Equal(24.99, product.PriceInDollars());
		}
	}
}
