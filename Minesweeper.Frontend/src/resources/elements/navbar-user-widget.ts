import { EventAggregator, Subscription } from "aurelia-event-aggregator";
import { AccountServiceEvents, AuthService } from "services/identity/auth-service";
import { autoinject } from "aurelia-framework";
import { Router } from "aurelia-router";

@autoinject()
export class NavbarUserWidget {
    private loggedInSubsciption: Subscription;
    private loggedOutSubsciption: Subscription;

    isLoggedIn = false;

    constructor(private authService: AuthService, private eventAggregator: EventAggregator, private router: Router) {
    }

    attached() {
        this.loggedInSubsciption = this.eventAggregator.subscribe(AccountServiceEvents.LoggedIn, () => this.loggedIn.call(this));
        this.loggedOutSubsciption = this.eventAggregator.subscribe(AccountServiceEvents.LoggedOut, () => this.loggedOut.call(this));

        // TODO: Fetch logged in status (e.g. when a cookie is set and the app is loaded in a new tab)
    }

    detached() {
        this.loggedInSubsciption.dispose();
        this.loggedOutSubsciption.dispose();
    }

    loggedIn() {
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