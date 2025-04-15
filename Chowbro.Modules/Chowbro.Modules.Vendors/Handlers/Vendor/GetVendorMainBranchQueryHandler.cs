using Chowbro.Core.Models;
using Chowbro.Core.Models.Vendor;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Chowbro.Core.Repository.Interfaces.Vendor;
using Chowbro.Modules.Vendors.Queries;
using Chowbro.Core.Services.Interfaces.Auth;

namespace Chowbro.Modules.Vendors.Handlers.Vendor
{
    public class GetVendorMainBranchQueryHandler : IRequestHandler<GetVendorMainBranchQuery, ApiResponse<BranchDto>>
    {
        private readonly IVendorRepository _vendorRepository;
        private readonly ICurrentUserService _currentUserService;


        public GetVendorMainBranchQueryHandler(IVendorRepository vendorRepository, ICurrentUserService currentUserService)
        {
            _vendorRepository = vendorRepository;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<BranchDto>> Handle(GetVendorMainBranchQuery request, CancellationToken cancellationToken)
        {
            var vendorId = await _currentUserService.GetVendorIdAsync();
            if (!vendorId.HasValue)
                return ApiResponse<BranchDto>.Fail(null, "Vendor not found", HttpStatusCode.NotFound);

            var branch = await _vendorRepository.GetMainBranchByVendorIdAsync(vendorId.Value, include: q => q
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
                AltAddress = branch.AltAddress,
                City = branch.City,
                StateId = branch.StateId ?? Guid.Empty,
                LgaId = branch.LgaId ?? Guid.Empty,
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