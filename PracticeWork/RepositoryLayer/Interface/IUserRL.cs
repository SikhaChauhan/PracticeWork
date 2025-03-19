using ModelLayer.Model;

namespace RepositoryLayer.Interface
{
    public interface IUserRL
    {
        Task<UserEntity?> GetUserByEmailAsync(string email);
        Task<UserEntity> RegisterUserAsync(UserEntity user);
        Task<UserEntity?> LoginUserAsync(string email, string password);

    }
}
