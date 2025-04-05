using Chowbro.Core.Entities;
using Chowbro.Core.Models;
using Chowbro.Core.Models.Vendor;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Chowbro.Core.Interfaces.Vendor;
using Chowbro.Modules.Vendors.Queries;

namespace Chowbro.Modules.Vendors.Handlers.Vendor
{
    public class GetVendorByIdQueryHandler : IRequestHandler<GetVendorByIdQuery, ApiResponse<VendorDto>>
    {
        private readonly IVendorRepository _vendorRepository;

        public GetVendorByIdQueryHandler(IVendorRepository vendorRepository)
        {
            _vendorRepository = vendorRepository;
        }

        public async Task<ApiResponse<VendorDto>> Handle(GetVendorByIdQuery request, CancellationToken cancellationToken)
        {
            var vendor = await _vendorRepository.GetVendorByIdAsync(request.VendorId, include: q => q
                .Include(v => v.Branches)
                .ThenInclude(b => b.State)
                .Include(v => v.Branches)
                .ThenInclude(b => b.Lga));

            if (vendor == null)
                return ApiResponse<VendorDto>.Fail(null, "Vendor not found", HttpStatusCode.NotFound);

            var vendorDto = new VendorDto
            {
                Id = vendor.Id,
                BusinessName = vendor.BusinessName,
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