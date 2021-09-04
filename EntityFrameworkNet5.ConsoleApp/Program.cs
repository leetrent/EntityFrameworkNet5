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
            ////////////////////////////////////////////////////////////
            // CREATE LEAGUE
            ////////////////////////////////////////////////////////////
            //League league1 = new() { Name = "Red Stripe Premier League" };
            //await context.Leagues.AddAsync(league1);
            //await context.SaveChangesAsync();

            ////////////////////////////////////////////////////////////
            // ADD NEWLY CREATED TEAMS TO LEAGUE
            ////////////////////////////////////////////////////////////
            //await AddTeamsWithLeague(league1);
            //await context.SaveChangesAsync();

            ////////////////////////////////////////////////////////////
            // CREATE NEW LEAGUE AND NEW TEAM AT THE SAME TIME
            ////////////////////////////////////////////////////////////
            //League league2 = new() { Name = "Bundesliga" };
            //Team team2 = new() {Name = "Bayern Munich", League = league2 };
            //await context.AddAsync(team2); // Entity type not specified. EF Core infers entity type.
            //await context.SaveChangesAsync(); 

            Console.WriteLine("Press any key to continue...");
            Console.Read();
        }

        static async Task AddTeamsWithLeague(League league)
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

    }
}
