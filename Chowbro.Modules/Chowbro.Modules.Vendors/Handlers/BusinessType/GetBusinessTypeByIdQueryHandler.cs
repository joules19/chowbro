using Chowbro.Core.Models;
using Chowbro.Modules.Vendors.DTOs.BusinessType;
using Chowbro.Modules.Vendors.Queries.BusinessType;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using Chowbro.Core.Repository.Interfaces.Vendor;

namespace Chowbro.Modules.Vendors.Handlers.BusinessType
{
    public class GetBusinessTypeByIdQueryHandler : IRequestHandler<GetBusinessTypeByIdQuery, ApiResponse<BusinessTypeDto>>
    {
        private readonly IBusinessTypeRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetBusinessTypeByIdQueryHandler> _logger;

        public GetBusinessTypeByIdQueryHandler(
            IBusinessTypeRepository repository,
            IMapper mapper,
            ILogger<GetBusinessTypeByIdQueryHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponse<BusinessTypeDto>> Handle(GetBusinessTypeByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var businessType = await _repository.GetByIdAsync(request.Id, cancellationToken);
                if (businessType == null)
                {
                    return ApiResponse<BusinessTypeDto>.Fail(
                        null,
                        "Business type not found",
                        HttpStatusCode.NotFound);
                }

                var result = _mapper.Map<BusinessTypeDto>(businessType);
                return ApiResponse<BusinessTypeDto>.Success(result, "Business type retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving business type with ID: {BusinessTypeId}", request.Id);
                return ApiResponse<BusinessTypeDto>.Fail(
                    null,
                    "Error retrieving business type",
                    HttpStatusCode.InternalServerError);
            }
        }
    }
}