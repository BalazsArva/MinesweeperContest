import { HttpClient, json } from "aurelia-fetch-client";
import { autoinject } from "aurelia-framework";

const apiUrl = "https://localhost:5001/api/lobby/"

@autoinject()
export class LobbyService {
}

export interface GetAnonymousPlayerIdResponse {
    playerId: string;
}