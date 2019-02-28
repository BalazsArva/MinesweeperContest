import { autoinject } from 'aurelia-framework';
import { EventAggregator } from 'aurelia-event-aggregator';
import * as SignalR from '@aspnet/signalr';

const gameHubUrl = 'https://localhost:5001/hubs/game';

@autoinject
export class GameHubSignalRService {
    constructor(private eventAggregator: EventAggregator) {

    }

    async connect(gameId: string) {
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

        connection.send("JoinGame", { gameId });

        console.log(`SignalR connection to Game hub with GameId=${gameId} succeeded.`);

        connection.on("GameTableUpdated", (newGameTable) => {
            console.log(newGameTable);
            console.log("game table updated");

            this.eventAggregator.publish(`games:${gameId}:tableChanged`, { newGameTable });
        });
    }
}