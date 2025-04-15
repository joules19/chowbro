using Chowbro.Modules.Vendors.Commands.Vendor;
using Chowbro.Modules.Vendors.Queries.Vendor;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chowbro.Api.Controllers.Vendor
{
    [Route("api/vendor/store-operation")]
    [Area("Vendors")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Vendor")]
    public class StoreOperationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StoreOperationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetOperations()
        {
            var result = await _mediator.Send(new GetStoreOperationQuery());
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPut("update-store-operation")]
        public async Task<IActionResult> UpdateOperations([FromBody] UpdateStoreOperationCommand command)
        {
            var result = await _mediator.Send(command);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}