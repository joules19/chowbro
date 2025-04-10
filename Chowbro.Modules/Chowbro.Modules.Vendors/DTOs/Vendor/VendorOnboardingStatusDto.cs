namespace Chowbro.Modules.Vendors.DTOs.Vendor
{
    public class VendorOnboardingStatusDto
    {
        public bool IsPersonalInformationComplete { get; set; }
        public bool IsBusinessInformationComplete { get; set; }
        public bool IsLogoComplete { get; set; }
        public bool IsCoverImageComplete { get; set; }
        public bool IsBusinessAddressComplete { get; set; }
        public bool IsStoreOperationComplete { get; set; }
        public decimal CompletionPercentage { get; set; }
        public List<string> PendingSteps { get; set; } = new();
    }
}