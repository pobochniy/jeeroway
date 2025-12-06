import { Component, input, computed, ChangeDetectionStrategy } from '@angular/core';
import { SizeEnum } from '../enums/size.enum';

@Component({
  selector: 'app-fa-icon',
  templateUrl: './fa-icon.component.html',
  styleUrls: ['./fa-icon.component.css'],
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class FaIconComponent {
  src = input.required<string>();
  size = input<SizeEnum>(SizeEnum.M);
  color = input<string>('black');

  sizePx = computed(() => {
    let res = 10;
    switch (this.size()) {
      case SizeEnum.XS: res = 10; break;
      case SizeEnum.S: res = 15; break;
      case SizeEnum.M: res = 20; break;
      case SizeEnum.L: res = 35; break;
      case SizeEnum.XL: res = 30; break;
    }
    return res + 'px';
  });
}
