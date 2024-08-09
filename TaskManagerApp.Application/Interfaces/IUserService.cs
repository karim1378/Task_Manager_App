using TaskManagerApp.Application.DTOs;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TaskManagerApp.Application.Interfaces
{
    public interface IUserService
    {
        Task<LoginDto> RegisterAsync(UserRegisterDto userRegisterDto);
        Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
        Task<string> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<string> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<string> RefreshTokenAsync(string token);
        Task<GetProfileDto> UpdateProfileAsync(UpdateProfileDto updateProfileDto);
        Task<GetProfileDto> GetProfileAsync();
        Task<IEnumerable<GetUserDto>> GetAllUsersAsync();
        Task<IEnumerable<GetUserDto>> SearchUserAsync(string userName);
        Task DeleteAccountAsync(string password);
    }
}
