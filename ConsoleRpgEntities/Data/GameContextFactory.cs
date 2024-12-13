﻿using ConsoleRpgEntities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ConsoleRpgEntities.Data
{
    public class GameContextFactory : IDesignTimeDbContextFactory<GameContext>
    {
        public GameContext CreateDbContext(string[] args)
        {
            // Explicitly set the base path to the ConsoleRpg project
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "ConsoleRpg");

            // Build configuration
            var configuration = ConfigurationHelper.GetConfiguration(basePath);

            // Get connection string
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Build options
            var optionsBuilder = new DbContextOptionsBuilder<GameContext>();
            ConfigurationHelper.ConfigureDbContextOptions(optionsBuilder, connectionString);

            // Create and return the context
            return new GameContext(optionsBuilder.Options);
        }
    }
}
