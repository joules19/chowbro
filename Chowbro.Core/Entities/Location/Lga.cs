
namespace Chowbro.Core.Entities.Location
{
    public class Lga : BaseEntity
    {
        public string Name { get; set; }
        public Guid StateId { get; set; }
        public State State { get; set; }
    }
}