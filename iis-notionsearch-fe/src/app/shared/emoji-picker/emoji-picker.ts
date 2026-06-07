import { Component, computed, input, output, signal } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { FormsModule } from '@angular/forms';

interface EmojiCategory {
  name: string;
  icon: string;
  items: string[];
}

const CATEGORIES: EmojiCategory[] = [
  {
    name: 'Smileys', icon: '😀',
    items: ['😀','😃','😄','😁','😅','😂','🤣','😊','😇','🙂','😉','😌','😍','🥰','😘','😗','😋','😛','😜','🤪','😝','🤑','🤗','🤭','🤫','🤔','🤐','😐','😑','😶','😏','😒','🙄','😬','😮','😯','😲','😳','🥺','😢','😭','😤','😡','🤬','😈','👿','💀','☠️','🤡','👻','👽','🤖'],
  },
  {
    name: 'People', icon: '👋',
    items: ['👋','🤚','🖐️','✋','🖖','👌','🤌','🤏','✌️','🤞','🤟','🤘','🤙','👈','👉','👆','👇','👍','👎','✊','👊','🤛','🤜','👏','🙌','👐','🤲','🤝','🙏','💪','🧠','👀','👁️','👅','👄','💋','👶','🧒','👦','👧','🧑','👨','👩','🧔','👴','👵','🧓'],
  },
  {
    name: 'Nature', icon: '🐵',
    items: ['🐵','🐒','🦍','🐶','🐕','🦮','🐩','🐺','🦊','🦝','🐱','🐈','🦁','🐯','🐅','🐆','🐴','🐎','🦄','🦓','🦌','🐮','🐂','🐄','🐷','🐖','🐗','🐽','🐏','🐑','🐐','🐪','🐫','🦒','🐘','🦏','🐭','🐹','🐰','🐇','🐿️','🦔','🦇','🐻','🐨','🐼','🦥','🦦','🦨','🦘','🦡','🐔','🐓','🐣','🐤','🐥','🐦','🐧','🕊️','🦅','🦆','🦉','🐸','🐊','🐢','🦎','🐍','🐲','🐉','🦕','🦖','🐳','🐋','🐬','🐟','🐠','🐡','🦈','🐙','🐚','🐌','🦋','🐛','🐜','🐝','🐞','🦗','🕷️','🦂','🦟','🦠','🌱','🌿','☘️','🍀','🌲','🌳','🌴','🌵','🌾','🌻','🌼','🌸','🌺','🌷','🌹','🍄'],
  },
  {
    name: 'Food', icon: '🍔',
    items: ['🍇','🍈','🍉','🍊','🍋','🍌','🍍','🥭','🍎','🍏','🍐','🍑','🍒','🍓','🥝','🍅','🥑','🍆','🥔','🥕','🌽','🌶️','🥒','🥬','🥦','🧄','🧅','🍞','🥐','🥖','🥨','🧀','🥞','🧇','🥓','🥩','🍗','🍖','🌭','🍔','🍟','🍕','🥪','🥙','🧆','🌮','🌯','🥗','🥘','🫕','🥫','🍝','🍜','🍲','🍛','🍣','🍱','🥟','🦪','🍤','🍙','🍚','🍘','🍥','🥠','🥮','🍢','🍡','🍧','🍨','🍦','🥧','🧁','🍰','🎂','🍮','🍭','🍬','🍫','🍿','🍩','🍪','🌰','🥜','🍯','🥛','☕','🍵','🧃','🥤','🍶','🍺','🍻','🥂','🍷','🥃','🍸','🍹','🧊'],
  },
  {
    name: 'Activities', icon: '⚽',
    items: ['⚽','🏀','🏈','⚾','🥎','🎾','🏐','🏉','🥏','🎱','🪀','🏓','🏸','🏒','🏑','🥍','🏏','⛳','🥊','🥋','🎽','🛹','🛶','⛵','🚣','🏊','🤽','🤸','🤺','⛷️','🏂','🏄','🚴','🤼','🤾','🤹','🎪','🎭','🎨','🎬','🎤','🎧','🎼','🎹','🥁','🎷','🎺','🎸','🪕','🎻','🎲','♟️','🎯','🎳','🎮','🕹️'],
  },
  {
    name: 'Travel', icon: '🚗',
    items: ['🚗','🚙','🚕','🚌','🚎','🏎️','🚓','🚑','🚒','🚐','🛻','🚚','🚛','🚜','🏍️','🛵','🛺','🚲','🛴','🚨','🚔','🚍','🚘','🚖','🛞','⛽','🛤️','🛣️','🗺️','🌍','🌎','🌏','🧭','🏔️','⛰️','🌋','🗻','🏕️','🏖️','🏜️','🏝️','🏞️','🏟️','🏛️','🏗️','🧱','🏠','🏡','🏘️','🏚️','🏢','🏣','🏤','🏥','🏦','🏨','🏩','🏪','🏫','🏬','🏭','🏯','🏰','💒','🗼','🗽','⛪','🕌','🛕','🕍','⛩️','🕋','⛲','⛺','🌁','🌃','🏙️','🌄','🌅','🌆','🌇','🌉','🎠','🎡','🎢','🚂','🚃','🚄','🚅','🚆','🚇','🚈','🚉','🚊','🚝','🚞','🚋','🚌','🚍','🚎','🚐','🚑','🚒','🚓','🚔','🚕','🚖','🚗','🚘','🚙','🚚','🚛','🚜','🚲','🛴','🛵','🛺','🚏','🛑','🚦','🚥','⛵','🛶','🚤','🛳️','⛴️','🛥️','🚢','✈️','🛩️','🛫','🛬','🪂','💺','🚁','🚟','🚠','🚡','🛰️','🚀','🛸'],
  },
  {
    name: 'Objects', icon: '💡',
    items: ['💡','🔦','🏮','🪔','💻','🖥️','🖨️','⌨️','🖱️','🖲️','💽','💾','💿','📀','📼','📷','📸','📹','🎥','📽️','🎞️','📞','☎️','📟','📠','📺','📻','🎙️','🎚️','🎛️','🧭','⏱️','⏲️','⏰','🕰️','⌛','⏳','📡','🔋','🔌','💵','💴','💶','💷','💰','💳','💎','⚖️','🔧','🔨','⚒️','🛠️','⛏️','🔩','⚙️','🧰','🧲','🔗','⛓️','🧪','🧫','🧬','🔬','🔭','📐','📏','📎','🖇️','📁','📂','🗂️','📅','📆','🗒️','🗓️','📇','📈','📉','📊','📋','📌','📍','📎','🖇️','📏','📐','✂️','🗃️','🗄️','🗑️','🔒','🔓','🔏','🔐','🔑','🗝️','🔨','🪓','⛏️','⚔️','🛡️','🏹','🔫','🪃','🏹','🛡️'],
  },
  {
    name: 'Symbols', icon: '❤️',
    items: ['❤️','🧡','💛','💚','💙','💜','🖤','🤍','🤎','💔','❣️','💕','💞','💓','💗','💖','💘','💝','💟','☮️','✝️','☪️','🕉️','☸️','✡️','🔯','🕎','☯️','☦️','🛐','⛎','♈','♉','♊','♋','♌','♍','♎','♏','♐','♑','♒','♓','🆔','⚕️','♿','🚹','🚺','🚻','🚼','🚾','🛂','🛃','🛄','🛅','⚠️','🚸','⛔','🚫','🚳','🚭','🚯','🚱','🚷','📵','🔞','☢️','☣️','⬆️','⬇️','➡️','⬅️','↗️','↘️','↙️','↖️','↕️','↔️','🔄','◀️','▶️','🔼','🔽','⏪','⏩','⏫','⏬','🔼','🔽','✅','❌','❓','❔','❕','❗','➕','➖','➗','♾️','💱','💲','🔄','🔃','🆕','🆙','🆒','🆓','🆔','🆕','🆖','🆗','🆙','🆚'],
  },
  {
    name: 'Flags', icon: '🚩',
    items: ['🏳️','🏴','🏁','🚩','🎌','🏴‍☠️','🇦🇫','🇦🇱','🇩🇿','🇦🇸','🇦🇩','🇦🇴','🇦🇮','🇦🇶','🇦🇬','🇦🇷','🇦🇲','🇦🇼','🇦🇺','🇦🇹','🇦🇿','🇧🇸','🇧🇭','🇧🇩','🇧🇧','🇧🇾','🇧🇪','🇧🇿','🇧🇯','🇧🇲','🇧🇹','🇧🇴','🇧🇦','🇧🇼','🇧🇷','🇧🇳','🇧🇬','🇧🇫','🇧🇮','🇨🇻','🇰🇭','🇨🇲','🇨🇦','🇨🇫','🇹🇩','🇨🇱','🇨🇳','🇨🇴','🇰🇲','🇨🇬','🇨🇩','🇨🇷','🇭🇷','🇨🇺','🇨🇾','🇨🇿','🇩🇰','🇩🇯','🇩🇲','🇩🇴','🇪🇨','🇪🇬','🇸🇻','🇬🇶','🇪🇷','🇪🇪','🇪🇹','🇪🇺','🇫🇮','🇫🇷','🇬🇦','🇬🇲','🇬🇪','🇩🇪','🇬🇭','🇬🇷','🇬🇩','🇬🇹','🇬🇳','🇬🇼','🇬🇾','🇭🇹','🇭🇳','🇭🇰','🇭🇺','🇮🇸','🇮🇳','🇮🇩','🇮🇷','🇮🇶','🇮🇪','🇮🇱','🇮🇹','🇨🇮','🇯🇲','🇯🇵','🇯🇴','🇰🇿','🇰🇪','🇰🇮','🇽🇰','🇰🇼','🇰🇬','🇱🇦','🇱🇻','🇱🇧','🇱🇸','🇱🇷','🇱🇾','🇱🇮','🇱🇹','🇱🇺','🇲🇴','🇲🇬','🇲🇼','🇲🇾','🇲🇻','🇲🇱','🇲🇹','🇲🇭','🇲🇷','🇲🇺','🇲🇽','🇫🇲','🇲🇩','🇲🇨','🇲🇳','🇲🇪','🇲🇦','🇲🇿','🇲🇲','🇳🇦','🇳🇷','🇳🇵','🇳🇱','🇳🇨','🇳🇿','🇳🇮','🇳🇪','🇳🇬','🇳🇺','🇰🇵','🇲🇰','🇳🇴','🇴🇲','🇵🇰','🇵🇼','🇵🇸','🇵🇦','🇵🇬','🇵🇾','🇵🇪','🇵🇭','🇵🇱','🇵🇹','🇵🇷','🇶🇦','🇷🇴','🇷🇺','🇷🇼','🇼🇸','🇸🇲','🇸🇹','🇸🇦','🇸🇳','🇷🇸','🇸🇨','🇸🇱','🇸🇬','🇸🇰','🇸🇮','🇸🇧','🇸🇴','🇿🇦','🇰🇷','🇸🇸','🇪🇸','🇱🇰','🇸🇩','🇸🇷','🇸🇿','🇸🇪','🇨🇭','🇸🇾','🇹🇼','🇹🇯','🇹🇿','🇹🇭','🇹🇱','🇹🇬','🇹🇴','🇹🇹','🇹🇳','🇹🇷','🇹🇲','🇹🇻','🇺🇬','🇺🇦','🇦🇪','🇬🇧','🇺🇳','🇺🇸','🇺🇾','🇺🇿','🇻🇺','🇻🇦','🇻🇪','🇻🇳','🇾🇪','🇿🇲','🇿🇼'],
  },
];

@Component({
  selector: 'app-emoji-picker',
  standalone: true,
  imports: [MatIconModule, MatButtonModule, FormsModule],
  template: `
    <div class="picker-wrapper">
      <button mat-stroked-button type="button" class="trigger" (click)="open.set(!open())">
        @if (value()) {
          <span class="preview">{{ value() }}</span>
        } @else {
          <mat-icon>emoji_emotions</mat-icon>
        }
        <span class="label">{{ value() ? 'Change icon' : 'Pick icon' }}</span>
      </button>

      @if (open()) {
        <div class="backdrop" (click)="open.set(false)"></div>
        <div class="panel" (click)="$event.stopPropagation()">
          <div class="search-row">
            <mat-icon class="search-icon">search</mat-icon>
            <input
              class="search-input"
              [(ngModel)]="searchQuery"
              placeholder="Search emoji..."
              (keydown)="$event.stopPropagation()"
            />
          </div>

          <div class="tabs">
            @for (cat of categories; track cat.name) {
              <button
                type="button"
                class="tab"
                [class.active]="activeCategory() === cat.name"
                (click)="scrollTo(cat.name)"
                [title]="cat.name"
              >
                {{ cat.icon }}
              </button>
            }
          </div>

          <div class="scroll-area" #scrollArea>
            @for (cat of categories; track cat.name) {
              @if (filtered(cat).length) {
                <div class="cat-section" [id]="'emoji-cat-' + cat.name">
                  <div class="cat-header">{{ cat.icon }} {{ cat.name }}</div>
                  <div class="grid">
                    @for (emoji of filtered(cat); track emoji) {
                      <button
                        type="button"
                        class="emoji-btn"
                        [class.selected]="value() === emoji"
                        (click)="select(emoji)"
                        [title]="emoji"
                      >
                        {{ emoji }}
                      </button>
                    }
                  </div>
                </div>
              }
            }
          </div>
        </div>
      }
    </div>
  `,
  styles: `
    .picker-wrapper { position: relative; }
    .trigger {
      display: inline-flex; align-items: center; gap: 6px; padding: 4px 12px;
    }
    .preview { font-size: 1.4rem; line-height: 1; }
    .label { font-size: 0.875rem; }
    .backdrop { position: fixed; inset: 0; z-index: 999; }
    .panel {
      position: absolute; top: calc(100% + 4px); left: 0; z-index: 1000;
      background: #fff; border: 1px solid rgba(0,0,0,0.12); border-radius: 8px;
      box-shadow: 0 4px 16px rgba(0,0,0,0.15); width: 340px;
      display: flex; flex-direction: column;
    }
    .search-row {
      display: flex; align-items: center; gap: 6px;
      padding: 8px 10px; border-bottom: 1px solid rgba(0,0,0,0.08);
    }
    .search-icon { font-size: 1.1rem; color: rgba(0,0,0,0.4); }
    .search-input {
      flex: 1; border: none; outline: none; font-size: 0.875rem;
      font-family: inherit; background: transparent;
    }
    .tabs {
      display: flex; gap: 2px; padding: 6px 8px;
      border-bottom: 1px solid rgba(0,0,0,0.08); overflow-x: auto;
    }
    .tab {
      width: 32px; height: 32px; display: flex; align-items: center;
      justify-content: center; border: none; border-radius: 6px;
      background: transparent; cursor: pointer; font-size: 1.1rem;
      flex-shrink: 0;
    }
    .tab:hover { background: rgba(0,0,0,0.06); }
    .tab.active { background: var(--mat-sys-secondary-container, #e0e0e0); }
    .scroll-area {
      max-height: 280px; overflow-y: auto; padding: 4px 8px 8px;
    }
    .cat-section { margin-top: 4px; }
    .cat-header {
      font-size: 0.75rem; color: rgba(0,0,0,0.5); padding: 4px 2px;
      font-weight: 500;
    }
    .grid { display: flex; flex-wrap: wrap; gap: 2px; }
    .emoji-btn {
      width: 34px; height: 34px; display: flex; align-items: center;
      justify-content: center; font-size: 1.15rem; border: none;
      border-radius: 6px; background: transparent; cursor: pointer; padding: 0;
    }
    .emoji-btn:hover { background: rgba(0,0,0,0.06); }
    .emoji-btn.selected { background: var(--mat-sys-secondary-container, #e0e0e0); }
  `,
})
export class EmojiPickerComponent {
  readonly value = input('');
  readonly valueChange = output<string>();
  readonly open = signal(false);
  readonly searchQuery = signal('');
  readonly activeCategory = signal('');

  readonly categories = CATEGORIES;

  filtered(cat: EmojiCategory): string[] {
    const q = this.searchQuery().toLowerCase();
    if (!q) return cat.items;
    return cat.items.filter((e) => e.includes(q));
  }

  select(emoji: string): void {
    this.valueChange.emit(emoji);
    this.open.set(false);
    this.searchQuery.set('');
  }

  scrollTo(catName: string): void {
    this.activeCategory.set(catName);
    const el = document.getElementById('emoji-cat-' + catName);
    el?.scrollIntoView({ behavior: 'smooth', block: 'start' });
  }
}
