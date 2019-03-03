import { bindable, bindingMode } from 'aurelia-framework';
import { Difficulty } from '../../interfaces/difficulty';

export class DifficultySelector {
  @bindable difficulty: Difficulty = null;
  @bindable({ defaultBindingMode: bindingMode.twoWay }) selectedDifficulty: Difficulty = null;
  @bindable inputGroup: string = 'difficulty';

  get isSelected(): boolean {
    return this.difficulty !== null && this.difficulty === this.selectedDifficulty;
  }
}

