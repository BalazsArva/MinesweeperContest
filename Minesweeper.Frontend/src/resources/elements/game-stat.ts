import { bindable } from 'aurelia-framework';

export class GameStat {
  @bindable stat: string = null;
  @bindable title: string = null;
  @bindable colorClass: string = 'info';
}

