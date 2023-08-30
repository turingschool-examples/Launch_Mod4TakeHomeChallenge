namespace CommerceAPI.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int PriceInCents { get; set; }
        public int StockQuantity { get; set; }
        public DateTime ReleaseDate { get; set; }
        public Merchant Merchant { get; set; }
    }
}
