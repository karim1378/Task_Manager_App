using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TaskManagerApp.Application.DTOs;
using TaskManagerApp.Application.Interfaces;
using TaskManagerApp.Domain.Entities;
using TaskManagerApp.Application.Exceptions;
using TaskManagerApp.Infrastructure.Services;
using Task = System.Threading.Tasks.Task;
using System.Collections.Generic;
using System.Linq;
using Org.BouncyCastle.Asn1.X509;

namespace TaskManagerApp.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailService _emailService;

        public UserService(IMapper mapper, IUserRepository userRepository, ITokenService tokenService, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IEmailService emailService)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _tokenService = tokenService;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
        }

        public async Task<LoginDto> RegisterAsync(UserRegisterDto userRegisterDto)
        {
            try
            {
                var user = _mapper.Map<User>(userRegisterDto);
                var result = await _userRepository.CreateUserAsync(user, userRegisterDto.Password);
                if (!result.Succeeded)
                {
                    throw new ServiceException("Registration failed.");
                }

                return new LoginDto { UserName = userRegisterDto.UserName, Password = userRegisterDto.Password };
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("An error occurred while registering the user", ex);
            }
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var result = await _userRepository.PasswordSignInAsync(loginDto.UserName, loginDto.Password);

                if (!result.Succeeded)
                {
                    throw new ServiceException("Login failed.");
                }

                var user = await _userRepository.FindByNameAsync(loginDto.UserName);
                var accessToken = await _tokenService.GenerateAccessTokenAsync(user);
                var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user);
                

                return new LoginResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                };
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("An error occurred while logging in the user", ex);
            }
        }
        public async Task<string> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                var user = await _userRepository.FindByNameAsync(forgotPasswordDto.UserName);
                if (user == null || user.Email != forgotPasswordDto.Email)
                {
                    throw new ServiceException("Invalid username or email");
                }

                var token = await _userRepository.GeneratePasswordResetTokenAsync(user);
                var resetLink = $"{_configuration["AppUrl"]}/reset-password?token={token}&email={user.Email}";

                await _emailService.SendEmailAsync(user.Email, "Password Reset", $"Please reset your password by clicking <a href='{resetLink}'>here</a>.");
                return "An email send to your email";
            }
            catch(RepositoryException ex)
            {
                throw new ServiceException("error in forgot password section", ex);
            }
        }



        public async Task<string> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            try {

                var user = await _userRepository.FindByEmailAsync(resetPasswordDto.Email);
                if (user == null)
                {
                    throw new ServiceException("Invalid token or email.");
                }

                var result = await _userRepository.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
                if (result.Succeeded)
                {
                    return "password reset is successfully";
                }
                else
                {       
                    throw new ServiceException("Password reset failed.");
                }
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("error in reset password section", ex);
            }
        }


        public async Task<string> RefreshTokenAsync(string refreshTokenValue)
        {
            try
            {
                var newAccessToken = await _tokenService.GetRefreshTokenAsync(refreshTokenValue);
                return newAccessToken;
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("error in refresh token section",ex);
            }
        }

        public async Task<GetProfileDto> UpdateProfileAsync(UpdateProfileDto updateProfileDto)
        {
            try
            {

                var user = await CurrentUser();
                if (user == null)
                {
                    throw new ServiceException("User not found.");
                }
                Action<string, Action<string>> updateIfNotNullOrWhiteSpace = (value, updateAction) =>
                {
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        updateAction(value);
                    }
                };
                updateIfNotNullOrWhiteSpace(updateProfileDto.FirstName, firstName => user.FirstName = firstName);
                updateIfNotNullOrWhiteSpace(updateProfileDto.LastName, lastName => user.LastName = lastName);
                updateIfNotNullOrWhiteSpace(updateProfileDto.UserName, userName => user.UserName = userName);
                updateIfNotNullOrWhiteSpace(updateProfileDto.Email, email => user.Email = email);
                var result = await _userRepository.UpdateUserAsync(user);
                if (!result.Succeeded)
                {
                    throw new ServiceException("Profile update failed.");
                }

                if (!string.IsNullOrEmpty(updateProfileDto.Password))
                {
                    var token = await _userRepository.GeneratePasswordResetTokenAsync(user);
                    var passwordResult = await _userRepository.ResetPasswordAsync(user, token, updateProfileDto.Password);
                    if (!passwordResult.Succeeded)
                    {
                        throw new ServiceException("Password update failed.");
                    }
                }
                return _mapper.Map<GetProfileDto>(user);
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("An error occurred while updating the profile", ex);
            }
        }

        public async Task<GetProfileDto> GetProfileAsync()
        {
            try
            {
                var user = await CurrentUser();
                if (user == null)
                {
                    throw new ServiceException("User not found.");
                }

                return _mapper.Map<GetProfileDto>(user);
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("An error occurred while retrieving the profile", ex);
            }
        }

        public async Task<IEnumerable<GetUserDto>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userRepository.GetAllUsersAsync();
                return _mapper.Map<IEnumerable<GetUserDto>>(users);

            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("An error occurred while retrieving users", ex);
            }
        }

        public async Task<IEnumerable<GetUserDto>> SearchUserAsync(string userName)
        {
            try
            {
                var user = await _userRepository.SearchUsersAsync(userName);
                return _mapper.Map<IEnumerable<GetUserDto>>(user);

            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("User not found", ex);
            }
        }

        public async Task DeleteAccountAsync(string password)
        {
            try
            {
                var user = await CurrentUser();
                var authentication = await _userRepository.PasswordSignInAsync(user.UserName, password);
                if (!authentication.Succeeded)
                {
                    throw new ServiceException("user not found or password is wrong for delete account");
                }
                var result = await _userRepository.DeleteUserAsync(user);
                if (!result.Succeeded)
                {
                    throw new ServiceException("Delete account failed");
                }
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("An error occurred while deleting the account.", ex);
            }
        }

        private async Task<User> CurrentUser()
        {
            var userName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userRepository.FindByNameAsync(userName);
            if (user == null)
            {
                throw new ServiceException("please login");
            }
            return user;
        }
    }
}

