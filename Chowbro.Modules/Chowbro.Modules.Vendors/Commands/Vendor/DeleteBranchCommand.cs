using Chowbro.Core.Models;
using CloudinaryDotNet;
using MediatR;

public record DeleteBranchCommand(Guid Id) : IRequest<ApiResponse<object?>>;