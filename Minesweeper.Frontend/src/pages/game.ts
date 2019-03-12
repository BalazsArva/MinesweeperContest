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
            await this.makeMove(row, col);
        }
        else if (e.button === 2) {
            await this.markField(row, col);
        }
    }

    async makeMove(row: number, col: number) {
        await this.gameService.makeMove(this.gameId, row, col);
    }

    async markField(row: number, col: number) {
        let targetMarkType: MarkTypes;
        let currentMarkType = this.marks[row][col];

        if (currentMarkType === "None") {
            targetMarkType = "Empty";
        }
        else if (currentMarkType === "Empty") {
            targetMarkType = "Unknown";
        }
        else {
            targetMarkType = "None";
        }

        this.marks[row][col] = targetMarkType;

        // TODO: Error handling
        await this.gameService.markField(this.gameId, row, col, targetMarkType);
    }
}
