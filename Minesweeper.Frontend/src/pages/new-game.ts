import { bindable } from 'aurelia-framework';
import { Difficulty } from '../interfaces/difficulty';

export class NewGame {

  @bindable isSelected: boolean = false;
  @bindable displayName: string = null;
  @bindable selectedDifficulty: Difficulty = null;
  @bindable difficulties: Difficulty[] = [
    { width: 10, height: 10, mines: 10, title: 'Small' },
    { width: 15, height: 25, mines: 40, title: 'Medium' },
    { width: 16, height: 32, mines: 50, title: 'Large' }
  ];

  constructor() {
  }

  selectDifficulty(newDifficulty: Difficulty) {
    this.selectedDifficulty = newDifficulty;
  }
}
