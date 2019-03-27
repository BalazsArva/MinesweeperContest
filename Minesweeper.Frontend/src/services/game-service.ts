import { HttpClient, json } from "aurelia-fetch-client";
import { autoinject } from "aurelia-framework";

import { FieldTypes } from "../interfaces/field-types";

const apiUrl = "https://localhost:5001/api/games/"

// TODO: Implement proper error handling
@autoinject()
export class GameService {

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

        let body = { column, row };
        let request = { method: "post", body: json(body), credentials: "include" };

        try {
            let httpResponse = await client.fetch(`${gameId}/make-move`, request);

            // TODO: Do proper error handling
            if (!httpResponse.ok) {
                throw Error("Unexpected status code: " + httpResponse.status);
            }
        }
        catch (reason) {
            console.log(reason);
        }
    }

    async markField(gameId: string, row: number, column: number, markType: MarkTypes): Promise<MarkFieldResult> {
        let client = this.createHttpClient();

        let body = { column, row, markType };
        let request = { method: "post", body: json(body), credentials: "include" };

        try {
            let httpResponse = await client.fetch(`${gameId}/mark-field`, request);

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

        let request = { method: "POST", credentials: "include" };

        client.configure(config => {
            config.withBaseUrl(apiUrl)
                .withDefaults({
                    headers: defaultHeaders
                });
        });

        try {
            let httpResponse = await client.fetch(`${gameId}/player2`, request);

            // TODO: Do proper error handling
            if (!httpResponse.ok) {
                throw Error("Unexpected status code: " + httpResponse.status);
            }
        }
        catch (reason) {
            console.log(reason);
        }
    }

    async createGame(invitedPlayerId: string, tableRows: number, tableColumns: number, mineCount: number): Promise<void> {
        let client = new HttpClient();
        let defaultHeaders = {
            "Accept": "application/json",
            "X-Requested-With": "Fetch"
        };

        let body = { tableRows, tableColumns, mineCount, invitedPlayerId };
        let request = { method: "POST", credentials: "include", body: json(body) };

        client.configure(config => {
            config.withBaseUrl(apiUrl)
                .withDefaults({
                    headers: defaultHeaders
                });
        });

        try {
            let httpResponse = await client.fetch("", request);

            // TODO: Do proper error handling
            if (!httpResponse.ok) {
                throw Error("Unexpected status code: " + httpResponse.status);
            }
        }
        catch (reason) {
            console.log(reason);
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

export interface GetGameTableResponse {
    visibleTable: FieldTypes[][];
}

export interface GetPlayerMarksResponse {
    success: boolean;
    errorMessage?: string;
    marks?: MarkTypes[][];
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
