namespace CagedApi.Models

{
    public record User
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public List<Progression> ProgressionList = new List<Progression>();
    }
    public class Progression
    {
        public int ProgressionId { get; set; }
        public required string Title { get; set; }
        public required List<Chord> ChordList { get; set; }
    }
    public class Chord
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required List<int> Shape { get; set; }
        public List<int> MutedFrets { get; set; } = new List<int>();
        public string Barre { get; set; } = "";
        public string BarreIndicator { get; set; } = "";
    }
}