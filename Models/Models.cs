using System.ComponentModel.DataAnnotations;

namespace CagedApi.Models
{
    public class Chord
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required List<int> Shape { get; set; }
        public List<int>? MutedFrets { get; set; }
        public int? Barre { get; set; }
        public string? BarreIndicator { get; set; }
    }

    public class Progression
    {
        [Key]
        public required string Title { get; set; }
        public required List<Chord> ChordList { get; set; }
    }
}