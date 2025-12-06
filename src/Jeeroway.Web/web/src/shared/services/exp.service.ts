import {Injectable} from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ExpService {
  private readonly ups: number[] = [
    200000, 175000, 150000, 75000, 60000, 30000, 27000, 23000, 21000,
    19000, 17000, 15500, 14000, 12500, 12000, 11000, 10000, 9000, 8000,
    7000, 6000, 5000, 4600, 4200, 3800, 3350, 2900, 2500, 2200, 2050,
    1850, 1650, 1450, 1300, 1100, 950, 830, 670, 530, 410, 350, 280,
    215, 160, 110, 75, 45, 20, 0
  ];

  private readonly lvls: number[] = [110, 410, 1300, 2500, 5000, 12500, 30000, 300000];

  public nextUp(exp: number, prev: boolean = false): number {
    if (prev) {
      // Return the next threshold strictly greater than current exp.
      // For exp=0 should be 20 (next element in ups), not 0.
      for (let i = this.ups.length - 1; i >= 0; i--) {
        if (this.ups[i] > exp) return this.ups[i];
      }
      return 0;
    } else {
      let now = 0;
      let next = 20;
      for (let i = 0; i < this.ups.length; i++) {
        if (this.ups[i] <= exp) {
          now = this.ups[i];
          next = this.ups[i - 1];
          break;
        }
      }
      if (exp > next) exp = next;
      return (exp - now) * 87 / (next - now);
    }
    return 0;
  }

  public expHtml(userExp: number): string {
    const exp = userExp ?? 0;
    const nextUpRaw = this.nextUp(exp, true) as unknown as number;
    const nextUp = Number.isFinite(nextUpRaw) ? nextUpRaw : 0;
    const nextLevel = this.lvls.find(v => v > exp) ?? this.lvls[this.lvls.length - 1];

    const toUp = Math.max(0, nextUp - exp);
    const toLevel = Math.max(0, nextLevel - exp);

    return `Опыт: ${exp}<br/>До апа : ${toUp}<br/>До уровня : ${toLevel}`;
  }
}
