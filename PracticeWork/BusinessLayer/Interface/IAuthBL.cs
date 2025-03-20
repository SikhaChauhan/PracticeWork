using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.DTO;
using ModelLayer.Model;

namespace BussinessLayer.Interface
{
    public interface IAuthBL
    {
        Task<UserEntity> Register(UserRegisterDTO userDTO);
        Task<string> Login(UserLoginDTO userDTO);
        Task<bool> ForgotPassword(ForgotPasswordDTO forgotPasswordDTO);
        Task<bool> ResetPassword(ResetPasswordDTO resetPasswordDTO);
    }
}