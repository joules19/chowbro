using Chowbro.Core.Models;
using Chowbro.Core.Models.Vendor;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Chowbro.Core.Repository.Interfaces.Vendor;
using Chowbro.Modules.Vendors.Queries;

namespace Chowbro.Modules.Vendors.Handlers.Vendor
{
    public class GetVendorMainBranchQueryHandler : IRequestHandler<GetVendorMainBranchQuery, ApiResponse<BranchDto>>
    {
        private readonly IVendorRepository _vendorRepository;

        public GetVendorMainBranchQueryHandler(IVendorRepository vendorRepository)
        {
            _vendorRepository = vendorRepository;
        }

        public async Task<ApiResponse<BranchDto>> Handle(GetVendorMainBranchQuery request, CancellationToken cancellationToken)
        {
            var branch = await _vendorRepository.GetMainBranchByVendorIdAsync(request.VendorId, include: q => q
                .Include(b => b.State)
                .Include(b => b.Lga));

            if (branch == null)
                return ApiResponse<BranchDto>.Fail(null, "Main branch not found", HttpStatusCode.NotFound);

            var branchDto = new BranchDto
            {
                Id = branch.Id,
                Name = branch.Name,
                PhoneNumber = branch.PhoneNumber,
                Address = branch.Address,
                City = branch.City,
                StateId = branch.StateId,
                LgaId = branch.LgaId,
                Country = branch.Country,
                PostalCode = branch.PostalCode,
                Latitude = branch.Latitude,
                Longitude = branch.Longitude,
                IsMainBranch = branch.IsMainBranch
            };

            return ApiResponse<BranchDto>.Success(branchDto, "Main branch retrieved successfully");
        }
    }
}