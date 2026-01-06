==========================================================================================
TOURNAMENT GRAPHQL API
================================================================================

An ASP.NET Core GraphQL service for managing knockout tournaments, players, 
and match brackets.

--------------------------------------------------------------------------------
1. SETUP & INSTALLATION
--------------------------------------------------------------------------------

[1] PREREQUISITES
    - .NET 8.0 SDK
    - Git

[2] HOW TO RUN
    1. Open your terminal in the project folder.
    2. Run the command:
       dotnet run

    3. Once the server starts (e.g., listening on http://localhost:5000), 
       open your web browser to the GraphQL IDE:
       http://localhost:5000/graphql/

--------------------------------------------------------------------------------
2. DEMO SCRIPT: "THE MINI TREE CUP"
--------------------------------------------------------------------------------

Copy and paste the following blocks into the GraphQL IDE to simulate a full 
4-team tournament.

STEP 1: REGISTER 4 TEAMS
Run this mutation to create your participants.
--------------------------------------------------------------------------------
mutation RegisterTeams {
  u1: register(input: { email: "oak@trees.com", password: "Password123!", firstName: "Team", lastName: "Oak" }) { message }
  u2: register(input: { email: "pine@trees.com", password: "Password123!", firstName: "Team", lastName: "Pine" }) { message }
  u3: register(input: { email: "maple@trees.com", password: "Password123!", firstName: "Team", lastName: "Maple" }) { message }
  u4: register(input: { email: "birch@trees.com", password: "Password123!", firstName: "Team", lastName: "Birch" }) { message }
}


STEP 2: RETRIEVE SYSTEM IDs
Run this query. COPY THE 4 IDs returned in the result window. 
You will need them for Step 5.
--------------------------------------------------------------------------------
query GetTeamIds {
  users(where: { email: { endsWith: "@trees.com" } }) {
    id
    lastName
  }
}


STEP 3: LOGIN (GET ACCESS TOKEN)
Login as "Team Oak" to authorize tournament creation.
--------------------------------------------------------------------------------
mutation Login {
  login(input: { email: "oak@trees.com", password: "Password123!" }) {
    token
  }
}

*** IMPORTANT ACTION REQUIRED ***
1. Copy the "token" string from the response.
2. In the IDE, click the "Authorization" or "Headers" tab.
3. Select Type "Bearer" and paste the token.


STEP 4: CREATE TOURNAMENT
Create the tournament container.
--------------------------------------------------------------------------------
mutation CreateTournament {
  createTournament(name: "Mini Tree Cup") {
    id
    name
  }
}


STEP 5: ADD PLAYERS & START
Replace "REPLACE_WITH_ID_..." with the real IDs you copied in Step 2.
This adds players and automatically generates the matches.
--------------------------------------------------------------------------------
mutation SetupBracket {
  # Add Participants
  p1: addParticipant(tournamentId: 1, userId: "REPLACE_WITH_ID_OAK") { id }
  p2: addParticipant(tournamentId: 1, userId: "REPLACE_WITH_ID_PINE") { id }
  p3: addParticipant(tournamentId: 1, userId: "REPLACE_WITH_ID_MAPLE") { id }
  p4: addParticipant(tournamentId: 1, userId: "REPLACE_WITH_ID_BIRCH") { id }

  # Generate Bracket
  start: startTournament(tournamentId: 1) {
    status
    bracket { id }
  }
}


STEP 6: VIEW THE BRACKET
See who is playing against whom.
--------------------------------------------------------------------------------
query ViewBracket {
  tournaments(where: { id: { eq: 1 } }) {
    name
    status
    bracket {
      matches {
        id
        round
        player1 { lastName }
        player2 { lastName }
        winner { lastName }
      }
    }
  }
}


STEP 7: PLAY A MATCH (OPTIONAL)
Pick a "matchId" from Step 6 and set a winner (using the winner's User ID).
--------------------------------------------------------------------------------
mutation PlayMatch {
  playMatch(matchId: 1, winnerId: "REPLACE_WITH_WINNER_ID") {
    id
    winner { lastName }
  }
}
