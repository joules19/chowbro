using Chowbro.Api.Controllers;
using Chowbro.Modules.Vendors.Commands.Product;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chowbro.Api.Controllers.Areas.Vendor
{
    [Area("Vendors")]
    [Route("api/vendor/products")]
    [Authorize(Roles = "Vendor")]
    public class ProductController : BaseController
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromForm] AddProductCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // Other actions for update, delete, etc.
    }
}