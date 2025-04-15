using System.Net;
using Chowbro.Core.Models;
using Chowbro.Core.Repository.Interfaces.Vendor;
using Chowbro.Core.Services.Interfaces.Auth;
using Chowbro.Infrastructure.Persistence.Repository.Vendor;
using MediatR;
using Microsoft.Extensions.Logging;

public class DeleteBranchCommandHandler : IRequestHandler<DeleteBranchCommand, ApiResponse<object?>>
{
    private readonly IVendorRepository _vendorRepository;
    private readonly ICurrentUserService _currentUserService;

    private readonly ILogger<DeleteBranchCommandHandler> _logger;

    public DeleteBranchCommandHandler(
        IVendorRepository vendorRepository,
        ICurrentUserService currentUserService,
        ILogger<DeleteBranchCommandHandler> logger)
    {
        _vendorRepository = vendorRepository;
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<object?>> Handle(DeleteBranchCommand request, CancellationToken cancellationToken)
    {
        var branch = await _vendorRepository.GetBranchByIdAsync(request.Id, cancellationToken: cancellationToken);
        if (branch == null)
            return ApiResponse<object?>.Fail(
           null, "Branch not found.", HttpStatusCode.NotFound);

        await _vendorRepository.DeleteBranchAsync(branch, cancellationToken);
        _logger.LogInformation("Branch {BranchId} deleted", branch.Id);

        return ApiResponse<object?>.Success(null, "Branch deleted successfully", HttpStatusCode.OK);
    }
}