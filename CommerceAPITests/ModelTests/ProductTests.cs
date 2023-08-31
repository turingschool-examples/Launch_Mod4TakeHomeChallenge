using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommerceAPI.Models;

namespace CommerceAPITests.ModelTests
{
    public class ProductTests
    {
        [Fact]
        public void IsCreatedWithNoConstructor()
        {
            var product = new Product { Name = "Hamburger", Category = "Food", PriceInCents = 1000, StockQuantity = 10 };

            Assert.Equal("Hamburger", product.Name);
            Assert.Equal("Food", product.Category);
            Assert.Equal(1000, product.PriceInCents);
            Assert.Equal(10, product.StockQuantity);
        }
    }
}
