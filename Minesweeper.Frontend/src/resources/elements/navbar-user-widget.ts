import { EventAggregator, Subscription } from "aurelia-event-aggregator";
import { AccountServiceEvents, AuthService } from "services/identity/auth-service";
import { autoinject } from "aurelia-framework";
import { Router } from "aurelia-router";
import { AccountService } from "services/identity/account-service";

@autoinject()
export class NavbarUserWidget {
    private loggedInSubsciption: Subscription;
    private loggedOutSubsciption: Subscription;

    isLoggedIn = false;

    constructor(private authService: AuthService, private accountSerivce: AccountService, private eventAggregator: EventAggregator, private router: Router) {
    }

    async attached() {
        this.loggedInSubsciption = this.eventAggregator.subscribe(AccountServiceEvents.LoggedIn, () => this.loggedIn.call(this));
        this.loggedOutSubsciption = this.eventAggregator.subscribe(AccountServiceEvents.LoggedOut, () => this.loggedOut.call(this));

        let getUserInfoResult = await this.accountSerivce.getUserInfo();
        if (getUserInfoResult.success) {
            this.isLoggedIn = getUserInfoResult.isLoggedIn || false;
        } else {
            // TODO: Display error
            this.isLoggedIn = false;
        }
    }

    detached() {
        this.loggedInSubsciption.dispose();
        this.loggedOutSubsciption.dispose();
    }

    loggedIn() {
        // TODO: Fetch user info
        this.isLoggedIn = true;
    }

    loggedOut() {
        this.isLoggedIn = false;
    }

    async logOut() {
        let result = await this.authService.logOut();
        if (result.success) {
            return this.router.navigateToRoute("home");
        }

        // TODO: Implement more user-friendly feedback
        alert(result.errorMessage);
    }
}
