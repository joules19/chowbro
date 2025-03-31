using Chowbro.Modules.Vendors.Commands.Vendor;
using Chowbro.Modules.Vendors.DTOs.Vendor;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chowbro.Api.Controllers.Areas.Vendors
{
    [Area("Vendors")]
    [Route("api/vendors/vendor")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Vendor")]
    public class VendorController : ControllerBase
    {
        private readonly IMediator _mediator;

        public VendorController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("complete-onboarding")]
        public async Task<IActionResult> CompleteOnboarding([FromForm] CompleteVendorOnboardingRequest request)
        {
            var command = new CompleteVendorOnboardingCommand(request);
            var result = await _mediator.Send(command);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}