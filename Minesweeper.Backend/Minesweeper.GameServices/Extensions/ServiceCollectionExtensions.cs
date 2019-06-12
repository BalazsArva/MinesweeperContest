using Microsoft.Extensions.DependencyInjection;
using Minesweeper.GameServices.Contracts;
using Minesweeper.GameServices.Generators;
using Minesweeper.GameServices.Handlers.CommandHandlers;
using Minesweeper.GameServices.Providers;

namespace Minesweeper.GameServices.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGameServices(this IServiceCollection services)
        {
            // TODO: Review lifetime for each item
            return services
                .AddScoped<IGuidProvider, GuidProvider>()
                .AddScoped<IRandomNumberProvider, RandomNumberProvider>()
                .AddScoped<IDateTimeProvider, DateTimeProvider>()
                .AddScoped<IGameDriver, GameDriver>()
                .AddScoped<IGameService, GameService>()
                .AddScoped<ILobbyService, LobbyService>()
                .AddScoped<IRandomPlayerSelector, RandomPlayerSelector>()
                .AddScoped<IGameGenerator, GameGenerator>()
                .AddScoped<IPlayerMarksGenerator, PlayerMarksGenerator>()
                .AddScoped<IMakeMoveCommandHandler, MakeMoveCommandHandler>()
                .AddScoped<INewGameCommandHandler, NewGameCommandHandler>()
                .AddScoped<IMarkFieldCommandHandler, MarkFieldCommandHandler>()
                .AddScoped<IJoinGameCommandHandler, JoinGameCommandHandler>();
        }
    }
}