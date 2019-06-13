import { autoinject } from 'aurelia-framework';
import { EventAggregator } from 'aurelia-event-aggregator';

import * as SignalR from '@aspnet/signalr';

import { FieldTypes } from '../interfaces/field-types';

const gameHubUrl = 'https://localhost:5001/hubs/game';

@autoinject()
export class GameHubSignalRService {
    constructor(private eventAggregator: EventAggregator) {

    }

    async connect(gameId: string): Promise<SignalR.HubConnection> {
        // TODO: Error handling
        let connection = new SignalR.HubConnectionBuilder()
            .withUrl(gameHubUrl)
            .build();

        try {
            await connection.start();
        }
        catch (error) {
            console.error(error);

            return;
        }

        await connection.send("SubscribeToGameNotifications", { gameId });

        connection.on("GameTableUpdated", notification => {
            this.eventAggregator.publish(`games:${gameId}:tableChanged`, notification);
        });

        connection.on("RemainingMinesChanged", notification => {
            this.eventAggregator.publish(`games:${gameId}:remainingMinesChanged`, notification);
        });

        connection.on("GameOver", notification => {
            this.eventAggregator.publish(`games:${gameId}:gameOver`, notification);
        });

        connection.on("PointsChanged", notification => {
            this.eventAggregator.publish(`games:${gameId}:pointsChanged`, notification);
        });

        connection.on("TurnChanged", notification => {
            this.eventAggregator.publish(`games:${gameId}:turnChanged`, notification);
        });

        return connection;
    }
}

export interface GameTableUpdatedNotification {
    fieldUpdates: FieldUpdate[];
}

export interface FieldUpdate {
    row: number;
    column: number;
    fieldType: FieldTypes;
}

export interface RemainingMinesChangedNotification {
    remainingMineCount: number;
}

export interface GameOverNotification {
    winnerPlayerId: string;
}

export interface PlayerPointsChangedNotification {
    playerId: string;
    points: number;
}

export interface TurnChangedNotification {
    playerId: string;
}
