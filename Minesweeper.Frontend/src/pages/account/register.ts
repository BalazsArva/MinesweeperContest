import { autoinject } from 'aurelia-framework';
import { RegistrationService } from 'services/identity/registration-service';

@autoinject
export class Register {
    email = "";
    password = "";

    constructor(private registrationService: RegistrationService) {
    }

    async register() {
        let email = this.email;
        let password = this.password;

        await this.registrationService.registerUser(email, password);
    }
}