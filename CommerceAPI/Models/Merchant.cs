﻿namespace CommerceAPI.Models
{
    public class Merchant
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<Product> Products { get; set; } = new List<Product>();
    }
}
