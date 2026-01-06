namespace TournamentGraphQL.GraphQL;

public record AuthPayload(string Token, string Message);
public record LoginInput(string Email, string Password);
public record RegisterInput(string Email, string Password, string FirstName, string LastName);