/** Мета данные о роботе */
export class RoboControlModel {
  /** GUID  Идентификатор робота, выдаётся при регистрации */
  public roboId: string = '';

  /** Время отправки сообщения */
  public timeJs: number = 0;

  public w: boolean = false;
  public s: boolean = false;
  public a: boolean = false;
  public d: boolean = false;

  public clearControls() {
    this.w = false;
    this.s = false;
    this.a = false;
    this.d = false;
  }
}
