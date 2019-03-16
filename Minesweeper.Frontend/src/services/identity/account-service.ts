import { HttpClient, json } from "aurelia-fetch-client";
import { autoinject } from "aurelia-framework";

import configuration from "../../client-configuration";

@autoinject()
export class AccountService {

    constructor() {
    }

    async getUserInfo(): Promise<GetUserInfoResult> {
        const loggedInStatus = 200;
        const notLoggedInStatus = 401;

        let client = this.createHttpClient();
        let request = { method: "GET", credentials: "include" };

        try {
            let httpResponse = await client.fetch("", request);

            if (httpResponse.status === loggedInStatus) {
                // TODO: Populate user info
                return { success: true, isLoggedIn: true };
            }

            if (httpResponse.status === notLoggedInStatus) {
                return { success: true, isLoggedIn: false };
            }

            // TODO: Include more details
            return {
                success: false,
                errorMessage: "Getting user info failed."
            };
        } catch (reason) {
            return {
                success: false,
                errorMessage: "A network error occured when trying to get the user information. Please try again later."
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

export interface GetUserInfoResult {
    success: boolean;
    isLoggedIn?: boolean;
    errorMessage?: string;
    userInfo?: UserInfo;
}

export interface UserInfo {
    // TODO: Implement
}
