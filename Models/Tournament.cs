using System.ComponentModel.DataAnnotations;

namespace TournamentGraphQL.Models;

public class Tournament
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, InProgress, Finished

    public ICollection<User> Participants { get; set; } = new List<User>();
    
    public int? BracketId { get; set; }
    public Bracket? Bracket { get; set; }
}