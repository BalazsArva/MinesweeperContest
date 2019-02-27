export class Lobby {
  availableGames: number = 0;
  joinGameId: string = null;
  joinGameDisplayName: String = null;

  constructor() {
  }

  joinGame() {
    alert('Joining game...')
  }
}