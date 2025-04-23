using ChefMeet.DTOs;

namespace ChefMeet.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto dto, string? immagineProfiloPath);

        Task<string?> LoginAsync(LoginDto dto);
    }
}
