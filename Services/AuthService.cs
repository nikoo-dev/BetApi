using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BetliveApi.Data;
using BetliveApi.DTOs.Auth;
using BetliveApi.Models;
using BetliveApi.Repositories.Interfaces;
using BetliveApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BetliveApi.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration  _config;
    private readonly AppDbContext    _context;

    public AuthService(IUserRepository userRepository, IConfiguration config, AppDbContext context)
    {
        _userRepository = userRepository;
        _config         = config;
        _context        = context;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
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

        return await BuildAuthResponseAsync(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email)
            ?? throw new UnauthorizedAccessException("Invalid email or password.");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        return await BuildAuthResponseAsync(user);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var token = await _context.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == refreshToken)
            ?? throw new UnauthorizedAccessException("Invalid refresh token.");

        if (token.IsRevoked)
            throw new UnauthorizedAccessException("Refresh token has been revoked.");

        if (token.ExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Refresh token has expired.");

        // Rotate: revoke old token and issue a new one
        token.IsRevoked = true;
        _context.RefreshTokens.Update(token);
        await _context.SaveChangesAsync();

        return await BuildAuthResponseAsync(token.User);
    }

    public async Task RevokeTokenAsync(string refreshToken)
    {
        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(r => r.Token == refreshToken)
            ?? throw new KeyNotFoundException("Refresh token not found.");

        token.IsRevoked = true;
        _context.RefreshTokens.Update(token);
        await _context.SaveChangesAsync();
    }

    private async Task<AuthResponseDto> BuildAuthResponseAsync(User user)
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

        var jwtToken = new JwtSecurityToken(
            issuer:             jwtSettings["Issuer"],
            audience:           jwtSettings["Audience"],
            claims:             claims,
            expires:            expiry,
            signingCredentials: creds
        );

        // Generate and store a refresh token
        var refreshToken = new RefreshToken
        {
            UserId    = user.Id,
            Token     = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            Token        = new JwtSecurityTokenHandler().WriteToken(jwtToken),
            RefreshToken = refreshToken.Token,
            Username     = user.Username,
            Email        = user.Email,
            Role         = user.Role,
            ExpiresAt    = expiry
        };
    }
}
