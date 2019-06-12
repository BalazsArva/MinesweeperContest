﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Minesweeper.GameServices.Cloners;
using Minesweeper.GameServices.Contracts;
using Minesweeper.GameServices.Contracts.Responses;
using Minesweeper.GameServices.Converters;
using Minesweeper.GameServices.Exceptions;
using Minesweeper.GameServices.Extensions;
using Raven.Client.Documents;

namespace Minesweeper.GameServices
{
    public class GameService : IGameService
    {
        private readonly IDocumentStore _documentStore;

        public GameService(IDocumentStore documentStore)
        {
            _documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
        }

        public async Task<MarkTypes[][]> GetPlayerMarksAsync(string gameId, string playerId, CancellationToken cancellationToken)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var playerMarksDocument = await session.LoadPlayerMarksAsync(gameId, playerId, cancellationToken).ConfigureAwait(false);

                if (playerMarksDocument == null)
                {
                    throw new ActionNotAllowedException("You are not involved in that game.");
                }

                return EnumArrayCloner.CloneAndMap(playerMarksDocument.Marks, MarkTypeConverter.ToContract);
            }
        }

        public async Task<VisibleFieldType[][]> GetVisibleGameTableAsync(string gameId, CancellationToken cancellationToken)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                // TODO: Validate user id
                var game = await session.LoadGameAsync(gameId, cancellationToken).ConfigureAwait(false);

                return EnumArrayCloner.CloneAndMap(game.VisibleTable, FieldTypeConverter.ToContract);
            }
        }
    }
}