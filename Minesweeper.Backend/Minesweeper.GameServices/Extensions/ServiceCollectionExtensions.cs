﻿using Microsoft.Extensions.DependencyInjection;
using Minesweeper.GameServices.Contracts;
using Minesweeper.GameServices.Generators;
using Minesweeper.GameServices.Providers;

namespace Minesweeper.GameServices.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGameServices(this IServiceCollection services)
        {
            return services
                .AddScoped<IGuidProvider, GuidProvider>()
                .AddScoped<IRandomNumberProvider, RandomNumberProvider>()
                .AddScoped<IDateTimeProvider, DateTimeProvider>()
                .AddScoped<IGameDriver, GameDriver>()
                .AddScoped<IGameTableVisibilityComputer, GameTableVisibilityComputer>()
                .AddScoped<IGameService, GameService>()
                .AddScoped<IRandomPlayerSelector, RandomPlayerSelector>()
                .AddScoped<IGameTableGenerator, GameTableGenerator>()
                .AddScoped<IGameGenerator, GameGenerator>();
        }
    }
}