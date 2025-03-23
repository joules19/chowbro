namespace Chowbro.Modules.Accounts.DTOs
{
    public class UserDto
    {
        public string Id { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public List<string> Roles { get; set; } = new();
    }
}