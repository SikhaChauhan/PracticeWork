using ModelLayer.DTO;

namespace BusinessLayer.Interface
{
    public interface IAuthBL
    {
        Task<string?> RegisterUserAsync(UserRegisterDTO userDto);
        Task<string?> LoginUserAsync(UserLoginDTO loginDto);
    }
}
