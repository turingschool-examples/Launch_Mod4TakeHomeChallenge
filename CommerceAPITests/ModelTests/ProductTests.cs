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
        public void IsCreatedWithNoConstructor()
        {
            var product = new Product { Name = "Burger", Category = "Food", Description = "Tasty", Price = 12.99M, StockQuantity = 4 };

            Assert.Equal("Burger", product.Name);
            Assert.Equal("Food", product.Category);
            Assert.Equal("Tasty", product.Description);
            Assert.Equal(12.99M, product.Price);
            Assert.Equal(4, product.StockQuantity);

        }
    }
}
