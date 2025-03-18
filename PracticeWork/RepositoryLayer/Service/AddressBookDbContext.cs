using Microsoft.EntityFrameworkCore; 
using ModelLayer.Model;

namespace RepositoryLayer.Service
{
    public class AddressBookDBContext : DbContext 
    {
        public AddressBookDBContext(DbContextOptions<AddressBookDBContext> options) : base(options)
        {
        }

        public DbSet<AddressBookEntity> AddressBooks { get; set; }
    }
}
