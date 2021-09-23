using EntityFrameworkNet5.Data;
using EntityFrameworkNet5.Domain;
using EntityFrameworkNet5.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EntityFrameworkNet5.ConsoleApp
{
    class Program
    {
        private static readonly FootballLeageDbContext context = new FootballLeageDbContext();

        static async Task Main(string[] args)
        {
            //await CreateRecords();
            //await RetrieveRecords();
            //await QueryFilters();
            //await AdditionalQueryMethods();
            //await AlternativeLinqSyntax();
            //await QueryRelatedRecords();

            await StronglyTypedProjection();
            Console.WriteLine("\nPress any key to continue...");
            Console.Read();
        }

            async static Task SelectOneProperty()
        {
            var teams = await context.Teams.Select(q => q.Name).ToListAsync();
        }

        async static Task AnonymousProjection()
        {
            var teams = await context.Teams.Include(q => q.Coach).Select(
                q => 
                new { 
                        TeamName = q.Name, 
                        CoachName = q.Coach.Name
                    }
                ).ToListAsync();

            foreach (var item in teams)
            {
                Console.WriteLine($"Team: {item.TeamName} | Coach: {item.CoachName}");
            }
        }

        async static Task StronglyTypedProjection()
        {
            var teams = await context.Teams.Include(q => q.Coach).Include(q => q.League).Select(
                q =>
                new TeamDetail {
                    TeamName = q.Name,
                    CoachName = q.Coach.Name,
                    LeagueName = q.League.Name
                }
                ).ToListAsync();

            Console.WriteLine("");
            foreach (var item in teams)
            {
                Console.WriteLine($"Team: {item.TeamName} | Coach: {item.CoachName} | League: {item.LeagueName}");
            }
        }

        static async Task QueryRelatedRecords()
        {
            //// Get Many Related Records - Leagues -> Teams
            //var leagues = await context.Leagues.Include(q => q.Teams).ToListAsync();

            //// Get One Related Record - Team -> Coach
            //var team = await context.Teams
            //    .Include(q => q.Coach)
            //    .FirstOrDefaultAsync(q => q.Id == 3);

            //// Get 'Grand Children' Related Record - Team -> Matches -> Home/Away Team
            //var teamsWithMatchesAndOpponents = await context.Teams
            //    .Include(q => q.AwayMatches).ThenInclude(q => q.HomeTeam).ThenInclude(q => q.Coach)
            //    .Include(q => q.HomeMatches).ThenInclude(q => q.AwayTeam).ThenInclude(q => q.Coach)
            //    .FirstOrDefaultAsync(q => q.Id == 1);

            //// Get Includes with filters
            var teams = await context.Teams
                .Where(q => q.HomeMatches.Count > 0)
                .Include(q => q.Coach)
                .ToListAsync();
        }

        static async Task CreateRecords()
        {
            League league = await AddNewLeague(name: "Red Stripe Premier League");
            await AddNewTeamsToLeague(league);
            Team team = await AddNewTeamToNewLeague(leagueName: "Bundesliga", teamName: "Bayern Munich");
        }

        static async Task RetrieveRecords()
        {
            await SimpleSelectQuery();
        }


        static async Task<League> AddNewLeague(string name)
        {
            League league = new() { Name = name };
            await context.Leagues.AddAsync(league);
            await context.SaveChangesAsync();
            return league;
        }

        static async Task AddNewTeamsToLeague(League league)
        {
            var teams = new List<Team>
            {
                new Team
                {
                    Name = "Juventus",
                    LeagueId = league.Id // Foreign Key Id
                },
                new Team
                {
                    Name = "AC Milan",
                    LeagueId = league.Id // Foreign Key Id
                },
                new Team
                {
                    Name = "AS Roma",
                    League = league // Foreign Key Navigation Object
                }
            };

            //// Operation to add multiple objects to database in one call.
            await context.AddRangeAsync(teams);
        }

        static async Task<Team> AddNewTeamToNewLeague(string leagueName, string teamName)
        {
            League league = new() { Name = leagueName };
            Team team = new() { Name = teamName, League = league };
            await context.AddAsync(team); // Entity type not specified. EF Core infers entity type.
            await context.SaveChangesAsync();
            return team;
        }
        static async Task SimpleSelectQuery()
        {
            //// Smartest most efficient way to get results
            var leagues = await context.Leagues.ToListAsync();
            foreach (var league in leagues)
            {
                Console.WriteLine($"{league.Id} - {league.Name}");
            }

            //// Inefficient way to get results. Keeps connection open until completed and might create lock on table
            ////foreach (var league in context.Leagues)
            ////{
            ////    Console.WriteLine($"{league.Id} - {league.Name}");
            ////}
        }

        static async Task QueryFilters()
        {
            Console.Write("\nEnter league name: ");
            string leagueName = Console.ReadLine();
            Console.WriteLine($"Searching for '{leagueName}':\n");

            List<League> exactMatches = await context.Leagues.Where(q => q.Name.Equals(leagueName)).ToListAsync();
            Console.WriteLine("\nEXACT MATCHES: ");
            foreach (var league in exactMatches)
            {
                Console.WriteLine($"{league.Id} - {league.Name}");
            }

            // CONTAINS         
            //List<League> partialMatches = await context.Leagues.Where(q => q.Name.Contains(leagueName)).ToListAsync();

            // EF.Functions.Like
            List<League> partialMatches = await context.Leagues.Where(q => EF.Functions.Like(q.Name, $"%{leagueName}%")).ToListAsync();

            Console.WriteLine("\nPARTIAL MATCHES:\n ");
            foreach (var league in partialMatches)
            {
                Console.WriteLine($"{league.Id} - {league.Name}");
            }
        }

        static async Task AdditionalQueryMethods()
        {
            //// These methods also have non-async
            //var leagues = context.Leagues;
            //var list = await leagues.ToListAsync();
            //var first = await leagues.FirstAsync();
            //var firstOrDefault = await leagues.FirstOrDefaultAsync();
            //var single = await leagues.SingleAsync();
            //var singleOrDefault = await leagues.SingleOrDefaultAsync();

            //var count = await leagues.CountAsync();
            long nbrOfTeams = await context.Teams.LongCountAsync();
            int minTeamId = await context.Teams.MinAsync(q => q.Id);
            int maxTeamId = await context.Teams.MaxAsync(q => q.Id);

            Console.WriteLine();
            Console.WriteLine($"# of teams.....: {nbrOfTeams}");
            Console.WriteLine($"Lowest Team Id.: {minTeamId}");
            Console.WriteLine($"Highest Team Id: {maxTeamId}");
            Console.WriteLine();


            //var max = await leagues.MaxAsync();

            //// DbSet Method that will execute
            //var league = await context.Leagues.FindAsync(1);
        }

        static async Task AlternativeLinqSyntax()
        {
            Console.Write($"Enter Team Name (or part of): ");
            var teamName = Console.ReadLine();
            var teams = await (from i in context.Teams
                               where EF.Functions.Like(i.Name, $"%{teamName}%")
                               select i).ToListAsync();

            foreach (var team in teams)
            {
                Console.WriteLine($"{team.Id} - {team.Name}");
            }
        }
    }
}
