import { autoinject } from "aurelia-framework";
import { GameService } from "services/game-service";

enum FieldTypes {
  Unknown,
  Player1FoundMine,
  Player2FoundMine,
  MinesAround0,
  MinesAround1,
  MinesAround2,
  MinesAround3,
  MinesAround4,
  MinesAround5,
  MinesAround6,
  MinesAround7,
  MinesAround8
}

@autoinject()
export class Game {

  gameTable: FieldTypes[][] = null;
  timerHandle: number = null;
  elapsedSeconds = 0;

  constructor(private gameService: GameService) {
  }

  async activate() {
    const rows = 16;
    const cols = 24;

    this.gameTable = [];
    let temp = [
      FieldTypes.Unknown,
      FieldTypes.Player1FoundMine,
      FieldTypes.Player2FoundMine,
      FieldTypes.MinesAround0,
      FieldTypes.MinesAround1,
      FieldTypes.MinesAround2,
      FieldTypes.MinesAround3,
      FieldTypes.MinesAround4,
      FieldTypes.MinesAround5,
      FieldTypes.MinesAround6,
      FieldTypes.MinesAround7,
      FieldTypes.MinesAround8
    ]

    for (let row = 0; row < rows; ++row) {
      this.gameTable.push([]);

      for (let col = 0; col < cols; ++col) {
        this.gameTable[row][col] = temp[(row * col) % temp.length];
      }
    }

    //await this.gameService.getGameTable('2537e8e7-89e1-4747-b1ed-c6538ee56f03');

    this.timerHandle = <number><any>setInterval(_ => ++this.elapsedSeconds, 1000);
  }

}