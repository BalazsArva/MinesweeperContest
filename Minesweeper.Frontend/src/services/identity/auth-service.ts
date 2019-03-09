import { HttpClient, json } from "aurelia-fetch-client";
import configuration from "../../client-configuration";

export class AuthService {

    async authorizeUser(email: string, password: string) {
        const expectedStatus = 200;

        let client = this.createHttpClient();
        let request = { method: "GET" };
        let url = "authorize?";
        // let url = "token?";

        url += "client_id=Minesweeper.Frontend&";
        url += "scope=Game&";
        url += "response_type=token&";
        url += "grant_type=password&";
        url += "username=" + encodeURIComponent(email) + "&";
        url += "password=" + encodeURIComponent(password) + "&";
        url += "redirect_uri=http%3A%2F%2Flocalhost%3A9000";

        alert(url)

        try {
            let httpResponse = await client.fetch(url, request);

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
            "Accept": "application/json",
            "X-Requested-With": "Fetch"
        };

        client.configure(config => {
            config.withBaseUrl(configuration.identityConnectBaseUrl)
                .withDefaults({
                    headers: defaultHeaders
                });
        });

        return client;
    }
}
