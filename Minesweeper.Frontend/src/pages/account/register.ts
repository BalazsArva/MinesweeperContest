import { autoinject } from 'aurelia-framework';
import { RegistrationService } from 'services/identity/registration-service';

@autoinject
export class Register {
    email = "";
    displayName = "";
    password = "";
    passwordConfirmation = "";

    constructor(private registrationService: RegistrationService) {
    }

    async register() {
        let email = this.email;
        let password = this.password;
        let passwordConfirmation = this.passwordConfirmation;
        let displayName = this.displayName;

        if (password !== passwordConfirmation) {
            // TODO: Implement proper validation feedback
            alert("Password and its confirmation must be the same.");

            return;
        }

        await this.registrationService.registerUser(email, displayName, password);
    }
}