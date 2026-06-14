import { Component, input, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltip } from "@angular/material/tooltip";

@Component({
  selector: 'app-editor',
  standalone: true,
  imports: [FormsModule, MatIconModule, MatTooltip],
  template: `
    <div class="editor-wrapper">
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
      <button class="copy-btn" (click)="copy()" matTooltip="Copy to clipboard">
        <mat-icon fontIcon="content_copy" />
      </button>
    </div>
  `,
  styles: `
    .editor-wrapper {
      position: relative;
    }
    .editor {
      width: 100%;
      font-family: 'Roboto Mono', monospace;
      font-size: 14px;
      padding: 12px;
      border: 1px solid rgba(0,0,0,0.2);
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
    .copy-btn {
      position: absolute;
      top: 6px;
      right: 6px;
      background: rgba(0,0,0,0.06);
      border: 1px solid rgba(0,0,0,0.12);
      border-radius: 4px;
      cursor: pointer;
      padding: 4px;
      display: flex;
      align-items: center;
      justify-content: center;
      opacity: 0.5;
      transition: opacity 0.15s;
    }
    .copy-btn:hover {
      opacity: 1;
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

  copy(): void {
    navigator.clipboard.writeText(this.code());
  }
}
