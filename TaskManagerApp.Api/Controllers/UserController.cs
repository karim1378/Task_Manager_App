using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TaskManagerApp.Application.DTOs;
using TaskManagerApp.Application.Interfaces;
using TaskManagerApp.Application.Exceptions;
using TaskManagerApp.Api.Filters;
using Org.BouncyCastle.Asn1.Ocsp;
using TaskManagerApp.Domain.Entities;

/// <summary>
/// Controller for managing user-related operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="userRegisterDto">The user registration data.</param>
    /// <returns>HTTP status code.</returns>
    [HttpPost("register")]
    [ValidateModel]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegisterDto)
    {
        try
        {
            var loginDto = await _userService.RegisterAsync(userRegisterDto);
            var response = new
            {
                Message = "registration is successfully",
                LoginInformation = loginDto
            };
            return new ObjectResult(response)
            {
                StatusCode = StatusCodes.Status201Created
            };
        }
        catch (ServiceException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Service Exception : " + ex.Message + "  ,  " + "Repository Exception : " + ex.InnerException);
        }
    }

    ///<summary>
    /// Logs in a user.
    /// </summary>
    /// <param name="loginDto">The user login data.</param>
    ///<returns>The access and refresh tokens.</returns>
    [HttpPost("login")]
    [ValidateModel]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var tokens = await _userService.LoginAsync(loginDto);
            return Ok(tokens); // access token and refresh token
        }
        catch (ServiceException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Service Exception : " + ex.Message + "  ,  " + "Repository Exception : " + ex.InnerException);
        }
    }

    /// <summary>
    /// Sends a password reset link to the user's email.
    /// </summary>
    /// <param name="forgotPasswordDto">The forgot password data.</param>
    /// <returns>HTTP status code.</returns>
    [HttpPost("forgot-password")]
    [ValidateModel]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
    {
        try
        {
           var message =  await _userService.ForgotPasswordAsync(forgotPasswordDto);
            return Ok(message);
        }
        catch (ServiceException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Service Exception : " + ex.Message + "  ,  " + "Repository Exception : " + ex.InnerException);
        }
    }

    /// <summary>
    /// Resets the user's password.
    /// </summary>
    /// <param name="resetPasswordDto">The reset password data.</param>
    /// <returns>HTTP status code.</returns>
    [HttpPost("reset-password")]
    [ValidateModel]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        try
        {
            var message = await _userService.ResetPasswordAsync(resetPasswordDto);
            return Ok(message);
        }
        catch (ServiceException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Service Exception : " + ex.Message + "  ,  " + "Repository Exception : " + ex.InnerException);
        }
    }


    /// <summary>
    /// Refreshes the access token using the refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token.</param>
    /// <returns>The new access token.</returns>
    [HttpGet("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromQuery] string refreshToken)
    {
        try
        {
            var newAccessToken = await _userService.RefreshTokenAsync(refreshToken);
            return Ok(newAccessToken);
        }
        catch (ServiceException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Service Exception : " + ex.Message + "  ,  " + "Repository Exception : " + ex.InnerException);
        }
    }
    
    /// <summary>
    /// Gets the profile of the current user.
    /// </summary>
    /// <returns>The user profile.</returns>
    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var userProfile = await _userService.GetProfileAsync();
            return Ok(userProfile);
        }
        catch (ServiceException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Service Exception : " + ex.Message + "  ,  " + "Repository Exception : " + ex.InnerException);
        }
    }

    /// <summary>
    /// Gets all users.
    /// </summary>
    /// <returns>List of all users.</returns>
    [Authorize]
    [HttpGet("all-users")]
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            if (users == null || !users.Any())
            {
                return NotFound();
            }
            return Ok(users);
        }
        catch (ServiceException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Service Exception : " + ex.Message + "  ,  " + "Repository Exception : " + ex.InnerException);
        }
    }

    /// <summary>
    /// Searches for a user by username.
    /// </summary>
    /// <param name="userName">The username to search for.</param>
    /// <returns>The user details.</returns>
    [Authorize]
    [HttpGet("search")]
    public async Task<IActionResult> SearchUser([FromQuery] string userName)
    {
        try
        {
            var user = await _userService.SearchUserAsync(userName);
            if (user == null || !user.Any())
            {
                return NotFound();
            }
            return Ok(user);
        }
        catch (ServiceException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Service Exception : " + ex.Message + "  ,  " + "Repository Exception : " + ex.InnerException);
        }
    }

    /// <summary>
    /// Updates the profile of the current user.
    /// </summary>
    /// <param name="updateDto">The update profile data.</param>
    /// <returns>The updated profile.</returns>
    [Authorize]
    [HttpPut("profile")]
    [ValidateModel]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto updateDto)
    {
        try
        {
            var updatedProfile = await _userService.UpdateProfileAsync(updateDto);
            return Ok(updatedProfile);
        }
        catch (ServiceException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Service Exception : " + ex.Message + "  ,  " + "Repository Exception : " + ex.InnerException);
        }
    }

    /// <summary>
    /// Deletes the account of the current user.
    /// </summary>
    /// <param name="password">The password of the user.</param>
    /// <returns>HTTP status code.</returns>
    [Authorize]
    [HttpDelete("delete-account")]
    public async Task<IActionResult> DeleteAccount([FromBody] string password)
    {
        try
        {
            await _userService.DeleteAccountAsync(password);
            return NoContent();
        }
        catch (ServiceException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Service Exception : " + ex.Message + "  ,  " + "Repository Exception : " + ex.InnerException);
        }
    }
}
