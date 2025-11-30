import {Injectable, computed, signal} from '@angular/core';
import {MoneyModel} from '../models/money.model';

@Injectable({ providedIn: 'root' })
export class MoneyService {
  private readonly _money = signal<MoneyModel>(new MoneyModel());
  public readonly money = this._money.asReadonly();

  set(val: MoneyModel | Partial<MoneyModel>) {
    const model = new MoneyModel(val);
    this._money.set(model);
  }

  public readonly moneyTooltip = computed(() => {
    const m = this._money();
    return `<div>
      Кредиты : ${m.cr}<br/>
      Минералы : ${m.minerals}<br/>
      Газ : ${m.gas}
    </div>`;
  });
}
