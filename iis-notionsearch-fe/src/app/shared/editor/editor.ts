import { Component, input, output } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-editor',
  standalone: true,
  imports: [FormsModule],
  template: `
    <textarea
      class="editor"
      [class.readonly]="isReadonly()"
      [ngModel]="code()"
      (ngModelChange)="codeChange.emit($event)"
      [readonly]="isReadonly()"
      [placeholder]="placeholder()"
      [style.min-height]="minHeight()"
      spellcheck="false"
    ></textarea>
  `,
  styles: `
    .editor {
      width: 100%;
      font-family: 'Roboto Mono', 'Consolas', 'Courier New', monospace;
      font-size: 14px;
      padding: 12px;
      border: 1px solid rgba(0,0,0,0.12);
      border-radius: 4px;
      resize: vertical;
      tab-size: 2;
      line-height: 1.5;
      box-sizing: border-box;
      background: #fafafa;
    }
    .readonly {
      background: #f0f0f0;
      cursor: default;
    }
  `,
})
export class EditorComponent {
  readonly code = input('');
  readonly isReadonly = input(false);
  readonly placeholder = input('');
  readonly minHeight = input('200px');
  readonly codeChange = output<string>();

  onInput(event: Event): void {
    this.codeChange.emit((event.target as HTMLTextAreaElement).value);
  }
}
