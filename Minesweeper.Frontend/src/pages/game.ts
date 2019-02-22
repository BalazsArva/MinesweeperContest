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

export class Game {

  gameTable: FieldTypes[][] = null;

  timerHandle: number = null;
  elapsedSeconds = 0;

  constructor() {
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

    this.timerHandle = <number><any>setInterval(_ => ++this.elapsedSeconds, 1000);
  }

}