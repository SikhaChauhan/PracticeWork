using ModelLayer.DTO;

namespace BusinessLayer.Interface
{
    public interface IAuthBL
    {
        Task<string?> RegisterUserAsync(UserRegisterDTO userDto);
        Task<string?> LoginUserAsync(UserLoginDTO loginDto);
        Task<string> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(string resetToken, string newPassword);
    }
}
