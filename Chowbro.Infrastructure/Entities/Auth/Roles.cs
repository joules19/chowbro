namespace Chowbro.Infrastructure.Auth
{
    public static class Roles
    {
        public const string SuperAdmin = "SuperAdmin";
        public const string Admin = "Admin";
        public const string Vendor = "Vendor";
        public const string Customer = "Customer";
        public const string Rider = "Rider";
        public const string Guest = "Guest";

        public static List<string> AllRoles => [SuperAdmin, Admin, Vendor, Customer, Rider, Guest];
    }
}