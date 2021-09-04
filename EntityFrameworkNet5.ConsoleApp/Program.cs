using EntityFrameworkNet5.Data;
using EntityFrameworkNet5.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EntityFrameworkNet5.ConsoleApp
{
    class Program
    {
        private static readonly FootballLeageDbContext context = new FootballLeageDbContext();

        static async Task Main(string[] args)
        {
            await InsertRecords();

            Console.WriteLine("Press any key to continue...");
            Console.Read();
        }

        static async Task InsertRecords()
        {
            League league = await AddNewLeague(name: "Red Stripe Premier League");
            await AddNewTeamsToLeague(league);
            Team team = await AddNewTeamToNewLeague(leagueName: "Bundesliga", teamName: "Bayern Munich");
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

    }
}
