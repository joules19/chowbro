using AutoMapper;
using Chowbro.Modules.Vendors.DTOs.BusinessType;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;
using Chowbro.Core.Models;
using Chowbro.Core.Repository.Interfaces.Vendor;
using Chowbro.Modules.Vendors.Queries.BusinessType;

namespace Chowbro.Modules.Vendors.Handlers.BusinessType
{
    public class GetAllBusinessTypesQueryHandler : IRequestHandler<GetAllBusinessTypesQuery, ApiResponse<IEnumerable<BusinessTypeDto>>>
    {
        private readonly IBusinessTypeRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllBusinessTypesQueryHandler> _logger;

        public GetAllBusinessTypesQueryHandler(
            IBusinessTypeRepository repository,
            IMapper mapper,
            ILogger<GetAllBusinessTypesQueryHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<BusinessTypeDto>>> Handle(GetAllBusinessTypesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                IEnumerable<Core.Entities.Vendor.BusinessType> businessTypes;
                
                if (request.IncludeInactive)
                {
                    businessTypes = await _repository.GetAllAsync(cancellationToken);
                }
                else
                {
                    businessTypes = await _repository.GetActiveAsync(cancellationToken);
                }

                var result = _mapper.Map<IEnumerable<BusinessTypeDto>>(businessTypes);
                return ApiResponse<IEnumerable<BusinessTypeDto>>.Success(
                    result,
                    "Business types retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving business types");
                return ApiResponse<IEnumerable<BusinessTypeDto>>.Fail(
                    null,
                    "Error retrieving business types",
                    HttpStatusCode.InternalServerError);
            }
        }
    }
}