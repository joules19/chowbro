// using Chowbro.Core.Entities;
// using Chowbro.Core.Entities.Location;
// using Chowbro.Core.Interfaces.Auth;
// using Chowbro.Core.Interfaces.Location;
// using Chowbro.Core.Interfaces.Media;
// using Chowbro.Core.Interfaces.Vendors;
// using Chowbro.Core.Models.Vendor;
// using Chowbro.Core.Models;
// using Chowbro.Modules.Vendors.Commands.Vendor;
// using MediatR;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.Extensions.Logging;
// using static Chowbro.Core.Enums.Vendor;
// using System.Net;
// using Chowbro.Core.Entities.Vendor;
//
// public class CompleteVendorOnboardingCommandHandler : IRequestHandler<CompleteVendorOnboardingCommand, ApiResponse<VendorDto>>
// {
//     private readonly ICurrentUserService _currentUserService;
//     private readonly IVendorRepository _vendorRepository;
//     private readonly IStateRepository _stateRepository;
//     private readonly ILgaRepository _lgaRepository;
//     private readonly UserManager<ApplicationUser> _userManager;
//     private readonly ICloudinaryService _cloudinaryService;
//     private readonly ILogger<CompleteVendorOnboardingCommandHandler> _logger;
//
//     public CompleteVendorOnboardingCommandHandler(
//         ICurrentUserService currentUserService,
//         IVendorRepository vendorRepository,
//         IStateRepository stateRepository,
//         ILgaRepository lgaRepository,
//         UserManager<ApplicationUser> userManager,
//         ICloudinaryService cloudinaryService,
//         ILogger<CompleteVendorOnboardingCommandHandler> logger)
//     {
//         _currentUserService = currentUserService;
//         _vendorRepository = vendorRepository;
//         _stateRepository = stateRepository;
//         _lgaRepository = lgaRepository;
//         _userManager = userManager;
//         _cloudinaryService = cloudinaryService;
//         _logger = logger;
//     }
//
//     public async Task<ApiResponse<VendorDto>> Handle(CompleteVendorOnboardingCommand request, CancellationToken cancellationToken)
//     {
//         try
//         {
//             var userId = _currentUserService.UserId;
//             if (string.IsNullOrEmpty(userId))
//                 return ApiResponse<VendorDto>.Fail(null, "User not authenticated", HttpStatusCode.Unauthorized);
//
//             var user = await _userManager.FindByIdAsync(userId);
//             if (user == null)
//                 return ApiResponse<VendorDto>.Fail(null, "User not found", HttpStatusCode.NotFound);
//
//             if (!_currentUserService.IsVendor)
//                 return ApiResponse<VendorDto>.Fail(null, "User is not a vendor", HttpStatusCode.Forbidden);
//
//             // Validate State exists
//             var stateExists = await _stateRepository.GetByIdAsync(request.Model.MainBranch.StateId);
//             if (stateExists == null)
//                 return ApiResponse<VendorDto>.Fail(null, "Invalid State specified", HttpStatusCode.BadRequest);
//
//             // Validate LGA exists and belongs to the specified State
//             var lga = await _lgaRepository.GetByIdAsync(request.Model.MainBranch.LgaId);
//             if (lga == null || lga.StateId != request.Model.MainBranch.StateId)
//                 return ApiResponse<VendorDto>.Fail(null, "Invalid LGA specified or LGA doesn't belong to the selected State", HttpStatusCode.BadRequest);
//
//             // Upload logo if provided
//             string logoUrl = null;
//             if (request.Model.Logo != null)
//             {
//                 var logoUploadResult = await _cloudinaryService.UploadImageAsync(request.Model.Logo);
//                 if (logoUploadResult.Error != null)
//                 {
//                     _logger.LogError("Failed to upload logo: {Error}", logoUploadResult.Error.Message);
//                     return ApiResponse<VendorDto>.Fail(null, "Failed to upload logo", HttpStatusCode.BadRequest);
//                 }
//                 logoUrl = logoUploadResult.SecureUrl.ToString();
//             }
//
//             // Upload cover image if provided
//             string coverImageUrl = null;
//             if (request.Model.CoverImage != null)
//             {
//                 var coverUploadResult = await _cloudinaryService.UploadImageAsync(request.Model.CoverImage);
//                 if (coverUploadResult.Error != null)
//                 {
//                     // Clean up logo if cover image fails
//                     if (!string.IsNullOrEmpty(logoUrl))
//                         await _cloudinaryService.DeleteImageAsync(logoUrl);
//
//                     _logger.LogError("Failed to upload cover image: {Error}", coverUploadResult.Error.Message);
//                     return ApiResponse<VendorDto>.Fail(null, "Failed to upload cover image", HttpStatusCode.BadRequest);
//                 }
//                 coverImageUrl = coverUploadResult.SecureUrl.ToString();
//             }
//
//             var vendor = new Vendor
//             {
//                 Name = request.Model.Name,
//                 RcNumber = request.Model.RcNumber,
//                 Description = request.Model.Description,
//                 PhoneNumber = user.PhoneNumber,
//                 Email = user.Email,
//                 LogoUrl = logoUrl,
//                 CoverImageUrl = coverImageUrl,
//                 Status = VendorStatus.PendingApproval,
//                 UserId = userId
//             };
//
//             var mainBranch = new Branch
//             {
//                 Name = request.Model.MainBranch.Name,
//                 Address = request.Model.MainBranch.Address,
//                 City = request.Model.MainBranch.City,
//                 StateId = request.Model.MainBranch.StateId,
//                 LgaId = request.Model.MainBranch.LgaId,
//                 Country = request.Model.MainBranch.Country,
//                 PostalCode = request.Model.MainBranch.PostalCode,
//                 Latitude = request.Model.MainBranch.Latitude,
//                 Longitude = request.Model.MainBranch.Longitude,
//                 PhoneNumber = request.Model.MainBranch.PhoneNumber,
//                 IsMainBranch = true,
//                 Vendor = vendor
//             };
//
//             vendor.Branches.Add(mainBranch);
//
//             await _vendorRepository.AddAsync(vendor);
//             await _vendorRepository.SaveChangesAsync();
//
//             var vendorDto = new VendorDto
//             {
//                 Id = vendor.Id,
//                 Name = vendor.Name,
//                 LogoUrl = vendor.LogoUrl,
//                 CoverImageUrl = vendor.CoverImageUrl,
//                 Status = vendor.Status.ToString()
//             };
//
//             return ApiResponse<VendorDto>.Success(vendorDto, "Vendor onboarding completed", HttpStatusCode.Created);
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Error completing vendor onboarding");
//             return ApiResponse<VendorDto>.Fail(null, "An error occurred during onboarding", HttpStatusCode.InternalServerError);
//         }
//     }
// }