﻿using Chowbro.Core.Models;
using Chowbro.Core.Models.Vendor;
using MediatR;

namespace Chowbro.Modules.Vendors.Queries
{
    public class GetVendorMainBranchQuery : IRequest<ApiResponse<BranchDto>>
    {


        public GetVendorMainBranchQuery(Guid vendorId)
        {

        }
    }
}