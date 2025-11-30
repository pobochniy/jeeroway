export class MoneyModel {
  cr: number = 0;
  minerals: number = 0;
  gas: number = 0;

  constructor(val?: Partial<MoneyModel>) {
    if (val) Object.assign(this, val);
  }
}
