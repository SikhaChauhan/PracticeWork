using ModelLayer.Model;

namespace RepositoryLayer.Interface
{
    public interface IAddressBookRL
    {
        Task<IEnumerable<AddressBookEntity>> GetAllAsync();
        Task<AddressBookEntity?> GetByIdAsync(int id);
        Task<AddressBookEntity?> GetByEmailAsync(string email);
        Task<AddressBookEntity> AddAsync(AddressBookEntity contact);
        Task<AddressBookEntity?> UpdateAsync(AddressBookEntity contact);
        Task<bool> DeleteAsync(int id);
    }
}
