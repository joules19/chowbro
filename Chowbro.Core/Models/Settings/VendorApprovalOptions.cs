using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chowbro.Core.Models.Settings
{
    public class VendorApprovalOptions
    {
        public TimeSpan CheckInterval { get; set; } = TimeSpan.FromHours(1);
        public int BatchSize { get; set; } = 100;
        public bool AutomaticApprovalEnabled { get; set; } = true;

    }
}
