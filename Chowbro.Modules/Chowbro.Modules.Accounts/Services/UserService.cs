using Chowbro.Modules.Accounts.Services.Interfaces;

namespace Chowbro.Modules.Accounts.Services
{
    public class UserService : IUserService
    {
        public string GetUserInfo() => "User Info from Accounts Module";
    }
}