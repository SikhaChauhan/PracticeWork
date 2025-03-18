using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Interface;
using ModelLayer.Model;

namespace BusinessLayer.Service
{
    public class AddressBookBL : IAddressBookBL
    {
        private readonly IAddressBookRL _addressBookRL;

        public AddressBookBL(IAddressBookRL addressBookRL)
        {
            _addressBookRL = addressBookRL;
        }

        public async Task<IEnumerable<AddressBookEntity>> GetAllContactsAsync()
        {
            return await _addressBookRL.GetAllAsync();
        }

        public async Task<AddressBookEntity?> GetContactByIdAsync(int id)
        {
            return await _addressBookRL.GetByIdAsync(id);
        }

        public async Task<AddressBookEntity> CreateContactAsync(AddressBookEntity contact)
        {
            // Example: Ensure email is unique
            var existingContact = await _addressBookRL.GetByEmailAsync(contact.Email);
            if (existingContact != null)
                throw new Exception("Contact with this email already exists.");

            return await _addressBookRL.AddAsync(contact);
        }

        public async Task<AddressBookEntity?> UpdateContactAsync(int id, AddressBookEntity updatedContact)
        {
            var existingContact = await _addressBookRL.GetByIdAsync(id);
            if (existingContact == null) return null;

            return await _addressBookRL.UpdateAsync(updatedContact);
        }

        public async Task<bool> DeleteContactAsync(int id)
        {
            var contact = await _addressBookRL.GetByIdAsync(id);
            if (contact == null) return false;

            return await _addressBookRL.DeleteAsync(id);
        }
    }
}
