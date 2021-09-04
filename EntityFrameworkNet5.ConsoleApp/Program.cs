using EntityFrameworkNet5.Data;
using EntityFrameworkNet5.Domain;
using Microsoft.EntityFrameworkCore;
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
            //await CreateRecords();
            await RetrieveRecords();

            Console.WriteLine("Press any key to continue...");
            Console.Read();
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
    }
}
