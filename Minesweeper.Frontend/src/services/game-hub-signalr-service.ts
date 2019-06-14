import * as SignalR from '@aspnet/signalr';
import { FieldTypes } from '../interfaces/field-types';

const gameHubUrl = 'https://localhost:5001/hubs/game';

export class GameHubSignalRService {

    async connect(gameId: string): Promise<GameSession> {
        // TODO: Error handling
        let connection = new SignalR.HubConnectionBuilder()
            .withUrl(gameHubUrl)
            .build();

        try {
            await connection.start();
        }
        catch (error) {
            console.error(error);

            return null;
        }

        await connection.send("SubscribeToGameNotifications", { gameId });

        return new GameSession(connection);
    }
}

export class GameSession {

    constructor(private connection: SignalR.HubConnection) {
    }

    async dispose() {
        await this.connection.stop();
    }

    onTurnChanged(callback: (notification: TurnChangedNotification) => void) {
        this.connection.on("TurnChanged", notification => {
            callback(notification);
        });
    }

    onPointsChanged(callback: (notification: PlayerPointsChangedNotification) => void) {
        this.connection.on("PointsChanged", notification => {
            callback(notification);
        });
    }

    onGameOver(callback: (notification: GameOverNotification) => void) {
        this.connection.on("GameOver", notification => {
            callback(notification);
        });
    }

    onRemainingMinesChanged(callback: (notification: RemainingMinesChangedNotification) => void) {
        this.connection.on("RemainingMinesChanged", notification => {
            callback(notification);
        });
    }

    onGameTableUpdated(callback: (notification: GameTableUpdatedNotification) => void) {
        this.connection.on("GameTableUpdated", notification => {
            callback(notification);
        });
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
