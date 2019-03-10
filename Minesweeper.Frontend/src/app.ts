import { PLATFORM } from "aurelia-pal";
import { Router, RouterConfiguration } from "aurelia-router";

export class App {
    router: Router = null;

    configureRouter(config: RouterConfiguration, router: Router) {
        config.title = "Minesweeper Contest";

        config.map([
            {
                route: "",
                moduleId: PLATFORM.moduleName("./pages/home"),
                title: "Home",
                name: "home",
                nav: false
            },
            {
                route: "games/new",
                moduleId: PLATFORM.moduleName("./pages/new-game"),
                title: "New game",
                name: "newgame",
                nav: false
            },
            {
                route: "lobby",
                moduleId: PLATFORM.moduleName("./pages/lobby"),
                title: "Games",
                name: "lobby",
                nav: true
            },
            {
                route: "games/game/:gameId",
                moduleId: PLATFORM.moduleName("./pages/game"),
                title: "Game",
                name: "game",
                nav: false
            },
            {
                route: "account/register",
                moduleId: PLATFORM.moduleName("./pages/account/register"),
                title: "Register an account",
                name: "register"
            },
            {
                route: "account/login",
                moduleId: PLATFORM.moduleName("./pages/account/login"),
                title: "Log in",
                name: "login"
            }
        ]);

        config.fallbackRoute("");

        this.router = router;
    }
}
