using ModelLayer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Interface
{
    public interface IAddressBookBL
    {
        Task<IEnumerable<AddressBookEntity>> GetAllContactsAsync();
        Task<AddressBookEntity?> GetContactByIdAsync(int id);
        Task<AddressBookEntity> AddContactAsync(AddressBookEntity contact);
        Task<AddressBookEntity?> UpdateContactAsync(int id, AddressBookEntity updatedContact);
        Task<bool> DeleteContactAsync(int id);
    }
}
