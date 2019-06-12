using System;
using System.Threading;
using System.Threading.Tasks;
using Minesweeper.GameServices.Cloners;
using Minesweeper.GameServices.Contracts.Requests;
using Minesweeper.GameServices.Contracts.Responses;
using Minesweeper.GameServices.Converters;
using Minesweeper.GameServices.Exceptions;
using Minesweeper.GameServices.Extensions;
using Raven.Client.Documents;

namespace Minesweeper.GameServices.Handlers.RequestHandlers
{
    public class GetPlayerMarksRequestHandler : IGetPlayerMarksRequestHandler
    {
        private readonly IDocumentStore _documentStore;

        public GetPlayerMarksRequestHandler(IDocumentStore documentStore)
        {
            _documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
        }

        public async Task<MarkTypes[][]> HandleAsync(GetPlayerMarksRequest request, CancellationToken cancellationToken)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var playerMarksDocument = await session.LoadPlayerMarksAsync(request.GameId, request.PlayerId, cancellationToken).ConfigureAwait(false);

                if (playerMarksDocument == null)
                {
                    throw new ActionNotAllowedException("You are not involved in that game.");
                }

                return EnumArrayCloner.CloneAndMap(playerMarksDocument.Marks, MarkTypeConverter.ToContract);
            }
        }
    }
}