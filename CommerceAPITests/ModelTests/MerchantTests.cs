using CommerceAPI.Models;

namespace CommerceAPITests.ModelTests
{
    public class MerchantTests
    {
        [Fact]
        public void IsCreatedWithNoConstructor()
        {
            var merchant = new Merchant { Name = "Biker Jim's", Category = "Restaurant"};

            Assert.Equal("Biker Jim's", merchant.Name);
            Assert.Equal("Restaurant", merchant.Category);
        }
    }
}