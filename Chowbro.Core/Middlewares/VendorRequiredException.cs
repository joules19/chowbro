using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chowbro.Core.Middlewares
{
    public class VendorRequiredException : InvalidOperationException
    {
        public VendorRequiredException()
            : base("Vendor account not properly configured")
        {
        }
    }

}
