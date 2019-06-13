import { bindable, autoinject } from "aurelia-framework";
import { Difficulty } from "../interfaces/difficulty";
import { GameService } from "services/game-service";

@autoinject
export class NewGame {

    invitedPlayerId: string = null;
    @bindable selectedDifficulty: Difficulty = null;
    @bindable customDifficulty: Difficulty = { width: 10, height: 10, mines: 10, title: "Custom" };
    @bindable selectableDifficulties: Difficulty[] = [
        { width: 10, height: 10, mines: 10, title: "Small" },
        { width: 15, height: 25, mines: 40, title: "Medium" },
        { width: 16, height: 32, mines: 50, title: "Large" }
    ];

    constructor(private gameService: GameService) {
    }

    async create() {
        let diff = this.selectedDifficulty;
        let invitedPlayerId = this.invitedPlayerId;

        await this.gameService.createGame(invitedPlayerId, diff.height, diff.width, diff.mines);
    }
}
