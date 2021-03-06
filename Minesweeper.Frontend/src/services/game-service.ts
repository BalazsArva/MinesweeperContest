import { HttpClient, json } from "aurelia-fetch-client";
import { autoinject } from "aurelia-framework";

import { FieldTypes } from "../interfaces/field-types";
import { HttpResponse } from "@aspnet/signalr";

const apiUrl = "https://localhost:5001/api/games/"

// TODO: Implement proper error handling
@autoinject()
export class GameService {

    async getGameState(gameId: string): Promise<GetGameStateResponse> {
        let client = this.createHttpClient();

        // TODO: Proper error handling
        try {
            let httpResponse = await client.fetch(`${gameId}`, { method: "GET", credentials: "include" });

            let result = await httpResponse.json();

            return <GetGameStateResponse>result;
        }
        catch (reason) {
            console.log(reason);
        }
    }

    async getGameTable(gameId: string): Promise<GetGameTableResponse> {
        let client = this.createHttpClient();

        // TODO: Proper error handling
        try {
            let httpResponse = await client.fetch(`${gameId}/table`, { method: "GET", credentials: "include" });

            let result = await httpResponse.json();

            return <GetGameTableResponse>result;
        }
        catch (reason) {
            console.log(reason);
        }
    }

    async getPlayerMarks(gameId: string): Promise<GetPlayerMarksResponse> {
        let client = this.createHttpClient();

        try {
            let httpResponse = await client.fetch(`${gameId}/player-marks`, { method: "GET", credentials: "include" });

            if (httpResponse.ok) {
                let result = <{ marks: MarkTypes[][] }>(await httpResponse.json());

                return {
                    success: true,
                    marks: result.marks
                };
            }

            // TODO: Provide more details
            return {
                success: false,
                errorMessage: "Failed to fetch your field marks. Status code: " + httpResponse.status
            }
        }
        catch (reason) {
            return {
                success: false,
                errorMessage: "A network error occured when trying to fetch your field marks. Please try again later."
            };
        }
    }

    async makeMove(gameId: string, row: number, column: number): Promise<void> {
        let client = this.createHttpClient();

        let httpResponse: Response;
        let body = { column, row };
        let request = { method: "post", body: json(body), credentials: "include" };

        try {
            httpResponse = await client.fetch(`${gameId}/moves`, request);
        }
        catch{
            throw Error("Failed to make move due to a connection error. Please try again later.");
        }

        if (!httpResponse.ok) {
            throw Error("Failed to make move. Unexpected status code: " + httpResponse.status);
        }
    }

    async markField(gameId: string, row: number, column: number, markType: MarkTypes): Promise<MarkFieldResult> {
        let client = this.createHttpClient();

        let body = { column, row, markType };
        let request = { method: "post", body: json(body), credentials: "include" };

        try {
            let httpResponse = await client.fetch(`${gameId}/player-marks`, request);

            if (httpResponse.ok) {
                return { success: true };
            }

            // TODO: Include more details
            return {
                success: false,
                errorMessage: "Failed to mark field."
            }
        }
        catch (reason) {
            return {
                success: false,
                errorMessage: "A network error occured when trying to set the field mark. Please try again later."
            };
        }
    }

    async joinGame(gameId: string): Promise<void> {
        let client = new HttpClient();
        let defaultHeaders = {
            "Accept": "application/json",
            "X-Requested-With": "Fetch"
        };

        let httpResponse: Response;
        let request = { method: "POST", credentials: "include" };

        client.configure(config => {
            config.withBaseUrl(apiUrl)
                .withDefaults({
                    headers: defaultHeaders
                });
        });

        try {
            httpResponse = await client.fetch(`${gameId}/player2`, request);
        }
        catch (reason) {
            throw Error("Failed to join the game due to a connection error. Please try again later.");
        }

        if (!httpResponse.ok) {
            throw Error("Failed to join the game. Unexpected status code: " + httpResponse.status);
        }
    }

    async createGame(invitedPlayerId: string, tableRows: number, tableColumns: number, mineCount: number): Promise<string> {
        let client = new HttpClient();
        let defaultHeaders = {
            "Accept": "application/json",
            "X-Requested-With": "Fetch"
        };

        let httpResponse: Response;
        let body = { tableRows, tableColumns, mineCount, invitedPlayerId };
        let request = { method: "POST", credentials: "include", body: json(body) };

        client.configure(config => {
            config.withBaseUrl(apiUrl)
                .withDefaults({
                    headers: defaultHeaders
                });
        });

        try {
            httpResponse = await client.fetch("", request);
        }
        catch (e) {
            throw Error("Failed to create game due to a connection error. Please try again later.");
        }

        if (!httpResponse.ok) {
            throw Error("Failed to create the game. Unexpected status code: " + httpResponse.status);
        }

        const gamesUriSegment = "/games/";
        const guidStringLength = 36;

        let gameApiUri = httpResponse.headers.get("Location");
        let indexOfGamesUriSegment = gameApiUri.toLowerCase().indexOf(gamesUriSegment);
        let gameIdIndex = indexOfGamesUriSegment + gamesUriSegment.length;

        return gameApiUri.substr(gameIdIndex, guidStringLength);
    }

    private createHttpClient(): HttpClient {
        let client = new HttpClient();
        let defaultHeaders = {
            "Accept": "application/json",
            "X-Requested-With": "Fetch"
        };

        client.configure(config => {
            config.withBaseUrl(apiUrl)
                .withDefaults({
                    headers: defaultHeaders
                });
        });

        return client;
    }
}

export interface GetGameStateResponse {
    remainingMines: number;
    utcDateTimeStarted?: string;
    player1State: PlayerState;
    player2State: PlayerState;
    nextPlayer: Players;
    winner?: Players;
    status: GameStatus;
}

export interface PlayerState {
    points: number;
    playerId: string;
}

export interface GetGameTableResponse {
    visibleTable: FieldTypes[][];
}

export interface GetPlayerMarksResponse {
    success: boolean;
    errorMessage?: string;
    marks?: MarkTypes[][];
}

export enum Players {
    Player1 = 0,
    Player2 = 1
}

export enum GameStatus {
    NotStarted = 0,
    InProgress = 1,
    Finished = 2
}

export enum MarkTypes {
    None = 0,
    Empty = 1,
    Unknown = 2
}

export interface MarkFieldResult {
    success: boolean;
    errorMessage?: string;
}
