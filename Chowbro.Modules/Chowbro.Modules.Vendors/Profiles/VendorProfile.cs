using AutoMapper;
using Chowbro.Core.Models.Vendor;

namespace Chowbro.Modules.Vendors.Profiles
{
    public class VendorProfile : Profile
    {
        public VendorProfile()
        {
            CreateMap<Vendor, VendorDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<Branch, BranchDto>();
        }
    }
}