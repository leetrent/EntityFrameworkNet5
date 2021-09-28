﻿using EntityFrameworkNet5.Data.Configurations.Entities;
using EntityFrameworkNet5.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkNet5.Data
{
    public class FootballLeageDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB; Initial Catalog=FootballLeage_EfCore")
                .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information)
                .EnableSensitiveDataLogging();
        }

        public DbSet<Team> Teams { get; set; }
        public DbSet<League> Leagues { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Coach> Coaches { get; set; }
        public DbSet<TeamsCoachesLeaguesView> TeamsCoachesLeagues { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>()
                .HasMany(t => t.HomeMatches)
                .WithOne(m => m.HomeTeam)
                .HasForeignKey(m => m.HomeTeamId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Team>()
                .HasMany(t => t.AwayMatches)
                .WithOne(m => m.AwayTeam)
                .HasForeignKey(m => m.AwayTeamId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TeamsCoachesLeaguesView>().HasNoKey().ToView("TeamsCoachesLeagues");

            modelBuilder.ApplyConfiguration(new LeagueSeedConfiguration());
            modelBuilder.ApplyConfiguration(new TeamSeedConfiguration());
            modelBuilder.ApplyConfiguration(new CoachSeedConfiguration());
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            /////////////////////////////////////////////////////////////////////////////////////////////////////////
            // NEW ENTITIES TO BE ADDED
            /////////////////////////////////////////////////////////////////////////////////////////////////////////
            IEnumerable<EntityEntry> addEntries = ChangeTracker.Entries().Where(y => y.State == EntityState.Added);
            foreach (EntityEntry addEntry in addEntries)
            {
                BaseDomain baseDomainObj = (BaseDomain)addEntry.Entity;
                baseDomainObj.CreatedDate = DateTime.Now;
            }

            /////////////////////////////////////////////////////////////////////////////////////////////////////////
            // EXISTING ENTITIES TO BE MODIFIED
            /////////////////////////////////////////////////////////////////////////////////////////////////////////
            IEnumerable<EntityEntry> modifiedEntries = ChangeTracker.Entries().Where(y => y.State == EntityState.Modified);
            foreach (EntityEntry modifiedEntry in modifiedEntries)
            {
                BaseDomain baseDomainObj = (BaseDomain)modifiedEntry.Entity;
                baseDomainObj.ModifiedDate = DateTime.Now;
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}