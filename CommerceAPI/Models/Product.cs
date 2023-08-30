namespace CommerceAPI.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Price { get; set; }
        public int StockQuanity { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int MerchantId { get; set; }
    }
}
