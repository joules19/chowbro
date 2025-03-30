using Chowbro.Core.Entities;
using Chowbro.Core.Interfaces.Notifications;
using Chowbro.Core.Models;
using Chowbro.Infrastructure.Auth;
using Chowbro.Infrastructure.Helpers;
using Chowbro.Infrastructure.Services;
using Chowbro.Modules.Accounts.DTOs;
using Chowbro.Modules.Accounts.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Chowbro.Modules.Accounts.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IOtpService _otpService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IOtpService otpService, IEmailService emailService, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _otpService = otpService;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<ApiResponse<OtpResponse>> RegisterAsync(RegisterUser model)
        {
            var existingUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == model.Email || u.PhoneNumber == model.PhoneNumber);

            if (existingUser != null)
            {
                if (existingUser.EmailConfirmed)
                    return ApiResponse<OtpResponse>.Fail(null, "Email already in use. Please login.", HttpStatusCode.Conflict);

                // Resend OTP instead of creating a new user
                var newOtp = OtpHelper.GenerateOtp();
                existingUser.OtpCode = newOtp;
                existingUser.OtpExpires = DateTime.UtcNow.AddMinutes(10);
                await _userManager.UpdateAsync(existingUser);

                // await _otpService.SendOtpAsync(existingUser.Email!, newOtp, true);
                return ApiResponse<OtpResponse>.Success(new OtpResponse
                {
                    RequiresOtp = true,
                    Message = "An OTP has been resent to your email."
                }, statusCode: HttpStatusCode.OK);
            }

            // Generate a password using the email
            string generatedPassword = model.Email;

            // Default to "Customer" if no role is provided
            var userRole = string.IsNullOrEmpty(model.Role) ? Roles.Customer : model.Role;

            // Proceed with normal registration if user does not exist
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                FirstName = model.FirstName,
                LastName = model.LastName,
                RCNumber = model.RCNumber,
                ReferralCode = model.ReferralCode,
                DateOfBirth = model.DateOfBirth,
                State = model.State,
                City = model.City,
                Country = model.Country,
                Address = model.Address
            };

            // Hash the generated password before saving
            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, generatedPassword);

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
                return ApiResponse<OtpResponse>.Fail(null, "User registration failed.", HttpStatusCode.InternalServerError, result.Errors.Select(e => e.Description).ToList());

            // Assign Role to User
            if (await _roleManager.RoleExistsAsync(userRole))
            {
                await _userManager.AddToRoleAsync(user, userRole);
            }

            // Generate OTP and send
            var otp = OtpHelper.GenerateOtp();
            user.OtpCode = otp;
            user.OtpExpires = DateTime.UtcNow.AddMinutes(10);
            await _userManager.UpdateAsync(user);

            // await _otpService.SendOtpAsync(user.Email, otp, true);

            return ApiResponse<OtpResponse>.Success(new OtpResponse
            {
                RequiresOtp = true,
                Message = "An OTP has been sent to your email."
            }, statusCode: HttpStatusCode.Created);
        }

        public async Task<ApiResponse<bool>> VerifyRegistrationOtpAsync(string email, string otp)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return ApiResponse<bool>.Fail(false, "User not found.", HttpStatusCode.NotFound);

            if (user.OtpCode == null || user.OtpExpires < DateTime.UtcNow || user.OtpCode != otp)
                return ApiResponse<bool>.Fail(false, "Invalid or expired OTP.", statusCode: HttpStatusCode.BadRequest);

            // OTP is valid, activate the user and clear OTP fields
            user.EmailConfirmed = true;
            user.OtpCode = null;
            user.OtpExpires = null;

            await _userManager.UpdateAsync(user);

            return ApiResponse<bool>.Success(true, "OTP verified successfully, registration completed.", statusCode: HttpStatusCode.OK);
        }
       
        public async Task<ApiResponse<OtpResponse>> LoginAsync(string contactInfo)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == contactInfo || u.PhoneNumber == contactInfo);

            if (user == null)
                return ApiResponse<OtpResponse>.Fail(null, "User not found.", HttpStatusCode.NotFound);

            if (!user.EmailConfirmed)
            {
                // Resend OTP if user is not verified
                var newOtp = OtpHelper.GenerateOtp();
                user.OtpCode = newOtp;
                user.OtpExpires = DateTime.UtcNow.AddMinutes(10);
                await _userManager.UpdateAsync(user);

                await _otpService.SendOtpAsync(user.Email!, newOtp, true);

                return ApiResponse<OtpResponse>.Fail(new OtpResponse
                {
                    Message = "Email not verified. A new OTP has been sent to your email.",
                    RequiresOtp = true
                }, "Email not verified", HttpStatusCode.Forbidden);
            }

            var otpResponse = await SendOtpToUserAsync(contactInfo);
            return ApiResponse<OtpResponse>.Success(new OtpResponse
            {
                Message = otpResponse.Data,
                RequiresOtp = true
            }, statusCode: otpResponse.StatusCode);
        }
        
        public async Task<ApiResponse<string>> SendOtpToUserAsync(string contactInfo, bool isEmail = false)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == contactInfo || u.PhoneNumber == contactInfo);
            if (user == null)
                return ApiResponse<string>.Fail(null, "User not found.", HttpStatusCode.NotFound);

            // Generate OTP
            var otp = OtpHelper.GenerateOtp();
            user.OtpCode = otp;
            user.OtpExpires = DateTime.UtcNow.AddMinutes(10);
            await _userManager.UpdateAsync(user);

            // Send OTP
            // await _otpService.SendOtpAsync(contactInfo, otp, isEmail);

            return ApiResponse<string>.Success("OTP sent successfully.", statusCode: HttpStatusCode.OK);
        }
       
        public async Task<ApiResponse<AuthResponse>> VerifyOtpAsync(string contactInfo, string otp)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == contactInfo || u.PhoneNumber == contactInfo);

            if (user == null)
                return ApiResponse<AuthResponse>.Fail(null, "User not found.", HttpStatusCode.NotFound);

            if (string.IsNullOrEmpty(user.OtpCode) || user.OtpExpires == null || user.OtpCode != otp || user.OtpExpires < DateTime.UtcNow)
                return ApiResponse<AuthResponse>.Fail(null, "Invalid or expired OTP.", statusCode: HttpStatusCode.BadRequest);

            // Reset OTP fields and confirm email
            user.OtpCode = null;
            user.OtpExpires = null;
            // user.EmailConfirmed = true;

            await _userManager.UpdateAsync(user);

            // Generate JWT and refresh token
            var tokenResponse = await GenerateJwtToken(user);

            return ApiResponse<AuthResponse>.Success(tokenResponse.Data, statusCode: HttpStatusCode.OK);
        }
        
        public async Task<ApiResponse<bool>> RevokeRefreshTokenAsync(string refreshToken)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
            if (user == null)
                return ApiResponse<bool>.Fail(false, "Invalid refresh token.", statusCode: HttpStatusCode.NotFound);

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _userManager.UpdateAsync(user);

            return ApiResponse<bool>.Success(true, "Refresh token revoked successfully.", statusCode: HttpStatusCode.OK);
        }
        
        public async Task<ApiResponse<AuthResponse>> RefreshTokenAsync(string refreshToken)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
                return ApiResponse<AuthResponse>.Fail(null, "Invalid or expired refresh token.", statusCode: HttpStatusCode.Unauthorized);

            return await GenerateJwtToken(user);
        }
        
         public async Task<ApiResponse<AuthResponse>> GenerateJwtToken(ApplicationUser user)
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

            // Generate and store refresh token
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // Set refresh token expiry

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
