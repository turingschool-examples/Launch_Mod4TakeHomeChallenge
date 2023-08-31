using CommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CommerceAPI.DataAccess
{
    public class CommerceApiContext : DbContext
    {
        public DbSet<Merchant> Merchants { get; set; }
        public DbSet<Product> Products { get; set; }
        public CommerceApiContext(DbContextOptions<CommerceApiContext> options) : base(options)
        {
            
        }
    }
}
