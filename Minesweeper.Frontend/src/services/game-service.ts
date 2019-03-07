import { HttpClient, json } from 'aurelia-fetch-client';
import { autoinject } from 'aurelia-framework';

import { FieldTypes } from "../interfaces/field-types";

const apiUrl = 'https://localhost:5001/api/games/'

// TODO: Implement proper error handling
@autoinject()
export class GameService {

    async getGameTable(gameId: string): Promise<GetGameTableResponse> {
        let client = this.createHttpClient();

        try {
            let httpResponse = await client.fetch(`${gameId}/table`, { method: 'get' });

            let result = await httpResponse.json();

            return <GetGameTableResponse>result;
        }
        catch (reason) {
            console.log(reason);
        }
    }

    async makeMove(gameId: string, playerId: string, row: number, column: number): Promise<void> {
        let client = this.createHttpClient();

        let body = { column, row, playerId };
        let request = { method: 'post', body: json(body) };

        try {
            let httpResponse = await client.fetch(`${gameId}/movement`, request);

            // TODO: Do proper error handling
            if (!httpResponse.ok) {
                throw Error('Unexpected status code: ' + httpResponse.status);
            }
        }
        catch (reason) {
            console.log(reason);
        }
    }

    private createHttpClient(): HttpClient {
        let client = new HttpClient();
        let defaultHeaders = {
            'Accept': 'application/json',
            'X-Requested-With': 'Fetch'
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