namespace Chowbro.Core.Models.SeedData
{
    public class NigeriaStateSeed
    {
        public string Name { get; set; }
        public List<string> Lgas { get; set; }
    }

    public class NigeriaLocationSeed
    {
        public List<NigeriaStateSeed> States { get; set; }
    }
}