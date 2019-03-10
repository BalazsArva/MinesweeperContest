import { autoinject } from "aurelia-framework";

import { LobbyService } from "services/lobby-service";
import { GameService } from "services/game-service";


@autoinject
export class Lobby {
  availableGames: number = 0;
  joinGameId: string = null;

  constructor(private lobbyService: LobbyService, private gameService: GameService) {
  }

  activate() {
  }

  async joinGame() {

    await this.gameService.joinGame(this.joinGameId);
  }
}