using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Chowbro.Core.Services.Interfaces.Auth;

namespace Chowbro.Api.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
   

        // Inject dependencies through constructor
        public BaseController() { }

        protected string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);
        protected string UserEmail => User.FindFirstValue(ClaimTypes.Email);
        protected string UserPhone => User.FindFirstValue(ClaimTypes.MobilePhone);
        protected string UserRole => User.FindFirstValue(ClaimTypes.Role);
        protected string UserName => User.FindFirstValue(ClaimTypes.Name);

        /// <summary>
        /// Checks if the logged-in user has a specific role.
        /// </summary>
        protected bool IsInRole(string role) => User.IsInRole(role);

        /// <summary>
        /// Gets all roles assigned to the user.
        /// </summary>
        protected List<string> GetUserRoles()
        {
            return User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();
        }
    }
}