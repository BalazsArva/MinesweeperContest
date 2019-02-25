import { FrameworkConfiguration, PLATFORM } from 'aurelia-framework';

export function configure(config: FrameworkConfiguration) {
  config.globalResources([
    PLATFORM.moduleName('./elements/navbar'),
    PLATFORM.moduleName('./elements/page-header'),
    PLATFORM.moduleName('./elements/difficulty-selector'),
    PLATFORM.moduleName('./elements/game-stat'),
    PLATFORM.moduleName('./value-converters/seconds-to-timespan'),
  ]);
}
