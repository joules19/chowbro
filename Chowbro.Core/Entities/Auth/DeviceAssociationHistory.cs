using System;
using System.ComponentModel.DataAnnotations;

namespace Chowbro.Core.Entities.Auth
{
    public class DeviceAssociationHistory
    {
        [Key]
        public Guid Id { get; set; }
        public string DeviceId { get; set; }
        public string UserId { get; set; }
        public DateTime AssociatedAt { get; set; }
        public DateTime? DisassociatedAt { get; set; }
        public string? AssociationType { get; set; } // "Register", "Login", "Refresh", etc.
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }
}