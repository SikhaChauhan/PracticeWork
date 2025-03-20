using ModelLayer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepositoryLayer.Interface
{
    public interface IAddressBookRL
    {
        Task<IEnumerable<AddressBookEntity>> GetAllContactsAsync();
        Task<AddressBookEntity?> GetContactByIdAsync(int id);
        Task<AddressBookEntity> AddContactAsync(AddressBookEntity contact);
        Task<AddressBookEntity?> UpdateContactAsync(int id, AddressBookEntity updatedContact);
        Task<bool> DeleteContactAsync(int id);
    }
}
