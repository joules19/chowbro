using Chowbro.Core.Entities;
using Chowbro.Core.Models;
using Chowbro.Core.Models.Vendor;
using Chowbro.Core.Repository.Interfaces.Vendor;
using Chowbro.Modules.Vendors.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chowbro.Modules.Vendors.Handlers.Vendor
{
    public class GetVendorBranchesQueryHandler : IRequestHandler<GetVendorBranchesQuery, ApiResponse<List<BranchDto>>>
    {
        private readonly IVendorRepository _vendorRepository;

        public GetVendorBranchesQueryHandler(IVendorRepository vendorRepository)
        {
            _vendorRepository = vendorRepository;
        }

        public async Task<ApiResponse<List<BranchDto>>> Handle(GetVendorBranchesQuery request, CancellationToken cancellationToken)
        {
            var branches = await _vendorRepository.GetBranchesByVendorIdAsync(request.VendorId, include: q => q
                .Include(b => b.State)
                .Include(b => b.Lga));

            var branchDtos = branches.Select(b => new BranchDto
            {
                Id = b.Id,
                Name = b.Name,
                PhoneNumber = b.PhoneNumber,
                Address = b.Address,
                City = b.City,
                StateId = b.StateId,
                LgaId = b.LgaId,
                Country = b.Country,
                PostalCode = b.PostalCode,
                Latitude = b.Latitude,
                Longitude = b.Longitude,
                IsMainBranch = b.IsMainBranch
            }).ToList();

            return ApiResponse<List<BranchDto>>.Success(branchDtos, "Branches retrieved successfully");
        }
    }
}