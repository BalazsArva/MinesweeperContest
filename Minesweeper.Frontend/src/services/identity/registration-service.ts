import { HttpClient, json } from "aurelia-fetch-client";
import configuration from "../../client-configuration";

export class RegistrationService {

    async registerUser(email: string, displayName: string, password: string): Promise<RegistrationResult> {
        const expectedStatus = 200;

        let client = this.createHttpClient();
        let body = { email, password, displayName };
        let request = { method: "POST", body: json(body) };

        try {
            let httpResponse = await client.fetch("", request);

            if (httpResponse.status === expectedStatus) {
                return { success: true };
            }

            // TODO: Include more details
            return {
                success: false,
                errorMessage: "Registration failed."
            };
        } catch (reason) {
            return {
                success: false,
                errorMessage: "A network error occured when trying to register you. Please try again later."
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
            config.withBaseUrl(configuration.identityApiBaseUrl)
                .withDefaults({
                    headers: defaultHeaders
                });
        });

        return client;
    }
}

export interface RegistrationResult {
    success: boolean;
    errorMessage?: string;
}
