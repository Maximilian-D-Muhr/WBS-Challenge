using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

using ChallengeTracker.Api.Application.Interfaces;
using ChallengeTracker.Api.Dtos.Auth;
using ChallengeTracker.Api.Models;

namespace ChallengeTracker.Api.Application.Services;

public class AuthService : IAuthService
{
  private readonly UserManager<User> _userManager;
  private readonly IConfiguration _configuration;

  public AuthService(UserManager<User> userManager, IConfiguration configuration)
  {
    _userManager = userManager;
    _configuration = configuration;
  }

  public async Task<(bool Success, IEnumerable<string> Errors)> RegisterAsync(RegisterRequestDto request)
  {
    var user = new User
    {
      UserName = request.Email,
      Email = request.Email,
      DisplayName = request.DisplayName
    };

    var result = await _userManager.CreateAsync(user, request.Password);
    var errors = result.Errors.Select(e => e.Description);
    return (result.Succeeded, errors);
  }

  public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto request)
  {
    var user = await _userManager.FindByEmailAsync(request.Email);
    if (user is null) return null;

    var valid = await _userManager.CheckPasswordAsync(user, request.Password);
    if (!valid) return null;

    var expires = DateTime.UtcNow.AddMinutes(
        int.Parse(_configuration["Jwt:ExpiryMinutes"]!));

    var claims = new[]
    {
      new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
      new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
      new Claim("displayName", user.DisplayName)
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: _configuration["Jwt:Issuer"],
        audience: _configuration["Jwt:Audience"],
        claims: claims,
        expires: expires,
        signingCredentials: creds);

    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
    return new AuthResponseDto(tokenString, expires);
  }

  public async Task<MeResponseDto?> GetCurrentUserAsync(Guid userId)
  {
    var user = await _userManager.FindByIdAsync(userId.ToString());
    if (user is null) return null;

    return new MeResponseDto(user.Id, user.Email ?? string.Empty, user.DisplayName);
  }
}
