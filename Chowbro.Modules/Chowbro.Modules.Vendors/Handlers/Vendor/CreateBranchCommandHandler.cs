using System.Net;
using Chowbro.Core.Models;
using Chowbro.Core.Repository.Interfaces.Vendor;
using Chowbro.Core.Services.Interfaces.Auth;
using MediatR;
using Microsoft.Extensions.Logging;
using static Chowbro.Core.Enums.Vendor;

public class CreateBranchCommandHandler : IRequestHandler<CreateBranchCommand, ApiResponse<Guid?>>
{
    private readonly IVendorRepository _vendorRepository;
    private readonly ICurrentUserService _currentUserService;

    private readonly ILogger<CreateBranchCommandHandler> _logger;

    public CreateBranchCommandHandler(
        IVendorRepository vendorRepository,
        ICurrentUserService currentUserService,
        ILogger<CreateBranchCommandHandler> logger)
    {
        _vendorRepository = vendorRepository;
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<Guid?>> Handle(CreateBranchCommand request, CancellationToken cancellationToken)
    {
        var vendorId = await _currentUserService.GetVendorIdAsync();
        if (!vendorId.HasValue)
        {
            return ApiResponse<Guid?>.Fail(
            null, "Vendor not found", HttpStatusCode.NotFound);
        }

        var vendorRecord = await _vendorRepository.GetByIdAsync(vendorId.Value, cancellationToken);
        if (vendorRecord == null)
        {
            return ApiResponse<Guid?>.Fail(
            null, "Vendor not found", HttpStatusCode.NotFound);
        }

        var existingBranches = await _vendorRepository.GetBranchesByVendorIdAsync(vendorId.Value);

        if (existingBranches.Count > 0 && vendorRecord.Status == VendorStatus.PendingApproval)
        {
            // Assuming they are in onboarding and they want to update the initially created record
            var mainBranch = existingBranches.FirstOrDefault(b => b.IsMainBranch);

            if (mainBranch != null)
            {
                mainBranch.Address = request.Address;
                mainBranch.AltAddress = request.AltAddress;
                mainBranch.StateId = request.StateId;
                mainBranch.LgaId = request.LgaId;
                mainBranch.City = request.City;
                mainBranch.Country = request.Country ?? "Nigeria";
                mainBranch.PostalCode = request.PostalCode;
                mainBranch.Latitude = request.Latitude;
                mainBranch.Longitude = request.Longitude;
                mainBranch.PhoneNumber = request.PhoneNumber;

                await _vendorRepository.UpdateBranchAsync(mainBranch, cancellationToken);
                _logger.LogInformation("Main branch {BranchId} updated for Vendor {VendorId}", mainBranch.Id, vendorId.Value);

                return ApiResponse<Guid?>.Success(mainBranch.Id, "Main branch updated", HttpStatusCode.OK);
            }
        }


        if (existingBranches.Count > 0 && (vendorRecord.Status == VendorStatus.UnderReview || vendorRecord.Status == VendorStatus.Approved))
        {
            // Assuming they are no longer in onboarding and they want to update the initially created record
            var branchRecord = existingBranches.FirstOrDefault(b => b.Id == request.Id);
            if (branchRecord == null)
            {
                return ApiResponse<Guid?>.Fail(
                null, "branchRecord not found", HttpStatusCode.NotFound);
            }

            var mainBranch = existingBranches.FirstOrDefault(b => b.IsMainBranch);
            if (mainBranch != null)
            {
                return ApiResponse<Guid?>.Fail(
                null, "A main branch already exists", HttpStatusCode.Conflict);
            }

            branchRecord.Address = request.Address;
            branchRecord.AltAddress = request.AltAddress;
            branchRecord.StateId = request.StateId;
            branchRecord.LgaId = request.LgaId;
            branchRecord.City = request.City;
            branchRecord.Country = request.Country ?? "Nigeria";
            branchRecord.PostalCode = request.PostalCode;
            branchRecord.Latitude = request.Latitude;
            branchRecord.Longitude = request.Longitude;
            if (mainBranch != null)
            {
                mainBranch.IsMainBranch = false;
            }
            branchRecord.IsMainBranch = request.IsMainBranch ?? false;

            mainBranch.IsMainBranch = false;

            await _vendorRepository.UpdateBranchAsync(branchRecord, cancellationToken);

            _logger.LogInformation("branchRecord {branchRecordId} updated for Vendor {VendorId}", branchRecord.Id, vendorId.Value);
        }


        // Create a new branch if no main branch exists
        var branch = new Branch
        {
            VendorId = vendorId.Value,
            Name = request.Name,
            Address = request.Address,
            AltAddress = request.AltAddress,
            StateId = request.StateId,
            LgaId = request.LgaId,
            City = request.City,
            Country = request.Country ?? "Nigeria",
            PostalCode = request.PostalCode,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            PhoneNumber = request.PhoneNumber,
            IsMainBranch = !existingBranches.Any()
        };

        await _vendorRepository.AddBranchAsync(branch, cancellationToken);
        _logger.LogInformation("Branch {BranchId} created for Vendor {VendorId}", branch.Id, vendorId.Value);

        return ApiResponse<Guid?>.Success(branch.Id, "Branch retrieved", HttpStatusCode.OK);
    }
}