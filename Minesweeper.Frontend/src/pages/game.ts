import { autoinject } from "aurelia-framework";
import { EventAggregator } from 'aurelia-event-aggregator';

import { GameService, MarkTypes } from "services/game-service";
import { FieldTypes } from "../interfaces/field-types";
import {
    GameHubSignalRService,
    GameTableUpdatedNotification,
    RemainingMinesChangedNotification,
    PlayerPointsChangedNotification,
    GameOverNotification,
    TurnChangedNotification
} from "services/game-hub-signalr-service";
import { AccountService } from "services/identity/account-service";

interface Field {
    fieldType: FieldTypes;
    markType: MarkTypes;
    row: number;
    column: number;
}

@autoinject()
export class Game {

    gameTable: Field[][] = null;
    timerHandle: number = null;
    elapsedSeconds = 0;
    remainingMines: number = null;
    gameId: string = null;
    myPlayerId: string = null;
    isMyTurn: boolean = null;

    myPoints: number = 0;
    opponentsPoints: number = 0;

    constructor(private eventAggregator: EventAggregator, private accountService: AccountService, private gameService: GameService, private gameHubService: GameHubSignalRService) {
    }

    async activate(params: any) {
        let gameId = <string>params.gameId;

        this.gameId = gameId;

        // TODO: Retrieve which player's turn it is at start
        // TODO: Retrieve the points at start (so that interrupted games, reloaded pages don't show 0-0 points until a change event is received).
        await this.initializeUserInfo();
        await this.updateTable();
        await this.updateMarks();
        await this.gameHubService.connect(gameId);

        // TODO: Consider interrupted games 
        this.timerHandle = <number><any>setInterval(_ => ++this.elapsedSeconds, 1000);

        this.eventAggregator.subscribe(`games:${gameId}:tableChanged`, (notification: GameTableUpdatedNotification) => {
            for (let i = 0; i < notification.fieldUpdates.length; ++i) {
                let fieldUpdate = notification.fieldUpdates[i];
                this.gameTable[fieldUpdate.row][fieldUpdate.column].fieldType = fieldUpdate.fieldType;
            }
        });

        this.eventAggregator.subscribe(`games:${gameId}:remainingMinesChanged`, (notification: RemainingMinesChangedNotification) => {
            this.remainingMines = notification.remainingMineCount;
        });

        this.eventAggregator.subscribe(`games:${gameId}:gameOver`, (notification: GameOverNotification) => {
            // TODO: Create better notification UX
            if (notification.winnerPlayerId === this.myPlayerId) {
                alert("You won!");
            } else {
                alert("You lost!");
            }
        });

        this.eventAggregator.subscribe(`games:${gameId}:pointsChanged`, (notification: PlayerPointsChangedNotification) => {
            if (notification.playerId === this.myPlayerId) {
                this.myPoints = notification.points;
            } else {
                this.opponentsPoints = notification.points;
            }
        });

        this.eventAggregator.subscribe(`games:${gameId}:turnChanged`, (notification: TurnChangedNotification) => {
            if (notification.playerId === this.myPlayerId) {
                this.isMyTurn = true;
            } else {
                this.isMyTurn = false;
            }
        });
    }

    async initializeUserInfo() {
        let result = await this.accountService.getUserInfo();

        // TODO: Show a message
        if (!result.success) {
            return;
        }

        this.myPlayerId = result.userInfo.id;
    }

    async updateTable() {
        let table = await this.gameService.getGameTable(this.gameId);

        let newTableState: Field[][] = [];
        for (let row = 0; row < table.visibleTable.length; ++row) {
            newTableState.push([]);

            for (let col = 0; col < table.visibleTable[row].length; ++col) {
                newTableState[row][col] = {
                    fieldType: table.visibleTable[row][col],
                    markType: MarkTypes.None,
                    column: col,
                    row: row
                };
            }
        }

        this.gameTable = newTableState;
    }

    async updateMarks() {
        let marks = await this.gameService.getPlayerMarks(this.gameId);

        // TODO: Show a message
        if (!marks.success) {
            return;
        }

        for (let row = 0; row < this.gameTable.length; ++row) {
            for (let col = 0; col < this.gameTable[row].length; ++col) {
                this.gameTable[row][col].markType = marks.marks[row][col];
            }
        }
    }

    async makeMove(e: MouseEvent, row: number, col: number) {
        // Left mouse button
        if (e.button !== 0) {
            return;
        }

        // Prevent moving on marked field
        if (this.gameTable[row][col].markType !== MarkTypes.None) {
            return;
        }

        await this.gameService.makeMove(this.gameId, row, col);

        e.preventDefault();
        return false;
    }

    async markField(e: MouseEvent, row: number, col: number) {
        // Right mouse button
        if (e.button !== 2) {
            return;
        }

        let targetMarkType: MarkTypes;
        let currentMarkType = this.gameTable[row][col].markType;

        if (currentMarkType === MarkTypes.None) {
            targetMarkType = MarkTypes.Empty;
        }
        else if (currentMarkType === MarkTypes.Empty) {
            targetMarkType = MarkTypes.Unknown;
        }
        else {
            targetMarkType = MarkTypes.None;
        }

        let result = await this.gameService.markField(this.gameId, row, col, targetMarkType);
        if (result.success) {
            this.gameTable[row][col].markType = targetMarkType;
        }
        else {
            // TODO: Error handling
        }

        e.preventDefault();
        return false;
    }
}
