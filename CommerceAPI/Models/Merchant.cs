namespace CommerceAPI.Models
{
    public class Merchant
    {
        public int Id { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
        public string Name { get; set; }
        public string Category { get; set; }
    }
}
