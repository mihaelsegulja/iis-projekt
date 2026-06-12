import { Component, input } from '@angular/core';

@Component({
  selector: 'app-error',
  standalone: true,
  template: `
    @if (message()) {
      <div class="error">{{ message() }}</div>
    }
  `,
  styles: `
    .error {
      color: #f44336;
      font-size: 0.875rem;
      margin: 8px 0;
    }
  `,
})
export class ErrorComponent {
  readonly message = input<string | null>(null);
}
