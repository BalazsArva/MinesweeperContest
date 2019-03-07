import { HttpClient, json } from 'aurelia-fetch-client';
import configuration from '../../client-configuration';

export class RegistrationService {

    async registerUser(email: string, displayName: string, password: string) {
        const expectedStatus = 200;

        let client = this.createHttpClient();
        let body = { email, password, displayName };
        let request = { method: 'POST', body: json(body) };

        try {
            let httpResponse = await client.fetch('', request);

            if (httpResponse.status === expectedStatus) {
                return;
            }

            alert(`Status: ${httpResponse.status}`);
        } catch (reason) {
            // TODO: Do proper error handling
            alert(reason);
        }
    }

    private createHttpClient() {
        let client = new HttpClient();
        let defaultHeaders = {
            'Accept': 'application/json',
            'X-Requested-With': 'Fetch'
        };

        client.configure(config => {
            config.withBaseUrl(configuration.identityApiBaseUrl)
                .withDefaults({
                    headers: defaultHeaders
                });
        });

        return client;
    }
}