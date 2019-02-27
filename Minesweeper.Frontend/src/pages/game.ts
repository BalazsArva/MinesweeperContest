import { autoinject } from "aurelia-framework";
import { NavigationInstruction, RouteConfig } from 'aurelia-router';

import { GameService, GetGameTableResponse } from "services/game-service";
import { FieldTypes } from "../interfaces/field-types";

interface Field {
  fieldType: FieldTypes;
  row: number;
  column: number;
}

@autoinject()
export class Game {

  gameTable: Field[][] = null;
  timerHandle: number = null;
  elapsedSeconds = 0;
  gameId: string = null;

  constructor(private gameService: GameService) {
  }

  async activate(params: any, routeConfig: RouteConfig, navigationInstruction: NavigationInstruction) {
    this.gameId = params.gameId;

    await this.updateTable();

    this.timerHandle = <number><any>setInterval(_ => ++this.elapsedSeconds, 1000);
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

  async makeMove(row: number, col: number) {

    // TODO: Remove eventually
    let playersTemp = ["24341538-9afb-4ae8-b90e-baa85cac57b5", "68dbcce5-eb47-4e1f-928d-4709bc0811e8"];

    let playerId = playersTemp[Math.floor(Math.random() * 2)];

    console.log(playerId);

    await this.gameService.makeMove(this.gameId, playerId, row, col);

    await this.updateTable();
  }
}