using CommerceAPI.Models;

namespace CommerceAPITests.ModelTests
{
    public class ProductTests
    {
        [Fact]
        public void IsCreatedWithNoConstructor()
        {
            var product = new Product { Name = "Slim Jims", Description = "Meat Stick", Category = "Snack", PriceInCents = 99, StockQuantity = 100, ReleaseDate = new DateTime(2000,1, 1, 0, 0, 0).ToUniversalTime()};

            Assert.Equal("Slim Jims", product.Name);
            Assert.Equal("Meat Stick", product.Description);
            Assert.Equal("Snack", product.Category);
            Assert.Equal(99, product.PriceInCents);
            Assert.Equal(100, product.StockQuantity);
            Assert.Equal(new DateTime(2000, 1, 1, 0, 0, 0).ToUniversalTime(), product.ReleaseDate);
        }
    }
}