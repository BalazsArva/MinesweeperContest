import { HttpClient, json } from "aurelia-fetch-client";
import { autoinject } from "aurelia-framework";

const apiUrl = "https://localhost:5001/api/lobby/"

@autoinject()
export class LobbyService {
    async getAvailableGames(page: number, pageSize: number): Promise<GetAvailableGamesResult> {
        const expectedStatus = 200;

        let client = this.createHttpClient();
        let request = { method: "GET", credentials: "include" };
        let url = `?page=${page}&pageSize=${pageSize}`;

        try {
            let httpResponse = await client.fetch(url, request);

            if (httpResponse.status === expectedStatus) {
                let result = <GetAvailableGamesResponse>(await httpResponse.json());

                return {
                    success: true,
                    response: result
                };
            }

            // TODO: Include more details
            return {
                success: false,
                errorMessage: "Retrieving the available games failed."
            };
        }
        catch (reason) {
            return {
                success: false,
                errorMessage: "A network error occured when trying to retrieve the available games. Please try again later."
            };
        }
    }

    async getPlayersGames(page: number, pageSize: number): Promise<GetPlayersGamesResult> {
        const expectedStatus = 200;

        let client = this.createHttpClient();
        let request = { method: "GET", credentials: "include" };
        let url = `my-games?page=${page}&pageSize=${pageSize}`;

        try {
            let httpResponse = await client.fetch(url, request);

            if (httpResponse.status === expectedStatus) {
                let result = <GetPlayersGamesResponse>(await httpResponse.json());

                return {
                    success: true,
                    response: result
                };
            }

            // TODO: Include more details
            return {
                success: false,
                errorMessage: "Retrieving the player's games failed."
            };
        }
        catch (reason) {
            return {
                success: false,
                errorMessage: "A network error occured when trying to retrieve the player's games. Please try again later."
            };
        }
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

export interface GetAvailableGamesResult {
    success: boolean;
    errorMessage?: string;
    response?: GetAvailableGamesResponse;
}

export interface GetAvailableGamesResponse {
    total: number;
    availableGames: AvailableGame[]
}

export interface AvailableGame {
    gameId: string;
    hostPlayerId: string;
    hostPlayerDisplayName: string;
    rows: number;
    columns: number;
    mines: number;
}

export interface GetPlayersGamesResult {
    success: boolean;
    errorMessage?: string;
    response?: GetPlayersGamesResponse;
}

export interface GetPlayersGamesResponse {
    total: number;
    playersGames: PlayersGame[]
}

export interface PlayersGame {
    gameId: string;
    otherPlayerId: string;
    otherPlayerDisplayName: string;
    rows: number;
    columns: number;
    mines: number;
}
