import { bindable } from 'aurelia-framework';
import { Router } from 'aurelia-router';

// TODO: Fix responsive view
export class Navbar {
  @bindable router: Router = null;
}
