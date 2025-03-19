using BusinessLayer.Interface;
using ModelLayer.DTO;
using ModelLayer.Model;
using RepositoryLayer.Interface;

namespace BusinessLayer.Service
{
    public class AddressBookBL : IAddressBookBL
    {
        private readonly IAddressBookRL _addressBookRL;

        public AddressBookBL(IAddressBookRL addressBookRL)
        {
            _addressBookRL = addressBookRL;
        }

        // Fetch all contacts from the AddressBook
        public async Task<IEnumerable<AddressBookDTO>> GetAllContactsAsync()
        {
            var contacts = await _addressBookRL.GetAllAsync();

            return contacts.Select(contact => new AddressBookDTO
            {
                Id = contact.Id,
                Name = contact.Name,
                Email = contact.Email,
                PhoneNumber = contact.PhoneNumber
            });
        }

        // Fetch a contact by Id
        public async Task<AddressBookDTO?> GetContactByIdAsync(int id)
        {
            var contact = await _addressBookRL.GetByIdAsync(id);

            if (contact == null) return null;

            return new AddressBookDTO
            {
                Id = contact.Id,
                Name = contact.Name,
                Email = contact.Email,
                PhoneNumber = contact.PhoneNumber
            };
        }

        // Create a new contact
        public async Task<AddressBookDTO> CreateContactAsync(AddressBookDTO contactDto)
        {
            var contactEntity = new AddressBookEntity
            {
                Name = contactDto.Name,
                Email = contactDto.Email,
                PhoneNumber = contactDto.PhoneNumber
            };

            var createdContact = await _addressBookRL.AddAsync(contactEntity);

            return new AddressBookDTO
            {
                Id = createdContact.Id,
                Name = createdContact.Name,
                Email = createdContact.Email,
                PhoneNumber = createdContact.PhoneNumber
            };
        }

        // Update an existing contact
        public async Task<AddressBookDTO?> UpdateContactAsync(int id, AddressBookDTO contactDto)
        {
            var existingContact = await _addressBookRL.GetByIdAsync(id);

            if (existingContact == null) return null;

            // Manually update fields
            existingContact.Name = contactDto.Name;
            existingContact.Email = contactDto.Email;
            existingContact.PhoneNumber = contactDto.PhoneNumber;

            var updatedContact = await _addressBookRL.UpdateAsync(existingContact);

            return new AddressBookDTO
            {
                Id = updatedContact.Id,
                Name = updatedContact.Name,
                Email = updatedContact.Email,
                PhoneNumber = updatedContact.PhoneNumber
            };
        }

        // Delete a contact
        public async Task<bool> DeleteContactAsync(int id)
        {
            return await _addressBookRL.DeleteAsync(id);
        }
    }
}
