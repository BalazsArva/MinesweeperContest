import { autoinject } from "aurelia-framework";
import { RegistrationService } from "services/identity/registration-service";
import { Router } from "aurelia-router";

@autoinject()
export class Register {
    email = "";
    displayName = "";
    password = "";
    passwordConfirmation = "";
    generalErrorMessage = "";

    isBusy = false;

    constructor(private registrationService: RegistrationService, private router: Router) {
    }

    async register() {
        if (this.isBusy) {
            return;
        }

        let email = this.email;
        let password = this.password;
        let passwordConfirmation = this.passwordConfirmation;
        let displayName = this.displayName;

        if (password !== passwordConfirmation) {
            // TODO: Implement proper validation feedback
            this.generalErrorMessage = "Password and its confirmation must be the same.";

            return;
        }

        this.isBusy = true;

        let result = await this.registrationService.registerUser(email, displayName, password);
        if (result.success) {
            this.isBusy = false;
            this.resetFields();

            return this.router.navigateToRoute("login");
        }

        // TODO: Display other possible server error messages
        this.generalErrorMessage = result.errorMessage;

        this.isBusy = false;
    }

    resetFields() {
        this.email = "";
        this.password = "";
        this.passwordConfirmation = "";
        this.displayName = "";
    }
}
