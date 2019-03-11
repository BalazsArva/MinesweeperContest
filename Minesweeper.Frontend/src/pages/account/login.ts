import { autoinject } from "aurelia-framework";
import { Router } from "aurelia-router";
import { AuthService } from "services/identity/auth-service";

@autoinject
export class Login {
    email = "";
    password = "";
    generalErrorMessage = "";

    isBusy = false;

    constructor(private authService: AuthService, private router: Router) {
    }

    async login() {
        if (this.isBusy) {
            return;
        }

        let email = this.email;
        let password = this.password;

        this.isBusy = true;

        // TODO: Validation, display errors
        let result = await this.authService.logIn(email, password);
        if (result.success) {
            this.isBusy = false;
            this.resetFields();

            return this.router.navigateToRoute("lobby");
        }

        this.generalErrorMessage = result.errorMessage;

        this.isBusy = false;
    }

    resetFields() {
        this.email = "";
        this.password = "";
        this.generalErrorMessage = "";
    }
}
