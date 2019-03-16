import { autoinject } from "aurelia-framework";
import { NavigationInstruction, RouteConfig } from 'aurelia-router';
import { EventAggregator } from 'aurelia-event-aggregator';

import { GameService, MarkTypes } from "services/game-service";
import { FieldTypes } from "../interfaces/field-types";
import { GameHubSignalRService } from "services/game-hub-signalr-service";

interface Field {
    fieldType: FieldTypes;
    row: number;
    column: number;
}

@autoinject()
export class Game {

    gameTable: Field[][] = null;
    marks: MarkTypes[][] = null;
    timerHandle: number = null;
    elapsedSeconds = 0;
    gameId: string = null;

    constructor(private eventAggregator: EventAggregator, private gameService: GameService, private gameHubService: GameHubSignalRService) {
    }

    async activate(params: any, routeConfig: RouteConfig, navigationInstruction: NavigationInstruction) {
        let gameId = <string>params.gameId;

        this.gameId = gameId;

        await this.updateTable();
        await this.updateMarks();
        await this.gameHubService.connect(gameId);

        // TODO: Consider interrupted games 
        this.timerHandle = <number><any>setInterval(_ => ++this.elapsedSeconds, 1000);
        this.eventAggregator.subscribe(`games:${gameId}:tableChanged`, notification => {
            let tmp: { table: FieldTypes[][] } = notification;

            let newTableState = [];
            for (let row = 0; row < tmp.table.length; ++row) {
                newTableState.push([]);

                for (let col = 0; col < tmp.table[row].length; ++col) {
                    newTableState[row][col] = {
                        fieldType: tmp.table[row][col],
                        column: col,
                        row: row
                    };
                }
            }

            this.gameTable = newTableState;
        });
    }

    async updateTable() {
        let table = await this.gameService.getGameTable(this.gameId);

        let newTableState = [];
        for (let row = 0; row < table.visibleTable.length; ++row) {
            newTableState.push([]);

            for (let col = 0; col < table.visibleTable[row].length; ++col) {
                newTableState[row][col] = {
                    fieldType: table.visibleTable[row][col],
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

        let newMarksState = [];
        for (let row = 0; row < this.gameTable.length; ++row) {
            newMarksState.push([]);

            for (let col = 0; col < this.gameTable[row].length; ++col) {
                newMarksState[row][col] = marks.marks[row][col];
            }
        }

        this.marks = newMarksState;
    }

    async clickField(e: MouseEvent, row: number, col: number) {
        if (e.button === 0) {
            return await this.makeMove(e, row, col);
        }
        else if (e.button === 2) {
            return await this.markField(e, row, col);
        }
    }

    async makeMove(e: MouseEvent, row: number, col: number) {
        // Prevent moving on marked field
        if (this.marks[row][col] !== MarkTypes.None) {
            return;
        }

        await this.gameService.makeMove(this.gameId, row, col);

        e.preventDefault();
        return false;
    }

    async markField(e: MouseEvent, row: number, col: number) {
        let targetMarkType: MarkTypes;
        let currentMarkType = this.marks[row][col];

        if (currentMarkType === MarkTypes.None) {
            targetMarkType = MarkTypes.Empty;
        }
        else if (currentMarkType === MarkTypes.Empty) {
            targetMarkType = MarkTypes.Unknown;
        }
        else {
            targetMarkType = MarkTypes.None;
        }

        this.marks[row][col] = targetMarkType;

        // TODO: Error handling
        await this.gameService.markField(this.gameId, row, col, targetMarkType);

        // TODO: Don't update the whole marks matrix all the time
        await this.updateMarks();

        e.preventDefault();
        return false;
    }

    contextMenu(e: MouseEvent) {
        e.preventDefault();
        return false;
    }
}
