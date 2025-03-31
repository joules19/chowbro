namespace Chowbro.Core.Entities.Location
{
    public class State : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<Lga> Lgas { get; set; } = new List<Lga>();
    }
}