namespace Chowbro.Core.Models.Location
{
    public class StateWithLgasDto
    {
        public StateDto State { get; set; }
        public IEnumerable<LgaDto> Lgas { get; set; }
    }


    public class StateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}