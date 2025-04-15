using Chowbro.Modules.Vendors.Commands.BusinessType;
using Chowbro.Modules.Vendors.DTOs.BusinessType;
using Chowbro.Modules.Vendors.Queries.BusinessType;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chowbro.Api.Controllers.Areas.Vendor
{
    [Area("Vendors")]
    [Route("api/vendor/business-types")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Vendor")]
    public class BusinessTypeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BusinessTypeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
        {
            var query = new GetAllBusinessTypesQuery(includeInactive);
            var result = await _mediator.Send(query);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetBusinessTypeByIdQuery(id);
            var result = await _mediator.Send(query);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBusinessTypeRequest request)
        {
            var command = new CreateBusinessTypeCommand(request.Name, request.Description, request.IsActive);
            var result = await _mediator.Send(command);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBusinessTypeRequest request)
        {
            var command = new UpdateBusinessTypeCommand(id, request.Name, request.Description, request.IsActive);
            var result = await _mediator.Send(command);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteBusinessTypeCommand(id);
            var result = await _mediator.Send(command);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}