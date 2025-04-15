using Chowbro.Core.Models;
using MediatR;

public record UpdateBranchCommand(
    Guid Id,
    string? Name,
    string? Address,
    string? AltAddress,
    Guid? StateId,
    Guid? LgaId,
    string? City,
    string? Country,
    string? PostalCode,
    double? Latitude,
    double? Longitude,
    string? PhoneNumber,
    bool? IsMainBranch) : IRequest<ApiResponse<object?>>;