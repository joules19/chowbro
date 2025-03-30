using Chowbro.Core.Entities;
using Chowbro.Core.Models;
using Chowbro.Modules.Accounts.Commands.Auth;
using Chowbro.Modules.Accounts.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Chowbro.Modules.Accounts.Commands.Handlers
{
    public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, ApiResponse<AuthResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public VerifyOtpCommandHandler(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<ApiResponse<AuthResponse>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == request.ContactInfo || u.PhoneNumber == request.ContactInfo);

            if (user == null)
                return ApiResponse<AuthResponse>.Fail(null, "User not found.", HttpStatusCode.NotFound);

            if (string.IsNullOrEmpty(user.OtpCode) || user.OtpExpires == null || user.OtpCode != request.Otp || user.OtpExpires < DateTime.UtcNow)
                return ApiResponse<AuthResponse>.Fail(null, "Invalid or expired OTP.", statusCode: HttpStatusCode.BadRequest);

            user.OtpCode = null;
            user.OtpExpires = null;
            await _userManager.UpdateAsync(user);

            return await GenerateJwtToken(user);
        }

        private async Task<ApiResponse<AuthResponse>> GenerateJwtToken(ApplicationUser user)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]!);
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.PhoneNumber!),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, string.Join(",", roles))
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            var response = new AuthResponse
            {
                AccessToken = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken,
                Expiration = token.ValidTo,
                User = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email!,
                    PhoneNumber = user.PhoneNumber!,
                    Roles = roles.ToList()
                }
            };

            return ApiResponse<AuthResponse>.Success(response, statusCode: HttpStatusCode.OK);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}