using Microsoft.AspNetCore.Identity;
using TaskManagerApp.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaskManagerApp.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<IdentityResult> CreateUserAsync(User user, string password);
        Task<User> FindByIdAsync(string id);
        Task<User> FindByNameAsync(string userName);
        Task<User> FindByEmailAsync(string email);
        Task<IdentityResult> UpdateUserAsync(User user);
        Task<IdentityResult> ResetPasswordAsync(User user, string token, string newPassword);
        Task<string> GeneratePasswordResetTokenAsync(User user);
        Task<SignInResult> PasswordSignInAsync(string userName, string password);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<IEnumerable<User>> SearchUsersAsync(string userName);
        Task<IdentityResult> DeleteUserAsync(User user);
    }
}
