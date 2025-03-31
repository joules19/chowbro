namespace Chowbro.Core.Models.Location
{
    public class StateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<string> Lgas { get; set; } = new();
    }
}