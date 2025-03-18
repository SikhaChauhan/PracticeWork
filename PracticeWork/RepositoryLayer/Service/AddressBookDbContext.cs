using Microsoft.EntityFrameworkCore; 
using ModelLayer.Model;

namespace RepositoryLayer.Service
{
    public class AddressBookDbContext : DbContext 
    {
        public AddressBookDbContext(DbContextOptions<AddressBookDbContext> options) : base(options)
        {
        }

        public DbSet<AddressBookEntity> AddressBooks { get; set; }
    }
}
