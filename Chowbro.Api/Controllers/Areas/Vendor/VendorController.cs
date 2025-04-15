using Chowbro.Modules.Vendors.Commands.Vendor;
using Chowbro.Modules.Vendors.DTOs.Vendor;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Chowbro.Modules.Vendors.Queries;
using static Chowbro.Core.Enums.Vendor;

namespace Chowbro.Api.Controllers.Areas.Vendor
{
    [Area("Vendors")]
    [Route("api/vendor/vendor")]
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

        [HttpGet("{vendorId}")]
        public async Task<IActionResult> GetVendorById(Guid vendorId)
        {
            var query = new GetVendorByIdQuery(vendorId);
            var result = await _mediator.Send(query);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("by-user")]
        public async Task<IActionResult> GetVendorByUserId()
        {
            var query = new GetVendorByUserIdQuery();
            var result = await _mediator.Send(query);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPut("update-vendor")]
        public async Task<IActionResult> UpdateVendor([FromForm] UpdateVendorRequest request)
        {
            var command = new UpdateVendorCommand(
                request.BusinessName,
                request.FirstName,
                request.LastName,
                request.RcNumber,
                request.Description,
                request.LogoFile,
                request.CoverFile,
                request.Email,
                request.PhoneNumber,
                request.BusinessEmail,
                request.BusinessPhoneNumber,
                request.BusinessTypeId
                );

            var result = await _mediator.Send(command);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPut("update-vendor-status")]
        public async Task<IActionResult> UpdateVendorStatus([FromBody] UpdateVendorStatusCommand command)
        {
            var result = await _mediator.Send(command);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPut("finish-onboarding")]
        public async Task<IActionResult> UpdateVendorStatus()
        {
            var command = new UpdateVendorStatusCommand(VendorStatus.UnderReview);
            var result = await _mediator.Send(command);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("onboarding-status")]
        public async Task<IActionResult> GetOnboardingStatus()
        {
            var result = await _mediator.Send(new GetVendorOnboardingStatusCommand());
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("branches")]
        public async Task<IActionResult> GetVendorBranches(Guid vendorId)
        {
            var query = new GetVendorBranchesQuery(vendorId);
            var result = await _mediator.Send(query);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("main-branch")]
        public async Task<IActionResult> GetVendorMainBranch(Guid vendorId)
        {
            var query = new GetVendorMainBranchQuery(vendorId);
            var result = await _mediator.Send(query);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("branch/create")]
        public async Task<IActionResult> CreateBranch([FromBody] CreateBranchCommand command)
        {
            var result = await _mediator.Send(command);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPut("branch/update")]
        public async Task<IActionResult> UpdateBranch([FromBody] UpdateBranchCommand command)
        {
            var result = await _mediator.Send(command);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpDelete("branch/remove")]
        public async Task<IActionResult> DeleteBranch([FromQuery] Guid id)
        {
            var result = await _mediator.Send(new DeleteBranchCommand(id));
            return StatusCode((int)result.StatusCode, result);
        }
    }
}