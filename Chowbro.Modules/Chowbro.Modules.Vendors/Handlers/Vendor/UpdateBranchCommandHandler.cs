using System.Net;
using Chowbro.Core.Models;
using Chowbro.Core.Repository.Interfaces.Vendor;
using Chowbro.Core.Services.Interfaces.Auth;
using MediatR;
using Microsoft.Extensions.Logging;

public class UpdateBranchCommandHandler : IRequestHandler<UpdateBranchCommand, ApiResponse<object?>>
{
    private readonly IVendorRepository _vendorRepository;
    private readonly ICurrentUserService _currentUserService;

    private readonly ILogger<UpdateBranchCommandHandler> _logger;

    public UpdateBranchCommandHandler(
        IVendorRepository vendorRepository,
        ICurrentUserService currentUserService,
        ILogger<UpdateBranchCommandHandler> logger)
    {
        _vendorRepository = vendorRepository;
        _logger = logger;
        _currentUserService = currentUserService;
    }
    public async Task<ApiResponse<object?>> Handle(UpdateBranchCommand request, CancellationToken cancellationToken)
    {
        var branch = await _vendorRepository.GetBranchByIdAsync(request.Id, cancellationToken: cancellationToken);
        if (branch == null)
            return ApiResponse<object?>.Fail(
            null, "Branch not found", HttpStatusCode.NotFound);

        var vendorId = await _currentUserService.GetVendorIdAsync();
        if (!vendorId.HasValue)
        {
            return ApiResponse<object?>.Fail(
            null, "Vendor not found", HttpStatusCode.NotFound);
        }

        branch.Name = request.Name;
        branch.Address = request.Address;

        branch.StateId = request.StateId;
        branch.LgaId = request.LgaId;
        branch.City = request.City;
        branch.Country = request.Country ?? "Nigeria";
        branch.PostalCode = request.PostalCode;
        branch.Latitude = request.Latitude;
        branch.Longitude = request.Longitude;
        branch.PhoneNumber = request.PhoneNumber;
        branch.IsMainBranch = request.IsMainBranch ?? false;

        await _vendorRepository.UpdateBranchAsync(branch, cancellationToken);
        _logger.LogInformation("Branch {BranchId} updated", branch.Id);

        return ApiResponse<object?>.Success(null, "Branch updated successfully", HttpStatusCode.OK);
    }
}