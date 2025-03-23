using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chowbro.Api.Controllers.Areas.Vendors
{
    [Area("Vendors")]
    [Route("api/vendors")]
    [ApiController]
    [Authorize(Roles = "Vendor,Admin,SuperAdmin")]
    public class VendorController : ControllerBase
    {
        [HttpGet("dashboard")]
        public IActionResult GetDashboard()
        {
            return Ok(new { message = "Welcome to Vendor Dashboard" });
        }
    }
}