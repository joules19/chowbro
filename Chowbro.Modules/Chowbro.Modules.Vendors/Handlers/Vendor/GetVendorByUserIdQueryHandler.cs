using Chowbro.Core.Entities;
using Chowbro.Core.Models;
using Chowbro.Core.Models.Vendor;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Chowbro.Core.Repository.Interfaces.Vendor;
using Chowbro.Core.Services.Interfaces.Auth;
using Chowbro.Modules.Vendors.Queries;

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
                .ThenInclude(b => b.Lga)
                .Include(v => v.BusinessType));

            if (vendor == null)
                return ApiResponse<VendorDto>.Fail(null, "Vendor not found", HttpStatusCode.NotFound);

            var vendorDto = new VendorDto
            {
                Id = vendor.Id,
                BusinessName = vendor.BusinessName,
                FirstName = vendor.FirstName,
                LastName = vendor.LastName,
                RcNumber = vendor.RcNumber,
                Description = vendor.Description,
                LogoUrl = vendor.LogoUrl,
                CoverImageUrl = vendor.CoverImageUrl,
                CoverPublicId = vendor.CoverPublicId,
                LogoPublicId = vendor.LogoPublicId,
                PhoneNumber = vendor.PhoneNumber,
                Email = vendor.Email,
                BusinessPhoneNumber = vendor.BusinessPhoneNumber,
                BusinessEmail = vendor.BusinessEmail,
                UserId = vendor.UserId,
                BusinessTypeId = vendor.BusinessTypeId,
                BusinessTypeName = vendor.BusinessType?.Name,
                Status = vendor.Status.ToString(),
                CreatedAt = vendor.CreatedAt,
                Branches = vendor.Branches.Select(b => new BranchDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    PhoneNumber = b.PhoneNumber,
                    Address = b.Address,
                    City = b.City,
                    StateId = b.StateId ?? Guid.Empty,
                    LgaId = b.LgaId ?? Guid.Empty,
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