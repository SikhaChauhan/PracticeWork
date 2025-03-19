using ModelLayer.DTO;


namespace BusinessLayer.Interface
{
    public interface IAddressBookBL
    {
        Task<IEnumerable<AddressBookDTO>> GetAllContactsAsync();
        Task<AddressBookDTO?> GetContactByIdAsync(int id);
        Task<AddressBookDTO> CreateContactAsync(AddressBookDTO contactDto);
        Task<AddressBookDTO?> UpdateContactAsync(int id, AddressBookDTO contactDto);
        Task<bool> DeleteContactAsync(int id);
    }
}
