using System;
using System.Threading;
using System.Threading.Tasks;
using Minesweeper.GameServices.Contracts.Requests.Game;
using Minesweeper.GameServices.Contracts.Responses.Game;
using Minesweeper.GameServices.Extensions;
using Minesweeper.GameServices.Utilities.Cloners;
using Minesweeper.GameServices.Utilities.Converters;
using Raven.Client.Documents;

namespace Minesweeper.GameServices.Handlers.RequestHandlers.Game
{
    public class GetVisibleGameTableRequestHandler : IGetVisibleGameTableRequestHandler
    {
        private readonly IDocumentStore _documentStore;

        public GetVisibleGameTableRequestHandler(IDocumentStore documentStore)
        {
            _documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
        }

        public async Task<VisibleFieldType[][]> HandleAsync(GetVisibleGameTableRequest request, CancellationToken cancellationToken)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                // TODO: Validate user id
                var game = await session.LoadGameAsync(request.GameId, cancellationToken).ConfigureAwait(false);

                return EnumArrayCloner.CloneAndMap(game.VisibleTable, FieldTypeConverter.ToContract);
            }
        }
    }
}