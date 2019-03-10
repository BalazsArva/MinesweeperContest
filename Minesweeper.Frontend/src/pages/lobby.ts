import { autoinject } from "aurelia-framework";

import { LobbyService, GetAvailableGamesResult, GetAvailableGamesResponse, AvailableGame } from "services/lobby-service";
import { GameService } from "services/game-service";
import { Router } from "aurelia-router";


@autoinject
export class Lobby {
    joinGameId: string = null;
    total: number = 0;
    availableGames: AvailableGame[] = [];

    constructor(private lobbyService: LobbyService, private gameService: GameService, private router: Router) {
    }

    async activate() {
        await this.refreshGames();
    }

    async refreshGames() {
        let result = await this.lobbyService.getAvailableGames(1, 10);

        // TODO: Validation, display errors
        if (result.success) {
            this.total = result.response.total;
            this.availableGames = result.response.availableGames;
        }
    }

    async joinGame(gameId: string) {
        // TODO: Validate success before redirecting
        await this.gameService.joinGame(gameId);

        return this.router.navigateToRoute("game", { gameId });
    }
}
