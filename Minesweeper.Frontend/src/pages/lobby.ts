import { autoinject } from "aurelia-framework";

import { LobbyService } from "services/lobby-service";

const PlayerIdLocalStorageKey = "playerId";
const DisplayNameLocalStorageKey = "displayName";

@autoinject
export class Lobby {
  availableGames: number = 0;
  joinGameId: string = null;
  joinGameEntryToken: string = null;
  displayName: string = null;

  constructor(private lobbyService: LobbyService) {
  }

  activate() {
    this.displayName = localStorage[DisplayNameLocalStorageKey];
  }

  async joinGame() {
    let playerId = localStorage[PlayerIdLocalStorageKey];
    let displayName = localStorage[DisplayNameLocalStorageKey];

    await this.lobbyService.joinGame(this.joinGameId, playerId, this.joinGameEntryToken, displayName);
  }

  async saveUserInfo() {

    if (!this.displayName) {
      // TODO: Show alert 
      return;
    }

    let playerId = localStorage[PlayerIdLocalStorageKey];

    if (!playerId) {
      let response = await this.lobbyService.getAnonymousPlayerId();

      playerId = response.playerId;
      localStorage[PlayerIdLocalStorageKey] = playerId;
    }

    localStorage[DisplayNameLocalStorageKey] = this.displayName
  }
}