import { autoinject } from "aurelia-framework";

import { LobbyService, GetAvailableGamesResult, GetAvailableGamesResponse, AvailableGame, PlayersGame } from "services/lobby-service";
import { GameService } from "services/game-service";
import { Router } from "aurelia-router";


@autoinject
export class Lobby {
    joinGameId: string = null;

    availableGamesTotal: number = 0;
    availableGamesLoading: boolean = false;
    availableGames: AvailableGame[] = [];

    playersGamesTotal: number = 0;
    playersGamesLoading: boolean = false;
    playersGames: PlayersGame[] = [];

    constructor(private lobbyService: LobbyService, private gameService: GameService, private router: Router) {
    }

    async attached() {
        await this.refreshGames();
    }

    async refreshGames() {
        let availableGamesPromise = this.updateAvailableGames();
        let playersGamesPromise = this.updatePlayersGames();

        await Promise.all([availableGamesPromise, playersGamesPromise]);
    }

    async updateAvailableGames() {
        if (this.availableGamesLoading) {
            return;
        }

        this.availableGamesLoading = true;
        // TODO: Include pagination
        let result = await this.lobbyService.getAvailableGames(1, 10);

        // TODO: Validation, display errors
        if (result.success) {
            this.availableGamesTotal = result.response.total;
            this.availableGames = result.response.availableGames;
        }

        this.availableGamesLoading = false;
    }

    async updatePlayersGames() {
        if (this.playersGamesLoading) {
            return;
        }

        this.playersGamesLoading = true;
        // TODO: Include pagination
        let result = await this.lobbyService.getPlayersGames(1, 10);

        // TODO: Validation, display errors
        if (result.success) {
            this.playersGamesTotal = result.response.total;
            this.playersGames = result.response.playersGames;
        }

        this.playersGamesLoading = false;
    }

    async joinGame(gameId: string) {
        // TODO: Validate success before redirecting
        await this.gameService.joinGame(gameId);

        return this.router.navigateToRoute("game", { gameId });
    }

    async goToGame(gameId: string) {
        return this.router.navigateToRoute("game", { gameId });
    }
}
