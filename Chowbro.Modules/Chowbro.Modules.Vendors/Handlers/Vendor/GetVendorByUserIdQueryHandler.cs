using Chowbro.Core.Entities;
using Chowbro.Core.Interfaces.Auth;
using Chowbro.Core.Interfaces.Vendors;
using Chowbro.Core.Models;
using Chowbro.Core.Models.Vendor;
using Chowbro.Modules.Vendors.Queries.Vendor;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Chowbro.Modules.Vendors.Handlers.Vendor
{
    public class GetVendorByUserIdQueryHandler : IRequestHandler<GetVendorByUserIdQuery, ApiResponse<VendorDto>>
    {
        private readonly IVendorRepository _vendorRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetVendorByUserIdQueryHandler(IVendorRepository vendorRepository, ICurrentUserService currentUserService)
        {
            _vendorRepository = vendorRepository;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<VendorDto>> Handle(GetVendorByUserIdQuery request, CancellationToken cancellationToken)
        {
            var currentUser = _currentUserService.UserId;

            var vendor = await _vendorRepository.GetByUserIdAsync(currentUser, include: q => q
                .Include(v => v.Branches)
                .ThenInclude(b => b.State)
                .Include(v => v.Branches)
                .ThenInclude(b => b.Lga));

            if (vendor == null)
                return ApiResponse<VendorDto>.Fail(null, "Vendor not found", HttpStatusCode.NotFound);

            var vendorDto = new VendorDto
            {
                Id = vendor.Id,
                Name = vendor.Name,
                Description = vendor.Description,
                Status = vendor.Status.ToString(),
                LogoUrl = vendor.LogoUrl,
                CoverImageUrl = vendor.CoverImageUrl,
                CreatedAt = vendor.CreatedAt,
                Branches = vendor.Branches.Select(b => new BranchDto
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
                }).ToList()
            };

            return ApiResponse<VendorDto>.Success(vendorDto, "Vendor retrieved successfully");
        }
    }
}