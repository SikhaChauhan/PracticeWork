using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ModelLayer.DTO;
using ModelLayer.Model;
using RepositoryLayer.Service;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Service
{
    public class AddressBookRL : IAddressBookRL
    {
        private readonly AddressBookDbContext _context;

        public AddressBookRL(AddressBookDbContext context)
        {
            _context = context;
        }


        //CRUD operations are Added 
        // Fetch all contacts
        public async Task<IEnumerable<AddressBookEntity>> GetAllContactsAsync()
        {
            return await _context.AddressBooks.ToListAsync();
        }

        // Get a contact by ID
        public async Task<AddressBookEntity?> GetContactByIdAsync(int id)
        {
            return await _context.AddressBooks.FindAsync(id);
        }

        // Add a new contact
        public async Task<AddressBookEntity> AddContactAsync(AddressBookEntity contact)
        {
            _context.AddressBooks.Add(contact);
            await _context.SaveChangesAsync();
            return contact;
        }

        // Update an existing contact
        public async Task<AddressBookEntity?> UpdateContactAsync(int id, AddressBookEntity updatedContact)
        {
            var existingContact = await _context.AddressBooks.FindAsync(id);
            if (existingContact == null) return null;

            existingContact.Name = updatedContact.Name;
            existingContact.Email = updatedContact.Email;
            existingContact.PhoneNumber = updatedContact.PhoneNumber;

            await _context.SaveChangesAsync();
            return existingContact;
        }

        // Delete a contact
        public async Task<bool> DeleteContactAsync(int id)
        {
            var contact = await _context.AddressBooks.FindAsync(id);
            if (contact == null) return false;

            _context.AddressBooks.Remove(contact);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}