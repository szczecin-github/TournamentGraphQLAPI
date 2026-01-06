using Microsoft.AspNetCore.Identity;

namespace TournamentGraphQL.Models;

public class User : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    
    // Navigation for EF Core
    public ICollection<Tournament> Tournaments { get; set; } = new List<Tournament>();
}