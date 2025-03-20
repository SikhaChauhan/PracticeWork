using Microsoft.EntityFrameworkCore;
using ModelLayer.DTO;
using ModelLayer.Model;

namespace RepositoryLayer.Service

{
    public class AddressBookDbContext : DbContext
    {
        public AddressBookDbContext(DbContextOptions<AddressBookDbContext> options) : base(options)
        {

        }

        public DbSet<UserEntity> Users { get; set; }

        public DbSet<AddressBookEntity> AddressBooks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}