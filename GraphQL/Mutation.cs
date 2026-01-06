using HotChocolate.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TournamentGraphQL.Data;
using TournamentGraphQL.Models;

namespace TournamentGraphQL.GraphQL;

public class Mutation
{
    // --- AUTHENTICATION ---

    public async Task<AuthPayload> Register(
        RegisterInput input,
        [Service] UserManager<User> userManager)
    {
        var user = new User 
        { 
            UserName = input.Email, 
            Email = input.Email,
            FirstName = input.FirstName,
            LastName = input.LastName
        };

        var result = await userManager.CreateAsync(user, input.Password);

        if (!result.Succeeded)
            throw new GraphQLException(string.Join(", ", result.Errors.Select(e => e.Description)));

        return new AuthPayload("Please login to get token", "Registration Successful");
    }

    public async Task<AuthPayload> Login(
        LoginInput input,
        [Service] UserManager<User> userManager,
        [Service] IConfiguration config)
    {
        var user = await userManager.FindByEmailAsync(input.Email);
        if (user == null || !await userManager.CheckPasswordAsync(user, input.Password))
            throw new GraphQLException("Invalid credentials");

        var authClaims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
        };

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));

        var token = new JwtSecurityToken(
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return new AuthPayload(new JwtSecurityTokenHandler().WriteToken(token), "Login Successful");
    }

    // --- DOMAIN LOGIC ---

    public async Task<Tournament> CreateTournament(string name, AppDbContext context)
    {
        var t = new Tournament { Name = name, Status = "Pending" };
        context.Tournaments.Add(t);
        await context.SaveChangesAsync();
        return t;
    }

    // Corresponds to Tournament.addParticipant
    public async Task<Tournament> AddParticipant(int tournamentId, string userId, AppDbContext context)
    {
        var tournament = await context.Tournaments.Include(t => t.Participants).FirstOrDefaultAsync(t => t.Id == tournamentId);
        var user = await context.Users.FindAsync(userId);

        if (tournament == null || user == null) throw new GraphQLException("Tournament or User not found.");
        if (tournament.Status != "Pending") throw new GraphQLException("Cannot add participants to a started tournament.");

        tournament.Participants.Add(user);
        await context.SaveChangesAsync();
        return tournament;
    }

    // Corresponds to Tournament.start() which triggers Bracket.generateBracket()
    public async Task<Tournament> StartTournament(int tournamentId, AppDbContext context)
    {
        var tournament = await context.Tournaments
            .Include(t => t.Participants)
            .Include(t => t.Bracket)
            .FirstOrDefaultAsync(t => t.Id == tournamentId);

        if (tournament == null) throw new GraphQLException("Tournament not found.");
        if (tournament.Participants.Count < 2) throw new GraphQLException("Need at least 2 players.");

        tournament.Status = "InProgress";
        tournament.StartDate = DateTime.UtcNow;

        // Generate Bracket Logic
        var bracket = new Bracket { TournamentId = tournamentId };
        context.Brackets.Add(bracket);
        
        // Simple pairing logic (Round 1)
        var players = tournament.Participants.OrderBy(a => Guid.NewGuid()).ToList(); // Shuffle
        for (int i = 0; i < players.Count; i += 2)
        {
            var match = new Match
            {
                Bracket = bracket,
                Round = 1,
                Player1 = players[i],
                // Handle odd number of players (Bye)
                Player2 = (i + 1 < players.Count) ? players[i + 1] : null, 
                // If Player 2 is null, Player 1 wins automatically
                Winner = (i + 1 < players.Count) ? null : players[i] 
            };
            context.Matches.Add(match);
        }

        await context.SaveChangesAsync();
        return tournament;
    }

    // Corresponds to Match.play(winner)
    public async Task<Match> PlayMatch(int matchId, string winnerId, AppDbContext context)
    {
        var match = await context.Matches.FindAsync(matchId);
        if (match == null) throw new GraphQLException("Match not found.");
        
        if (match.Player1Id != winnerId && match.Player2Id != winnerId)
             throw new GraphQLException("Winner must be one of the participants.");

        match.WinnerId = winnerId;
        
        // Check if tournament is finished logic could go here (Tournament.finish)
        
        await context.SaveChangesAsync();
        return match;
    }
}