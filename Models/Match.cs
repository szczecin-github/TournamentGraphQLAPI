namespace TournamentGraphQL.Models;

public class Match
{
    public int Id { get; set; }
    public int Round { get; set; }

    public int? BracketId { get; set; }
    public Bracket? Bracket { get; set; }

    public string? Player1Id { get; set; }
    public User? Player1 { get; set; }

    public string? Player2Id { get; set; }
    public User? Player2 { get; set; }

    public string? WinnerId { get; set; }
    public User? Winner { get; set; }
}