import { HttpClient, json } from "aurelia-fetch-client";
import configuration from "../../client-configuration";
import { autoinject } from "aurelia-framework";
import { EventAggregator } from "aurelia-event-aggregator";

@autoinject()
export class AuthService {

    constructor(private eventAggregator: EventAggregator) {
    }

    async logIn(email: string, password: string): Promise<LoginResult> {
        const expectedStatus = 200;

        let client = this.createHttpClient();
        let body = { email, password };
        let request = { method: "POST", body: json(body) };

        try {
            let httpResponse = await client.fetch("", request);

            if (httpResponse.status === expectedStatus) {
                this.eventAggregator.publish(AccountServiceEvents.LoggedIn);

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

    async logOut(): Promise<LogoutResult> {
        const expectedStatus = 200;

        let client = this.createHttpClient();
        let request = { method: "DELETE", credentials: "include" };

        try {
            let httpResponse = await client.fetch("", request);

            if (httpResponse.status === expectedStatus) {
                this.eventAggregator.publish(AccountServiceEvents.LoggedOut);

                return { success: true };
            }

            // TODO: Include more details
            return {
                success: false,
                errorMessage: "Logout failed."
            };
        } catch (reason) {
            return {
                success: false,
                errorMessage: "A network error occured when trying to log you out. Please try again later."
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

export interface LogoutResult {
    success: boolean;
    errorMessage?: string;
}

export enum AccountServiceEvents {
    LoggedIn = "Account:General:LoggedIn",
    LoggedOut = "Account:General:LoggedOut"
}
