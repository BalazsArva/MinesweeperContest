import { autoinject } from 'aurelia-framework';
import { Router } from 'aurelia-router';
import { AuthService } from 'services/identity/auth-service';

@autoinject
export class Login {
    email = "";
    password = "";
    generalErrorMessage = "";

    constructor(private authService: AuthService, private router: Router) {
    }

    async login() {
        let email = this.email;
        let password = this.password;

        // TODO: Validation, display errors
        let result = await this.authService.authenticateUser(email, password);
        if (result.success) {
            this.resetFields();
            
            return this.router.navigateToRoute('lobby');
        }

        this.generalErrorMessage = result.errorMessage;
    }

    resetFields() {
        this.email = "";
        this.password = "";
        this.generalErrorMessage = "";
    }
}
