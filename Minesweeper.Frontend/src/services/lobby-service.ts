import { HttpClient, json } from "aurelia-fetch-client";
import { autoinject } from "aurelia-framework";

const apiUrl = "https://localhost:5001/api/lobby/"

@autoinject()
export class LobbyService {

    async getAnonymousPlayerId(): Promise<GetAnonymousPlayerIdResponse> {
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

        try {
            let httpResponse = await client.fetch("anonymous-id", { method: "get" });

            let result = await httpResponse.json();

            return <GetAnonymousPlayerIdResponse>result;
        }
        catch (reason) {
            console.log(reason);
        }
    }

    async joinGame(gameId: string, playerId: string, entryToken: string, displayName: string): Promise<void> {
        let client = new HttpClient();
        let defaultHeaders = {
            'Accept': 'application/json',
            'X-Requested-With': 'Fetch'
        };

        let body = { playerId, entryToken, displayName };
        let request = { method: 'post', body: json(body) };

        client.configure(config => {
            config.withBaseUrl(apiUrl)
                .withDefaults({
                    headers: defaultHeaders
                });
        });

        try {
            let httpResponse = await client.fetch(`games/${gameId}`, request);

            // TODO: Do proper error handling
            if (!httpResponse.ok) {
                throw Error('Unexpected status code: ' + httpResponse.status);
            }
        }
        catch (reason) {
            console.log(reason);
        }
    }

    async createGame(playerId: string, displayName: string, tableRows: number, tableColumns: number, mineCount: number): Promise<void> {
        let client = new HttpClient();
        let defaultHeaders = {
            'Accept': 'application/json',
            'X-Requested-With': 'Fetch'
        };

        let body = { playerId, displayName, tableRows, tableColumns, mineCount };
        let request = { method: 'post', body: json(body) };

        client.configure(config => {
            config.withBaseUrl(apiUrl)
                .withDefaults({
                    headers: defaultHeaders
                });
        });

        try {
            let httpResponse = await client.fetch("games", request);

            // TODO: Do proper error handling
            if (!httpResponse.ok) {
                throw Error('Unexpected status code: ' + httpResponse.status);
            }
        }
        catch (reason) {
            console.log(reason);
        }
    }
}

export interface GetAnonymousPlayerIdResponse {
    playerId: string;
}