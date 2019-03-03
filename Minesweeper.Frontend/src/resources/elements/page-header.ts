import { bindable } from 'aurelia-framework';

export class PageHeader {
  @bindable title: string = null;
  @bindable subtitle: string = null;
}
