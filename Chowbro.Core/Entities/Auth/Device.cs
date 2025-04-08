namespace Chowbro.Core.Entities.Auth;

    public class Device : BaseEntity
    {
        public string DeviceId { get; set; } // Unique device identifier
        public string? DeviceName { get; set; }
        public string? DeviceType { get; set; } // e.g., "iOS", "Android", "Web"
        public string? OperatingSystem { get; set; }
        public string? OsVersion { get; set; }
        public string? Manufacturer { get; set; }
        public string? Model { get; set; }
        public DateTime FirstSeen { get; set; } = DateTime.UtcNow;
        public DateTime LastSeen { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public bool IsBlacklisted { get; set; } = false;
        
        public bool NeedsReview { get; set; } = false;
        public string? ReviewReason { get; set; }

        
        
        // Relationships
        public string? UserId { get; set; } // Nullable for unauthenticated devices
        public ApplicationUser? User { get; set; }
        
        // Location info (if needed)
        public string? IpAddress { get; set; }
        public string? CountryCode { get; set; }
        public string? Region { get; set; }
}