using Microsoft.EntityFrameworkCore;
using ModelLayer.Model;
using RepositoryLayer.Interface;
using RepositoryLayer.Service;

namespace RepositoryLayer.Repository
{
    public class AddressBookRL : IAddressBookRL
    {
        private readonly AddressBookDBContext _context;

        public AddressBookRL(AddressBookDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AddressBookEntity>> GetAllAsync()
        {
            return await _context.AddressBooks.ToListAsync();
        }

        public async Task<AddressBookEntity?> GetByIdAsync(int id)
        {
            return await _context.AddressBooks.FindAsync(id);
        }

        public async Task<AddressBookEntity?> GetByEmailAsync(string email)
        {
            return await _context.AddressBooks.FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<AddressBookEntity> AddAsync(AddressBookEntity contact)
        {
            _context.AddressBooks.Add(contact);
            await _context.SaveChangesAsync();
            return contact;
        }

        public async Task<AddressBookEntity?> UpdateAsync(AddressBookEntity contact)
        {
            _context.Entry(contact).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return contact;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var contact = await GetByIdAsync(id);
            if (contact == null) return false;

            _context.AddressBooks.Remove(contact);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
