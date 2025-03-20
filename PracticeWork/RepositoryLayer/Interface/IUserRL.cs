using System.Threading.Tasks;
using ModelLayer.Model;

namespace RepositoryLayer.Interface
{
    public interface IUserRL
    {
        Task<UserEntity> RegisterUser(UserEntity user);
        Task<UserEntity> GetUserByEmail(string email);
        Task<UserEntity> GetUserByResetToken(string Token);
        Task UpdateUser(UserEntity user);
    }
}
