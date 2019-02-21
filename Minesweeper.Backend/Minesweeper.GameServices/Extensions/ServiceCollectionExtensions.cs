using Microsoft.Extensions.DependencyInjection;
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
                .AddSingleton<IGuidProvider, GuidProvider>()
                .AddSingleton<IRandomNumberProvider, RandomNumberProvider>()
                .AddSingleton<IDateTimeProvider, DateTimeProvider>()
                .AddSingleton<IGameDriver, GameDriver>()
                .AddSingleton<IGameService, GameService>()
                .AddSingleton<IRandomPlayerSelector, RandomPlayerSelector>()
                .AddSingleton<IGameTableGenerator, GameTableGenerator>()
                .AddSingleton<IGameGenerator, GameGenerator>();
        }
    }
}