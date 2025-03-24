using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DTO;
using ModelLayer.Model;

namespace PracticeWork.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthBL _userService;

        public AuthController(IAuthBL userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// This method is used to Register the User
        /// </summary>
        /// <param name="userregisterDTO"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<ActionResult<UserEntity>> Register([FromBody] UserRegisterDTO userregisterDTO)
        {
            var registeredUser = await _userService.Register(userregisterDTO);
            return CreatedAtAction(nameof(Register), new { id = registeredUser.Id }, registeredUser);
        }

        /// <summary>
        /// This method is used to Login a User
        /// </summary>
        /// <param name="userloginDTO"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] UserLoginDTO userloginDTO)
        {
            var token = await _userService.Login(userloginDTO);
            if (token == null)
            {
                return Unauthorized("Invalid credentials");
            }

            return Ok(new { Token = token });
        }

        /// <summary>
        /// This method is used to Trigger Forget Password
        /// </summary>
        /// <param name="forgotPasswordDTO"></param>
        /// <returns></returns>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO forgotPasswordDTO)
        {
            var result = await _userService.ForgotPassword(forgotPasswordDTO);
            if (!result) return NotFound("User not found.");

            return Ok("Password reset link has been sent to your email.");
        }

        /// <summary>
        /// This method is used to Reset the user's password
        /// </summary>
        /// <param name="resetPasswordDTO"></param>
        /// <returns></returns>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            var result = await _userService.ResetPassword(resetPasswordDTO);
            if (!result) return BadRequest("Invalid or expired reset token.");

            return Ok("Password has been reset successfully.");
        }
    }
}