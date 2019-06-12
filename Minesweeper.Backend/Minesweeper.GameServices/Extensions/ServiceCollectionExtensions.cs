using Microsoft.Extensions.DependencyInjection;
using Minesweeper.GameServices.Contracts;
using Minesweeper.GameServices.Generators;
using Minesweeper.GameServices.Handlers.CommandHandlers;
using Minesweeper.GameServices.Handlers.RequestHandlers;
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
                .AddSingleton<ILobbyService, LobbyService>()
                .AddSingleton<IRandomPlayerSelector, RandomPlayerSelector>()
                .AddSingleton<IGameGenerator, GameGenerator>()
                .AddSingleton<IPlayerMarksGenerator, PlayerMarksGenerator>()
                .AddSingleton<IMakeMoveCommandHandler, MakeMoveCommandHandler>()
                .AddSingleton<INewGameCommandHandler, NewGameCommandHandler>()
                .AddSingleton<IMarkFieldCommandHandler, MarkFieldCommandHandler>()
                .AddSingleton<IJoinGameCommandHandler, JoinGameCommandHandler>()
                .AddSingleton<IGetPlayerMarksRequestHandler, GetPlayerMarksRequestHandler>()
                .AddSingleton<IGetVisibleGameTableRequestHandler, GetVisibleGameTableRequestHandler>();
        }
    }
}