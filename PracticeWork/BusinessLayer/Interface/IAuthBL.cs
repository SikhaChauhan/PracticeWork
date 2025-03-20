using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.DTO;
using ModelLayer.Model;

namespace BusinessLayer.Interface
{
    public interface IAuthBL
    {
        Task<UserEntity> Register(UserRegisterDTO userregisterDTO);
        Task<string> Login(UserLoginDTO userloginDTO);

        Task<bool> ForgotPassword(ForgotPasswordDTO forgotPasswordDTO);
        Task<bool> ResetPassword(ResetPasswordDTO resetPasswordDTO);
    }
}