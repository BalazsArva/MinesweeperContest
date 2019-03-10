import { HttpClient, json } from "aurelia-fetch-client";
import configuration from "../../client-configuration";

export class AuthService {

    async authenticateUser(email: string, password: string): Promise<LoginResult> {
        const expectedStatus = 200;

        let client = this.createHttpClient();
        let body = { email, password };
        let request = { method: "POST", body: json(body), credentials: "include" };

        try {
            let httpResponse = await client.fetch("", request);

            if (httpResponse.status === expectedStatus) {
                // TODO: Emit an event so dependent stuff can update themselves (e.g. user widget in navbar, subscribe to notifications, etc.)
                return { success: true };
            }

            // TODO: Include more details
            return {
                success: false,
                errorMessage: "Login failed."
            };
        } catch (reason) {
            return {
                success: false,
                errorMessage: "A network error occured when trying to log you in. Please try again later."
            };
        }
    }

    private createHttpClient() {
        let client = new HttpClient();
        let defaultHeaders = {
            "Accept": "application/json",
            "X-Requested-With": "Fetch"
        };

        client.configure(config => {
            config.withBaseUrl(configuration.accountApiBaseUrl)
                .withDefaults({
                    headers: defaultHeaders
                });
        });

        return client;
    }
}

export interface LoginResult {
    success: boolean;
    errorMessage?: string;
}
