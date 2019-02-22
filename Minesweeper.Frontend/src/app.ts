import { Aurelia } from 'aurelia-framework';
import { PLATFORM } from 'aurelia-pal';
import { Router, RouterConfiguration, NavigationInstruction, PipelineStep, Next, Redirect } from 'aurelia-router';
import { inject } from 'aurelia-framework';
import environment from './environment';

export class App {
  router: Router = null;
  message = "Hello world";

  configureRouter(config: RouterConfiguration, router: Router) {
    config.title = 'Minesweeper Contest';

    config.map([
      {
        route: '',
        moduleId: PLATFORM.moduleName('./pages/home'),
        title: 'Home',
        name: 'home'
      },
      {
        route: 'games/new',
        moduleId: PLATFORM.moduleName('./pages/new-game'),
        title: 'New game',
        name: 'newgame'
      },
      {
        route: 'games/game',
        moduleId: PLATFORM.moduleName('./pages/game'),
        title: 'Game',
        name: 'game'
      }
    ]);

    config.fallbackRoute('');

    this.router = router;
  }
}
