namespace Chowbro.Core.Models.Location
{
    public class LgaDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string StateName { get; set; }
    }

    public class LgaWithStateNameDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid StateId { get; set; }
    }

    public class LgaDetailDto
    {
        public LgaDto Lga { get; set; }
        public StateDto State { get; set; }
    }
}