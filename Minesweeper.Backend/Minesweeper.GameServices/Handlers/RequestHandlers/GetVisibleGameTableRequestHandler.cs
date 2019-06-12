﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Minesweeper.GameServices.Cloners;
using Minesweeper.GameServices.Contracts.Requests;
using Minesweeper.GameServices.Contracts.Responses;
using Minesweeper.GameServices.Converters;
using Minesweeper.GameServices.Extensions;
using Raven.Client.Documents;

namespace Minesweeper.GameServices.Handlers.RequestHandlers
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