import { autoinject } from "aurelia-framework";

import BrowserConstants from '../constants/browser-constants';
import { GameService, MarkTypes, Players } from "services/game-service";
import { FieldTypes } from "../interfaces/field-types";
import {
    GameHubSignalRService,
    GameTableUpdatedNotification,
    RemainingMinesChangedNotification,
    PlayerPointsChangedNotification,
    GameOverNotification,
    TurnChangedNotification,
    GameSession
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
    isPlayer1: boolean = null;

    myPoints: number = 0;
    opponentsPoints: number = 0;

    gameSession: GameSession = null;

    constructor(private accountService: AccountService, private gameService: GameService, private gameHubService: GameHubSignalRService) {
    }

    async activate(params: any) {
        let gameId = <string>params.gameId;

        this.gameId = gameId;

        let playerId = await this.initializeUserInfo();

        await this.initializeGameState(gameId, playerId);
        await this.updateTable(gameId);
        await this.updateMarks(gameId);

        let session = await this.gameHubService.connect(gameId);

        session.onGameOver(this.onGameOver.bind(this));
        session.onGameTableUpdated(this.onTableChanged.bind(this));
        session.onPointsChanged(this.onPointsChanged.bind(this));
        session.onRemainingMinesChanged(this.onRemainingMinesChanged.bind(this));
        session.onTurnChanged(this.onTurnChanged.bind(this));

        // TODO: Consider interrupted games 
        this.timerHandle = <number><any>setInterval(_ => ++this.elapsedSeconds, 1000);
        this.gameSession = session;
    };

    async deactivate() {
        clearInterval(this.timerHandle);

        await this.gameSession.dispose();
    }

    async initializeGameState(gameId: string, playerId: string) {
        let result = await this.gameService.getGameState(gameId);

        let isPlayer1 = playerId === result.player1State.playerId;
        let player = isPlayer1 ? Players.Player1 : Players.Player2;

        this.isPlayer1 = isPlayer1;
        this.isMyTurn = result.nextPlayer === player;
        this.remainingMines = result.remainingMines;
        this.myPoints = isPlayer1 ? result.player1State.points : result.player2State.points;
        this.opponentsPoints = isPlayer1 ? result.player2State.points : result.player1State.points;
    }

    async initializeUserInfo(): Promise<string> {
        let result = await this.accountService.getUserInfo();

        // TODO: Show a message
        if (!result.success) {
            return;
        }

        let userId = result.userInfo.id;

        this.myPlayerId = userId;

        return userId;
    }

    async updateTable(gameId: string) {
        let table = await this.gameService.getGameTable(gameId);

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

    async updateMarks(gameId: string) {
        let marks = await this.gameService.getPlayerMarks(gameId);

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

    async markSurroundingFields(e: MouseEvent, clickedRow: number, clickedCol: number) {
        if (!e.ctrlKey || e.button !== BrowserConstants.RightMouseButtonId) {
            return;
        }

        for (let row = clickedRow - 1; row <= clickedRow + 1; ++row) {
            for (let col = clickedCol - 1; col <= clickedCol + 1; ++col) {

                if (col < 0 || row < 0 || row >= this.gameTable.length || col >= this.gameTable[0].length) {
                    continue;
                }

                if (row === clickedRow && col === clickedCol) {
                    continue;
                }

                let targetMarkType = MarkTypes.Empty;
                let targetField = this.gameTable[row][col];

                if (targetField.markType === targetMarkType) {
                    continue;
                }

                let result = await this.gameService.markField(this.gameId, row, col, targetMarkType);
                if (result.success) {
                    targetField.markType = targetMarkType;
                }
            }
        }
    }

    async makeMove(e: MouseEvent, row: number, col: number) {
        if (e.button !== BrowserConstants.LeftMouseButtonId) {
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
        if (e.button !== BrowserConstants.RightMouseButtonId) {
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

    onTableChanged(notification: GameTableUpdatedNotification) {
        for (let i = 0; i < notification.fieldUpdates.length; ++i) {
            let fieldUpdate = notification.fieldUpdates[i];
            this.gameTable[fieldUpdate.row][fieldUpdate.column].fieldType = fieldUpdate.fieldType;
        }
    }

    onRemainingMinesChanged(notification: RemainingMinesChangedNotification) {
        this.remainingMines = notification.remainingMineCount;
    }

    onGameOver(notification: GameOverNotification) {
        // TODO: Create better notification UX
        if (notification.winnerPlayerId === this.myPlayerId) {
            alert("You won!");
        }
        else if (notification.winnerPlayerId) {
            alert("You lost!");
        }
        else {
            alert("The game ended in a draw.");
        }
    }

    onPointsChanged(notification: PlayerPointsChangedNotification) {
        if (notification.playerId === this.myPlayerId) {
            this.myPoints = notification.points;
        }
        else {
            this.opponentsPoints = notification.points;
        }
    }

    onTurnChanged(notification: TurnChangedNotification) {
        if (notification.playerId === this.myPlayerId) {
            this.isMyTurn = true;
        }
        else {
            this.isMyTurn = false;
        }
    }
}
