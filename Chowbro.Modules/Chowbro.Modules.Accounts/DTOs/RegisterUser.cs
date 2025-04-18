using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Chowbro.Modules.Accounts.DTOs
{
    public class RegisterUser
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, Phone]
        public string PhoneNumber { get; set; }

        public string? ReferralCode { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        public string? State { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Address { get; set; }

        [Required]
        public string? Role { get; set; }

        // Device information
        [JsonIgnore]
        public string? DeviceId { get; set; }
        [JsonIgnore]
        public string? DeviceName { get; set; }
        [JsonIgnore]
        public string? DeviceModel { get; set; }
    }
}