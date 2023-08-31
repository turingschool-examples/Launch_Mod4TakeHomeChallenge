using CommerceAPI.Models;

namespace CommerceAPITests.ModelTests
{
    public class ProductTests
    {
        [Fact]
        public void IsCreatedWithNoConstructor()
        {
            var product = new Product { Name = "Burger", Description = "Tasty", Category = "Food", Price = 15, StockQuantity = 20, ReleaseDate = DateTime.Now };

            Assert.Equal("Burger", product.Name);
            Assert.Equal("Food", product.Category);
            Assert.Equal("Tasty", product.Description);
            Assert.Equal(15, product.Price);
            Assert.Equal(20, product.StockQuantity);
        }
    }
}