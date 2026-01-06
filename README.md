# 🏆 Tournament GraphQL API

A simple ASP.NET Core GraphQL API for managing knockout tournaments. Built with **HotChocolate**, **Entity Framework Core**, and **ASP.NET Core Identity**.

## 🚀 Features
* **User Management:** Register and Login (JWT Authentication).
* **Tournament Control:** Create tournaments, add participants, and generate brackets.
* **Match System:** View matches and record results.
* **Privacy:** Users can fetch their own matches securely without exposing their ID.

## 🛠️ Prerequisites
* [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
* Git

## 🏃‍♂️ How to Run

1.  **Clone the repository:**
    ```bash
    git clone [https://github.com/YOUR_USERNAME/TournamentGraphQL.git](https://github.com/YOUR_USERNAME/TournamentGraphQL.git)
    cd TournamentGraphQL
    ```

2.  **Run the application:**
    ```bash
    dotnet run
    ```

3.  **Open the GraphQL Interface:**
    Look at the console output to find your local port (e.g., `http://localhost:5000`).
    Open your browser and navigate to:
    👉 **`http://localhost:YOUR_PORT/graphql/`**

    This opens **Banana Cake Pop**, a visual tool to test your API.

---

## 🌲 Quick Start: The "Mini Tree Cup" (4 Teams)

Follow these steps in **Banana Cake Pop** to run a complete 4-team tournament simulation.

### Step 1: Register 4 Teams
Copy and paste this mutation to create your teams:

```graphql
mutation RegisterTeams {
  t1: register(input: { email: "oak@trees.com", password: "Password123!", firstName: "Team", lastName: "Oak" }) { message }
  t2: register(input: { email: "pine@trees.com", password: "Password123!", firstName: "Team", lastName: "Pine" }) { message }
  t3: register(input: { email: "maple@trees.com", password: "Password123!", firstName: "Team", lastName: "Maple" }) { message }
  t4: register(input: { email: "birch@trees.com", password: "Password123!", firstName: "Team", lastName: "Birch" }) { message }
}
```
Step 2: Get Team IDs

Run this query to find the IDs assigned to your new teams. Copy these 4 IDs, you will need them for Step 5.
GraphQL

```graphql
query GetTeamIds {
  users(where: { email: { endsWith: "@trees.com" } }) {
    id
    lastName
  }
}
```
Step 3: Login (Get Access Token)

Login as "Team Oak" to get permission to create a tournament.
GraphQL

```graphql
mutation Login {
  login(input: { email: "oak@trees.com", password: "Password123!" }) {
    token
  }
}

    🔐 Authorize:

        Copy the token string from the response.

        Click the "Authorization" (or Headers) tab in Banana Cake Pop.

        Select Type: Bearer.

        Paste the token.
```
Step 4: Create the Tournament

Create the tournament container.
GraphQL

```graphql
mutation CreateTournament {
  createTournament(name: "Mini Tree Cup") {
    id
    name
  }
}
```
Step 5: Add Teams & Start

Action: Replace REPLACE_WITH_ID_X with the actual IDs you copied in Step 2.
GraphQL

```graphql
mutation SetupBracket {
  # Add 4 Participants
  p1: addParticipant(tournamentId: 1, userId: "REPLACE_WITH_ID_OAK") { id }
  p2: addParticipant(tournamentId: 1, userId: "REPLACE_WITH_ID_PINE") { id }
  p3: addParticipant(tournamentId: 1, userId: "REPLACE_WITH_ID_MAPLE") { id }
  p4: addParticipant(tournamentId: 1, userId: "REPLACE_WITH_ID_BIRCH") { id }

  # Start and Generate Bracket
  start: startTournament(tournamentId: 1) {
    status
    bracket { id }
  }
}
```
Step 6: View the Bracket

See who is playing against whom!
GraphQL

```graphql
query ViewMatches {
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
```
Step 7: Play a Match (Optional)

Pick a matchId from Step 6 and set a winner (using the winner's User ID).
GraphQL

```graphql
mutation PlayMatch {
  playMatch(matchId: 1, winnerId: "REPLACE_WITH_WINNER_ID") {
    winner { lastName }
  }
}
```
