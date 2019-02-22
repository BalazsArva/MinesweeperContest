import { bindable } from 'aurelia-framework';
import { Difficulty } from '../../interfaces/difficulty';

export class DifficultySelector {
  @bindable difficulty: Difficulty = null;
}

