using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using chatbot.Core.DTOs.Auth;
using chatbot.Core.Models;

namespace chatbot.Core.Interfaces.Services
{
    public interface IAuthService
    {
        // Register
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto,string? ipAddress);


        // Login
        Task<AuthResponseDto> LoginAsync(LoginDto dto,string? ipAddress);


        // Refresh
        Task<AuthResponseDto> RefreshAsync(string token,string? ipAddress);


        // Logout current device
        Task LogoutAsync(string token,string? ipAddress);


        // Logout all devices
        Task LogoutAllAsync(Guid userId,string? ipAddress);
    }
}
