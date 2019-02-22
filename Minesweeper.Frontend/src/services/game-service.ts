//import 'fetch';
import { HttpClient, json } from 'aurelia-fetch-client';
import { autoinject } from 'aurelia-framework';

const apiUrl = 'https://localhost:44382/api/games/'

@autoinject()
export class GameService {
    async getGameTable(gameId: string): Promise<void> {
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

        try {
            await client.fetch(`${gameId}/table`, { method: 'get' })
        }
        catch (reason) {
            console.log(reason);
        }
    }
}