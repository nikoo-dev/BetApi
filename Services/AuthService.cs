using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BetliveApi.DTOs.Auth;
using BetliveApi.Models;
using BetliveApi.Repositories.Interfaces;
using BetliveApi.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace BetliveApi.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _config;

    public AuthService(IUserRepository userRepository, IConfiguration config)
    {
        _userRepository = userRepository;
        _config         = config;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        // Check if email or username already taken
        if (await _userRepository.GetByEmailAsync(dto.Email) is not null)
            throw new ArgumentException("Email is already registered.");

        if (await _userRepository.GetByUsernameAsync(dto.Username) is not null)
            throw new ArgumentException("Username is already taken.");

        var user = new User
        {
            Username     = dto.Username,
            Email        = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role         = "Player"
        };

        await _userRepository.CreateAsync(user);

        return BuildAuthResponse(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email)
            ?? throw new UnauthorizedAccessException("Invalid email or password.");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        return BuildAuthResponse(user);
    }

    private AuthResponseDto BuildAuthResponse(User user)
    {
        var jwtSettings = _config.GetSection("JwtSettings");
        var key         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
        var creds       = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry      = DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["ExpiryMinutes"]!));

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email,          user.Email),
            new Claim(ClaimTypes.Name,           user.Username),
            new Claim(ClaimTypes.Role,           user.Role)
        };

        var token = new JwtSecurityToken(
            issuer:             jwtSettings["Issuer"],
            audience:           jwtSettings["Audience"],
            claims:             claims,
            expires:            expiry,
            signingCredentials: creds
        );

        return new AuthResponseDto
        {
            Token     = new JwtSecurityTokenHandler().WriteToken(token),
            Username  = user.Username,
            Email     = user.Email,
            Role      = user.Role,
            ExpiresAt = expiry
        };
    }
}
