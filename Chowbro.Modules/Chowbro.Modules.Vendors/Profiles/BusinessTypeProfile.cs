using AutoMapper;
using Chowbro.Core.Entities.Vendor;
using Chowbro.Modules.Vendors.Commands.BusinessType;
using Chowbro.Modules.Vendors.DTOs.BusinessType;

namespace Chowbro.Modules.Vendors.Profiles;

public class BusinessTypeProfile : Profile
{
    public BusinessTypeProfile()
    {
        CreateMap<BusinessType, BusinessTypeDto>();
        CreateMap<CreateBusinessTypeCommand, BusinessType>();
        CreateMap<UpdateBusinessTypeCommand, BusinessType>();
    }
}